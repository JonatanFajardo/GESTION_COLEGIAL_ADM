/* ============================================================
   Configuración de Cobranza — Hub JS
   Consume los endpoints existentes de cada catálogo:
     /{Controller}/ListAsync, /DetailAsync (o /FindAsync),
     /CreateAsync, /DeleteAsync
   No añade lógica de negocio; solo orquesta UI.

   Nota sobre modelos:
   - Los ViewModels de List de Cobranza usan camelCase
     (ConceptoPagoId, Descripcion, EsActivo, ...), a diferencia
     del hub de Institución que usa prefijos (Mda_, Niv_, ...).
   - Tarifas y Descuentos tienen 4+ campos editables, por lo
     que en Edit/New se redirige al formulario completo del
     controlador original (Opción B).
   ============================================================ */
(function () {
    'use strict';

    // --------------------------------------------------
    // Configuración de los 5 catálogos (orden = flujo cobranza)
    // --------------------------------------------------
    var CATALOGS = {
        conceptospago: {
            controller: 'ConceptosPago',
            singular: 'Concepto de Pago',
            plural: 'Conceptos de Pago',
            article: 'el',
            genero: 'm',
            icon: 'bi-tag',
            hint: 'Rubros por los que la institución cobra (matrícula, mensualidad, transporte, etc.).',
            placeholder: 'Ej: Matrícula, Mensualidad, Transporte...',
            // Nombres de campos que devuelve /ListAsync (ver ConceptoPagoListViewModel)
            fieldId: 'ConceptoPagoId',
            fieldDesc: 'Descripcion',
            fieldEsActivo: 'EsActivo',
            fieldUsuarioRegistraNombre: 'NombreCompletoUsuarioRegistra',
            fieldFechaRegistra: 'FechaRegistro',
            fieldUsuarioModificaNombre: 'NombreCompletoUsuarioModifica',
            fieldFechaModifica: 'FechaModifica',
            // Payload extra en el POST /CreateAsync
            extraFields: ['EsRecurrente', 'EsObligatorio'],
            // Uso principal a mostrar como badge en cards/tabla
            usageBadge: function (row) {
                var count = row.CantTarifas || 0;
                return { label: count + ' tarifa' + (count === 1 ? '' : 's'), tone: 'indigo' };
            },
            // Modo Edit: 'inline' (Opción A) | 'redirect' (Opción B)
            editMode: 'inline'
        },
        tarifas: {
            controller: 'Tarifas',
            singular: 'Tarifa',
            plural: 'Tarifas',
            article: 'la',
            genero: 'f',
            icon: 'bi-cash-stack',
            hint: 'Monto base a cobrar por cada concepto según nivel y grado.',
            placeholder: 'Ej: Mensualidad Primaria 2026...',
            fieldId: 'TarifaId',
            // TarifaListViewModel no tiene Descripcion; usamos Concepto como "descripción visible"
            fieldDesc: 'Concepto',
            fieldEsActivo: 'EsActivo',
            fieldUsuarioRegistraNombre: 'NombreCompletoUsuarioRegistra',
            fieldFechaRegistra: 'FechaRegistro',
            fieldUsuarioModificaNombre: 'NombreCompletoUsuarioModifica',
            fieldFechaModifica: 'FechaModifica',
            usageBadge: function (row) {
                var monto = row.Monto != null ? row.Monto : 0;
                return { label: 'L. ' + Number(monto).toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }), tone: 'emerald' };
            },
            editMode: 'redirect',
            editRedirectNew: '/Tarifas/CreateAsync',
            editRedirectEdit: '/Tarifas/FindAsync?id={id}',
            // Detail extra
            detailExtraLabel: 'Monto vigente',
            detailExtraGetter: function (item) {
                var m = item.Monto != null ? item.Monto : (item.Tar_Monto != null ? item.Tar_Monto : null);
                if (m == null) return '—';
                return 'L. ' + Number(m).toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
        },
        descuentos: {
            controller: 'Descuentos',
            singular: 'Descuento',
            plural: 'Descuentos',
            article: 'el',
            genero: 'm',
            icon: 'bi-percent',
            hint: 'Rebajas aplicables a los cobros (beca, hermano, pronto pago, empleado).',
            placeholder: 'Ej: Beca académica, Hermano, Pronto pago...',
            fieldId: 'DescuentoId',
            fieldDesc: 'Descripcion',
            fieldEsActivo: 'EsActivo',
            fieldUsuarioRegistraNombre: 'NombreCompletoUsuarioRegistra',
            fieldFechaRegistra: 'FechaRegistro',
            fieldUsuarioModificaNombre: 'NombreCompletoUsuarioModifica',
            fieldFechaModifica: 'FechaModifica',
            usageBadge: function (row) {
                var val = row.Valor != null ? row.Valor : 0;
                var tipo = (row.TipoDescuento || '').toString().toLowerCase();
                var isPct = tipo.indexOf('porcent') >= 0 || tipo === '%';
                var label = isPct
                    ? Number(val).toLocaleString('es-HN', { maximumFractionDigits: 2 }) + ' %'
                    : 'L. ' + Number(val).toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                return { label: label, tone: 'amber' };
            },
            editMode: 'redirect',
            editRedirectNew: '/Descuentos',
            editRedirectEdit: '/Descuentos',
            detailExtraLabel: 'Valor',
            detailExtraGetter: function (item) {
                var v = item.Valor != null ? item.Valor : (item.Des_Valor != null ? item.Des_Valor : null);
                if (v == null) return '—';
                var tipo = (item.TipoDescuento || item.Des_TipoDescuento || '').toString().toLowerCase();
                var isPct = tipo.indexOf('porcent') >= 0 || tipo === '%';
                return isPct
                    ? Number(v).toLocaleString('es-HN', { maximumFractionDigits: 2 }) + ' %'
                    : 'L. ' + Number(v).toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
        },
        formaspago: {
            controller: 'FormasPago',
            singular: 'Forma de Pago',
            plural: 'Formas de Pago',
            article: 'la',
            genero: 'f',
            icon: 'bi-credit-card',
            hint: 'Medios aceptados para recibir un pago (efectivo, transferencia, tarjeta, cheque).',
            placeholder: 'Ej: Efectivo, Transferencia, Tarjeta...',
            fieldId: 'FormaPagoId',
            fieldDesc: 'Descripcion',
            fieldEsActivo: 'EsActivo',
            fieldUsuarioRegistraNombre: 'NombreCompletoUsuarioRegistra',
            fieldFechaRegistra: 'FechaRegistro',
            fieldUsuarioModificaNombre: 'NombreCompletoUsuarioModifica',
            fieldFechaModifica: 'FechaModifica',
            extraFields: [],
            usageBadge: function (row) {
                var count = row.CantidadPagos || 0;
                return { label: count + ' pago' + (count === 1 ? '' : 's'), tone: 'cyan' };
            },
            editMode: 'inline'
        },
        estadospago: {
            controller: 'EstadosPago',
            singular: 'Estado de Pago',
            plural: 'Estados de Pago',
            article: 'el',
            genero: 'm',
            icon: 'bi-flag',
            hint: 'Estatus posibles de una cuenta por cobrar (pendiente, parcial, pagado, en mora).',
            placeholder: 'Ej: Pendiente, Pagado, En mora...',
            fieldId: 'EstadoPagoId',
            fieldDesc: 'Descripcion',
            fieldEsActivo: 'EsActivo',
            fieldUsuarioRegistraNombre: 'NombreCompletoUsuarioRegistra',
            fieldFechaRegistra: 'FechaRegistro',
            fieldUsuarioModificaNombre: 'NombreCompletoUsuarioModifica',
            fieldFechaModifica: 'FechaModifica',
            extraFields: [],
            usageBadge: function (row) {
                var count = row.CantidadCuentas || 0;
                return { label: count + ' cuenta' + (count === 1 ? '' : 's'), tone: 'indigo' };
            },
            editMode: 'inline'
        }
    };

    // --------------------------------------------------
    // Estado
    // --------------------------------------------------
    var state = {
        activeKey: 'conceptospago',
        view: 'cards',                 // 'cards' | 'table'
        search: '',
        data: {}                       // { conceptospago: [...], tarifas: [...], ... }
    };

    // --------------------------------------------------
    // Utils
    // --------------------------------------------------
    function esc(s) {
        if (s === null || s === undefined) return '';
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function fmtDate(v) {
        if (!v) return '—';
        try {
            var m = /\/Date\((\d+)/.exec(v);
            var d = m ? new Date(parseInt(m[1], 10)) : new Date(v);
            if (isNaN(d.getTime())) return String(v);
            var pad = function (n) { return n < 10 ? '0' + n : n; };
            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear()
                + ' ' + pad(d.getHours()) + ':' + pad(d.getMinutes());
        } catch (e) {
            return String(v);
        }
    }

    function currentCatalog() { return CATALOGS[state.activeKey]; }

    function url(catalog, action) {
        return '/' + catalog.controller + '/' + action;
    }

    function notify(type, msg) {
        try {
            if (window.toastr) {
                (toastr[type] || toastr.info)(msg);
                return;
            }
            if (window.Swal) {
                Swal.fire({ icon: type === 'success' ? 'success' : type === 'error' ? 'error' : 'info', title: msg, toast: true, position: 'top-end', showConfirmButton: false, timer: 2600 });
                return;
            }
        } catch (e) {}
        console.log('[' + type + '] ' + msg);
    }

    // --------------------------------------------------
    // Data loading
    // --------------------------------------------------
    function fetchCatalog(key) {
        var cat = CATALOGS[key];
        return $.ajax({
            url: url(cat, 'ListAsync'),
            type: 'GET',
            dataType: 'json'
        }).then(function (resp) {
            var rows = Array.isArray(resp) ? resp
                       : (resp && resp.data) ? resp.data
                       : (resp && resp.item) ? resp.item
                       : [];
            state.data[key] = rows;
            updateTabCount(key, rows.length);
            return rows;
        }).catch(function () {
            state.data[key] = [];
            updateTabCount(key, 0);
            return [];
        });
    }

    function fetchAll() {
        var keys = Object.keys(CATALOGS);
        var jobs = keys.map(fetchCatalog);
        return $.when.apply($, jobs);
    }

    // --------------------------------------------------
    // Rendering
    // --------------------------------------------------
    function updateTabCount(key, count) {
        var $tab = $('.cc-tab-item[data-catalog="' + key + '"]');
        $tab.find('[data-count]').text(count);
    }

    function filterRows() {
        var cat = currentCatalog();
        var rows = state.data[state.activeKey] || [];
        var q = state.search.trim().toLowerCase();
        if (!q) return rows;
        return rows.filter(function (r) {
            var d = (r[cat.fieldDesc] || '').toString().toLowerCase();
            var id = (r[cat.fieldId] || '').toString().toLowerCase();
            return d.indexOf(q) >= 0 || id.indexOf(q) >= 0;
        });
    }

    function renderCatalogInfo() {
        var cat = currentCatalog();
        $('#cc-info-title').text(cat.plural);
        $('#cc-info-desc').text(cat.hint);
        $('#cc-new-label').text('Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular);
    }

    function usageBadgeHtml(cat, row) {
        if (!cat.usageBadge) return '';
        var b = cat.usageBadge(row) || {};
        var tone = b.tone || 'indigo';
        return '<span class="cc-usage-badge tone-' + tone + '">' + esc(b.label) + '</span>';
    }

    function renderCards() {
        var $grid = $('#cc-cards-grid').empty();
        var cat = currentCatalog();
        var rows = filterRows();

        if (rows.length === 0) {
            $('#cc-empty').prop('hidden', false);
            $('#cc-cards-grid').prop('hidden', true);
            return;
        }
        $('#cc-empty').prop('hidden', true);
        $('#cc-cards-grid').prop('hidden', false);

        rows.forEach(function (r) {
            var id = r[cat.fieldId];
            var desc = r[cat.fieldDesc] || '(sin descripción)';
            var esActivo = cat.fieldEsActivo ? !!r[cat.fieldEsActivo] : null;
            var stateBadge = '';
            if (esActivo === true)  stateBadge = '<span class="cc-card-badge active"><i class="bi bi-check-circle-fill"></i> Activo</span>';
            if (esActivo === false) stateBadge = '<span class="cc-card-badge inactive"><i class="bi bi-slash-circle-fill"></i> Inactivo</span>';

            var $card = $(
                '<div class="cc-card ' + (esActivo === false ? 'cc-card-inactive' : '') + '" data-id="' + esc(id) + '">' +
                    '<div class="cc-card-head">' +
                        '<div class="cc-card-icon"><i class="bi ' + cat.icon + '"></i></div>' +
                        '<span class="cc-card-id">#' + esc(id) + '</span>' +
                    '</div>' +
                    '<div class="cc-card-body">' +
                        '<h4>' + esc(desc) + '</h4>' +
                        '<div class="cc-card-meta">' + stateBadge + ' ' + usageBadgeHtml(cat, r) + '</div>' +
                    '</div>' +
                    '<div class="cc-card-actions">' +
                        '<button type="button" class="cc-card-action view"   title="Ver detalle"><i class="bi bi-eye"></i></button>' +
                        '<button type="button" class="cc-card-action edit"   title="Editar"><i class="bi bi-pencil"></i></button>' +
                        '<button type="button" class="cc-card-action delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</div>' +
                '</div>'
            );
            $grid.append($card);
        });

        $grid.append(
            '<div class="cc-card-new" role="button" tabindex="0">' +
                '<i class="bi bi-plus-circle"></i>' +
                '<span>Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular + '</span>' +
                '<small>Agregar un registro nuevo</small>' +
            '</div>'
        );
    }

    function renderTable() {
        var $body = $('#cc-table-body').empty();
        var cat = currentCatalog();
        var rows = filterRows();

        if (rows.length === 0) {
            $body.append(
                '<tr><td colspan="6" style="text-align:center; padding:2rem; color:#94a3b8;">' +
                    '<i class="bi bi-inbox" style="font-size:1.5rem; display:block; margin-bottom:0.35rem;"></i>' +
                    'Sin registros' +
                '</td></tr>'
            );
            return;
        }

        rows.forEach(function (r) {
            var id = r[cat.fieldId];
            var desc = r[cat.fieldDesc] || '(sin descripción)';
            var esActivo = cat.fieldEsActivo ? !!r[cat.fieldEsActivo] : null;
            var estadoCell = esActivo === true
                ? '<span class="cc-card-badge active"><i class="bi bi-check-circle-fill"></i> Activo</span>'
                : esActivo === false
                ? '<span class="cc-card-badge inactive"><i class="bi bi-slash-circle-fill"></i> Inactivo</span>'
                : '<span style="color:#94a3b8;">—</span>';
            var fecha = fmtDate(r[cat.fieldFechaRegistra]);
            var usr = r[cat.fieldUsuarioRegistraNombre] || '';
            var usoCell = usageBadgeHtml(cat, r) || '<span style="color:#94a3b8;">—</span>';

            var $tr = $(
                '<tr data-id="' + esc(id) + '">' +
                    '<td><span class="badge-id">#' + esc(id) + '</span></td>' +
                    '<td><strong>' + esc(desc) + '</strong></td>' +
                    '<td>' + usoCell + '</td>' +
                    '<td>' + estadoCell + '</td>' +
                    '<td>' +
                        '<div style="font-size:0.8rem; color:#64748b;">' + esc(fecha) + '</div>' +
                        (usr ? '<div style="font-size:0.75rem; color:#94a3b8;">por ' + esc(usr) + '</div>' : '') +
                    '</td>' +
                    '<td class="actions-cell">' +
                        '<button type="button" class="cc-card-action view"   title="Detalle"><i class="bi bi-eye"></i></button> ' +
                        '<button type="button" class="cc-card-action edit"   title="Editar"><i class="bi bi-pencil"></i></button> ' +
                        '<button type="button" class="cc-card-action delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</td>' +
                '</tr>'
            );
            $body.append($tr);
        });
    }

    function render() {
        renderCatalogInfo();
        if (state.view === 'cards') {
            $('#cc-cards-view').prop('hidden', false);
            $('#cc-table-view').prop('hidden', true);
            renderCards();
        } else {
            $('#cc-cards-view').prop('hidden', true);
            $('#cc-table-view').prop('hidden', false);
            renderTable();
        }
    }

    // --------------------------------------------------
    // Modal: Crear / Editar
    // --------------------------------------------------
    function openEditModal(row) {
        var cat = currentCatalog();
        var isNew = !row;

        // Reset visibilidad de secciones opcionales
        $('.cc-only-conceptospago').prop('hidden', true);
        $('#cc-edit-estado-wrap').prop('hidden', true);
        $('#cc-edit-redirect').prop('hidden', true);
        $('#cc-edit-redirect-btn').prop('hidden', true).attr('href', '#');
        $('#cc-edit-save').prop('hidden', false);
        $('#cc-edit-descripcion').prop('disabled', false);

        $('#cc-edit-id').val(isNew ? 0 : row[cat.fieldId]);
        $('#cc-edit-descripcion')
            .val(isNew ? '' : (row[cat.fieldDesc] || ''))
            .attr('placeholder', cat.placeholder);
        $('#cc-edit-hint').text(cat.hint);

        var titulo = isNew ? 'Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular
                           : 'Editar ' + cat.singular;
        $('#cc-edit-title').text(titulo);
        $('#cc-edit-subtitle').text((isNew ? 'Complete los datos para crear un nuevo registro en el catálogo '
                                          : 'Modifique los datos del registro en el catálogo ') + cat.plural.toLowerCase());

        // Modo redirect: mostramos aviso y botón de redirección
        if (cat.editMode === 'redirect') {
            $('#cc-edit-redirect').prop('hidden', false);
            $('#cc-edit-redirect-title').text('Este catálogo requiere el formulario completo');
            var descTxt = isNew
                ? cat.singular + ' tiene varios campos obligatorios (concepto, nivel, monto, año, etc.). Serás llevado al formulario completo.'
                : 'Editar ' + cat.article + ' ' + cat.singular.toLowerCase() + ' requiere el formulario completo. Serás llevado a la pantalla dedicada.';
            $('#cc-edit-redirect-desc').text(descTxt);

            var href = isNew ? cat.editRedirectNew
                             : cat.editRedirectEdit.replace('{id}', encodeURIComponent(row[cat.fieldId]));
            $('#cc-edit-redirect-btn')
                .prop('hidden', false)
                .attr('href', href)
                .html('<i class="bi bi-box-arrow-up-right"></i> ' + (isNew ? 'Abrir formulario completo' : 'Editar en pantalla dedicada'));

            $('#cc-edit-save').prop('hidden', true);
            $('#cc-edit-descripcion').prop('disabled', true);
            $('#cc-edit-modal').modal('show');
            return;
        }

        // Modo inline: mostramos los campos según catálogo
        if (state.activeKey === 'conceptospago') {
            $('.cc-only-conceptospago').prop('hidden', false);
            $('#cc-edit-esrecurrente').prop('checked', isNew ? false : !!row.EsRecurrente);
            $('#cc-edit-esobligatorio').prop('checked', isNew ? false : !!row.EsObligatorio);
        }

        // Estado activo (los 3 inline lo tienen)
        if (cat.fieldEsActivo) {
            $('#cc-edit-estado-wrap').prop('hidden', false);
            var activo = isNew ? true : !!row[cat.fieldEsActivo];
            $('#cc-edit-esactivo').prop('checked', activo);
            $('#cc-edit-esactivo-label').text(activo ? 'Activo' : 'Inactivo');
        }

        $('#cc-edit-modal').modal('show');
        setTimeout(function () { $('#cc-edit-descripcion').trigger('focus'); }, 250);
    }

    function submitEdit() {
        var cat = currentCatalog();
        if (cat.editMode === 'redirect') return; // no debería llegar aquí

        var id = parseInt($('#cc-edit-id').val(), 10) || 0;
        var desc = $('#cc-edit-descripcion').val().trim();

        if (!desc) {
            notify('warning', 'La descripción es requerida.');
            $('#cc-edit-descripcion').trigger('focus');
            return;
        }

        var payload = {};
        payload[cat.fieldId] = id;
        payload.Descripcion = desc;

        if (cat.fieldEsActivo === 'EsActivo') {
            payload.EsActivo = $('#cc-edit-esactivo').prop('checked');
        }

        // Campos extra específicos
        if (state.activeKey === 'conceptospago') {
            payload.EsRecurrente = $('#cc-edit-esrecurrente').prop('checked');
            payload.EsObligatorio = $('#cc-edit-esobligatorio').prop('checked');
        }

        var $btn = $('#cc-edit-save').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Guardando...');

        $.ajax({
            url: url(cat, 'CreateAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', (id === 0 ? 'Registro creado.' : 'Registro actualizado.'));
                $('#cc-edit-modal').modal('hide');
                fetchCatalog(state.activeKey).done(render);
            } else {
                notify('error', (resp && resp.message) || 'No se pudo guardar el registro.');
            }
        }).fail(function () {
            notify('error', 'Error de conexión al guardar.');
        }).always(function () {
            $btn.prop('disabled', false).html('<i class="bi bi-check2"></i> Guardar');
        });
    }

    // --------------------------------------------------
    // Modal: Detalle
    // --------------------------------------------------
    function openDetailModal(id) {
        var cat = currentCatalog();
        $('#cc-detail-catalog-name').text('Catálogo · ' + cat.plural);
        $('#cc-detail-id').text('#' + id);
        $('#cc-detail-descripcion').text('Cargando...');
        $('#cc-detail-usuarioregistra').text('—');
        $('#cc-detail-fecharegistra').text('');
        $('#cc-detail-usuariomodifica').text('—');
        $('#cc-detail-fechamodifica').text('');
        $('#cc-detail-extra-wrap').prop('hidden', true);
        $('#cc-detail-modal').modal('show');

        $.ajax({
            url: url(cat, 'DetailAsync'),
            type: 'GET',
            data: { id: id },
            dataType: 'json'
        }).done(function (resp) {
            // AjaxResult(result, true) => { item, success }
            var item = (resp && resp.item) ? resp.item : (resp && resp.data) ? resp.data : resp;
            if (!item) {
                $('#cc-detail-descripcion').text('No se encontraron datos.');
                return;
            }

            // Descripción: intentamos prefijo (Detail) o camelCase (List)
            var desc = getDetailField(item, cat, 'Descripcion') ||
                       item[cat.fieldDesc] ||
                       findRowGetField(id, cat.fieldDesc) ||
                       '—';
            $('#cc-detail-descripcion').text(desc);

            // Campo extra (para Tarifas/Descuentos principalmente)
            if (cat.detailExtraGetter) {
                var extra = cat.detailExtraGetter(item);
                if (extra && extra !== '—') {
                    $('#cc-detail-extra-label').text(cat.detailExtraLabel || 'Uso');
                    $('#cc-detail-extra-value').text(extra);
                    $('#cc-detail-extra-wrap').prop('hidden', false);
                }
            }

            // Auditoría — buscamos en prefijo y camelCase
            $('#cc-detail-usuarioregistra').text(
                item.NombreCompletoUsuarioRegistra || item.UsuarioRegistraNombre || '—'
            );
            $('#cc-detail-fecharegistra').text(
                fmtDate(item.FechaRegistro || item.Cpa_FechaRegistra || item.Des_FechaRegistra || item.Fpa_FechaRegistra || item.Epa_FechaRegistra)
            );
            $('#cc-detail-usuariomodifica').text(
                item.NombreCompletoUsuarioModifica || item.UsuarioModificaNombre || '—'
            );
            $('#cc-detail-fechamodifica').text(
                fmtDate(item.FechaModifica || item.Cpa_FechaModifica || item.Des_FechaModifica || item.Fpa_FechaModifica || item.Epa_FechaModifica)
            );
        }).fail(function () {
            $('#cc-detail-descripcion').text('Error al cargar detalle.');
        });
    }

    // Utility: obtener campo prefijado (Cpa_Descripcion, Des_Descripcion, etc.)
    function getDetailField(item, cat, suffix) {
        var prefixes = { conceptospago: 'Cpa_', tarifas: 'Tar_', descuentos: 'Des_', formaspago: 'Fpa_', estadospago: 'Epa_' };
        var p = prefixes[state.activeKey];
        return p && item[p + suffix] ? item[p + suffix] : null;
    }

    function findRowGetField(id, field) {
        var row = findRow(id);
        return row ? row[field] : null;
    }

    // --------------------------------------------------
    // Modal: Eliminar
    // --------------------------------------------------
    function openDeleteModal(id, desc) {
        var cat = currentCatalog();
        $('#cc-delete-id').val(id);
        $('#cc-delete-name').text('"' + (desc || 'este registro') + '"');
        $('#cc-delete-catalog').text(cat.plural);
        $('#cc-delete-modal').modal('show');
    }

    function submitDelete() {
        var cat = currentCatalog();
        var id = parseInt($('#cc-delete-id').val(), 10) || 0;
        if (!id) return;

        var payload = {};
        // Tarifas usa TarifaViewModel con Tar_Id como campo; los demás usan {Entidad}Id
        if (state.activeKey === 'tarifas') {
            payload.Tar_Id = id;
        } else {
            payload[cat.fieldId] = id;
        }

        var $btn = $('#cc-delete-confirm').prop('disabled', true).text('Eliminando...');

        $.ajax({
            url: url(cat, 'DeleteAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', 'Registro eliminado.');
                $('#cc-delete-modal').modal('hide');
                fetchCatalog(state.activeKey).done(render);
            } else {
                notify('error', (resp && resp.message) || 'No se pudo eliminar. El registro puede estar en uso.');
            }
        }).fail(function () {
            notify('error', 'Error de conexión al eliminar.');
        }).always(function () {
            $btn.prop('disabled', false).text('Eliminar');
        });
    }

    // --------------------------------------------------
    // Row lookup helper
    // --------------------------------------------------
    function findRow(id) {
        var cat = currentCatalog();
        var rows = state.data[state.activeKey] || [];
        for (var i = 0; i < rows.length; i++) {
            if (String(rows[i][cat.fieldId]) === String(id)) return rows[i];
        }
        return null;
    }

    // --------------------------------------------------
    // Event bindings
    // --------------------------------------------------
    function bindEvents() {
        // Tabs
        $(document).on('click', '.cc-tab-item', function (e) {
            e.preventDefault();
            var key = $(this).data('catalog');
            if (!key || key === state.activeKey) return;
            $('.cc-tab-item').removeClass('active');
            $(this).addClass('active');
            state.activeKey = key;
            state.search = '';
            $('#cc-search').val('');
            render();
        });

        // Pills del hero
        $(document).on('click', '.cc-cascade-pill', function () {
            var key = $(this).data('jump');
            $('.cc-tab-item[data-catalog="' + key + '"]').trigger('click');
            $('html, body').animate({ scrollTop: $('.cc-tabs-card').offset().top - 20 }, 300);
        });

        // Toggle vista
        $(document).on('click', '.cc-view-btn', function () {
            var v = $(this).data('view');
            if (v === state.view) return;
            state.view = v;
            $('.cc-view-btn').removeClass('active');
            $(this).addClass('active');
            render();
        });

        // Búsqueda
        var searchTimer;
        $('#cc-search').on('input', function () {
            var v = this.value;
            clearTimeout(searchTimer);
            searchTimer = setTimeout(function () {
                state.search = v;
                render();
            }, 150);
        });

        // Reload
        $('#cc-reload').on('click', function () {
            fetchCatalog(state.activeKey).done(function () {
                render();
                notify('success', 'Datos actualizados.');
            });
        });

        // Nuevo
        $('#cc-new-btn').on('click', function () {
            var cat = currentCatalog();
            if (cat.editMode === 'redirect') {
                window.location.href = cat.editRedirectNew;
                return;
            }
            openEditModal(null);
        });

        // Card acciones y card "nueva"
        $(document).on('click', '.cc-card-new', function () {
            $('#cc-new-btn').trigger('click');
        });

        $(document).on('click', '.cc-card-action.view', function (e) {
            e.stopPropagation();
            var id = $(this).closest('[data-id]').data('id');
            openDetailModal(id);
        });

        $(document).on('click', '.cc-card-action.edit', function (e) {
            e.stopPropagation();
            var id = $(this).closest('[data-id]').data('id');
            var row = findRow(id);
            var cat = currentCatalog();
            if (cat.editMode === 'redirect') {
                // Ir directo a la pantalla dedicada
                window.location.href = cat.editRedirectEdit.replace('{id}', encodeURIComponent(id));
                return;
            }
            if (row) openEditModal(row);
        });

        $(document).on('click', '.cc-card-action.delete', function (e) {
            e.stopPropagation();
            var $host = $(this).closest('[data-id]');
            var id = $host.data('id');
            var row = findRow(id);
            var cat = currentCatalog();
            openDeleteModal(id, row ? row[cat.fieldDesc] : '');
        });

        // Form Edit
        $('#cc-edit-form').on('submit', function (e) { e.preventDefault(); submitEdit(); });
        $('#cc-edit-save').on('click', function (e) { e.preventDefault(); submitEdit(); });

        // Switch label
        $('#cc-edit-esactivo').on('change', function () {
            $('#cc-edit-esactivo-label').text(this.checked ? 'Activo' : 'Inactivo');
        });

        // Confirm Delete
        $('#cc-delete-confirm').on('click', function () { submitDelete(); });
    }

    // --------------------------------------------------
    // Init
    // --------------------------------------------------
    $(function () {
        bindEvents();
        fetchAll().always(function () { render(); });
    });

})();
