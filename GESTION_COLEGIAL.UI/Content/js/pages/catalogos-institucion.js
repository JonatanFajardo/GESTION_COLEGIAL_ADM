/* ============================================================
   Catálogos de Institución — Hub JS
   Consume los endpoints existentes de cada catálogo:
     /{Controller}/ListAsync, /DetailAsync, /CreateAsync,
     /DeleteAsync
   No añade lógica de negocio; solo orquesta UI.
   ============================================================ */
(function () {
    'use strict';

    // --------------------------------------------------
    // Configuración de los 5 catálogos (orden = cascada)
    // --------------------------------------------------
    var CATALOGS = {
        modalidades: {
            controller: 'Modalidades',
            singular: 'Modalidad',
            plural: 'Modalidades',
            article: 'la',
            genero: 'f',
            icon: 'bi-clock-history',
            hint: 'Formatos en los que se dicta la enseñanza (matutina, vespertina, nocturna).',
            placeholder: 'Ej: Matutina, Vespertina, Nocturna...',
            fieldId: 'Mda_Id',
            fieldDesc: 'Mda_Descripcion',
            fieldEsActivo: null,
            fieldUsuarioRegistra: 'Mda_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Mda_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Mda_FechaRegistra',
            fieldUsuarioModificaNombre: 'Mda_UsuarioModificaNombre',
            fieldFechaModifica: 'Mda_FechaModifica'
        },
        niveleseducativos: {
            controller: 'NivelesEducativos',
            singular: 'Nivel Educativo',
            plural: 'Niveles Educativos',
            article: 'el',
            genero: 'm',
            icon: 'bi-mortarboard',
            hint: 'Etapas académicas (Preescolar, Primaria, Secundaria, Bachillerato).',
            placeholder: 'Ej: Primaria, Secundaria, Bachillerato...',
            fieldId: 'Niv_Id',
            fieldDesc: 'Niv_Descripcion',
            fieldEsActivo: 'Niv_EsActivo',
            fieldUsuarioRegistra: 'Niv_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Niv_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Niv_FechaRegistra',
            fieldUsuarioModificaNombre: 'Niv_UsuarioModificaNombre',
            fieldFechaModifica: 'Niv_FechaModifica'
        },
        secciones: {
            controller: 'Secciones',
            singular: 'Sección',
            plural: 'Secciones',
            article: 'la',
            genero: 'f',
            icon: 'bi-columns-gap',
            hint: 'Subgrupos dentro de un grado (A, B, C, Mañana, Tarde).',
            placeholder: 'Ej: A, B, C, Mañana, Tarde...',
            fieldId: 'Sec_Id',
            fieldDesc: 'Sec_Descripcion',
            fieldEsActivo: null,
            fieldUsuarioRegistra: 'Sec_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Sec_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Sec_FechaRegistra',
            fieldUsuarioModificaNombre: 'Sec_UsuarioModificaNombre',
            fieldFechaModifica: 'Sec_FechaModifica'
        },
        semestres: {
            controller: 'Semestres',
            singular: 'Semestre',
            plural: 'Semestres',
            article: 'el',
            genero: 'm',
            icon: 'bi-calendar-range',
            hint: 'Períodos que dividen el año lectivo (I Semestre, II Semestre).',
            placeholder: 'Ej: I Semestre 2026, II Semestre 2026...',
            fieldId: 'Sem_Id',
            fieldDesc: 'Sem_Descripcion',
            fieldEsActivo: 'Sem_EsActivo',
            fieldUsuarioRegistra: 'Sem_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Sem_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Sem_FechaRegistra',
            fieldUsuarioModificaNombre: 'Sem_UsuarioModificaNombre',
            fieldFechaModifica: 'Sem_FechaModifica'
        },
        parciales: {
            controller: 'Parciales',
            singular: 'Parcial',
            plural: 'Parciales',
            article: 'el',
            genero: 'm',
            icon: 'bi-calendar2-week',
            hint: 'Sub-períodos de evaluación dentro de un semestre (I Parcial, II Parcial).',
            placeholder: 'Ej: I Parcial, II Parcial, Final...',
            fieldId: 'Pac_Id',
            fieldDesc: 'Pac_Descripcion',
            fieldEsActivo: null,
            fieldUsuarioRegistra: 'Pac_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Pac_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Pac_FechaRegistra',
            fieldUsuarioModificaNombre: 'Pac_UsuarioModificaNombre',
            fieldFechaModifica: 'Pac_FechaModifica'
        }
    };

    // --------------------------------------------------
    // Estado
    // --------------------------------------------------
    var state = {
        activeKey: 'modalidades',
        view: 'cards',                 // 'cards' | 'table'
        search: '',
        data: {}                       // { modalidades: [...], niveleseducativos: [...], ... }
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
            // Soporta "/Date(...)/" y ISO
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

    // Toast/Alert. Reutiliza toastr o swal si están disponibles.
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
        var $tab = $('.ci-tab-item[data-catalog="' + key + '"]');
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
        $('#ci-info-title').text(cat.plural);
        $('#ci-info-desc').text(cat.hint);
        $('#ci-new-label').text('Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular);
    }

    function renderCards() {
        var $grid = $('#ci-cards-grid').empty();
        var cat = currentCatalog();
        var rows = filterRows();

        if (rows.length === 0) {
            $('#ci-empty').prop('hidden', false);
            $('#ci-cards-grid').prop('hidden', true);
            return;
        }
        $('#ci-empty').prop('hidden', true);
        $('#ci-cards-grid').prop('hidden', false);

        rows.forEach(function (r) {
            var id = r[cat.fieldId];
            var desc = r[cat.fieldDesc] || '(sin descripción)';
            var esActivo = cat.fieldEsActivo ? !!r[cat.fieldEsActivo] : null;
            var badge = '';
            if (esActivo === true)  badge = '<span class="ci-card-badge active"><i class="bi bi-check-circle-fill"></i> Activo</span>';
            if (esActivo === false) badge = '<span class="ci-card-badge inactive"><i class="bi bi-slash-circle-fill"></i> Inactivo</span>';

            var $card = $(
                '<div class="ci-card ' + (esActivo === false ? 'ci-card-inactive' : '') + '" data-id="' + esc(id) + '">' +
                    '<div class="ci-card-head">' +
                        '<div class="ci-card-icon"><i class="bi ' + cat.icon + '"></i></div>' +
                        '<span class="ci-card-id">#' + esc(id) + '</span>' +
                    '</div>' +
                    '<div class="ci-card-body">' +
                        '<h4>' + esc(desc) + '</h4>' +
                        '<div class="ci-card-meta">' + badge + '</div>' +
                    '</div>' +
                    '<div class="ci-card-actions">' +
                        '<button type="button" class="ci-card-action view"   title="Ver detalle"><i class="bi bi-eye"></i></button>' +
                        '<button type="button" class="ci-card-action edit"   title="Editar"><i class="bi bi-pencil"></i></button>' +
                        '<button type="button" class="ci-card-action delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</div>' +
                '</div>'
            );
            $grid.append($card);
        });

        // Card "Nueva" al final
        $grid.append(
            '<div class="ci-card-new" role="button" tabindex="0">' +
                '<i class="bi bi-plus-circle"></i>' +
                '<span>Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular + '</span>' +
                '<small>Agregar un registro nuevo</small>' +
            '</div>'
        );
    }

    function renderTable() {
        var $body = $('#ci-table-body').empty();
        var cat = currentCatalog();
        var rows = filterRows();

        if (rows.length === 0) {
            $body.append(
                '<tr><td colspan="5" style="text-align:center; padding:2rem; color:#94a3b8;">' +
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
                ? '<span class="ci-card-badge active"><i class="bi bi-check-circle-fill"></i> Activo</span>'
                : esActivo === false
                ? '<span class="ci-card-badge inactive"><i class="bi bi-slash-circle-fill"></i> Inactivo</span>'
                : '<span style="color:#94a3b8;">—</span>';
            var fecha = fmtDate(r[cat.fieldFechaRegistra]);
            var usr = r[cat.fieldUsuarioRegistraNombre] || '';

            var $tr = $(
                '<tr data-id="' + esc(id) + '">' +
                    '<td><span class="badge-id">#' + esc(id) + '</span></td>' +
                    '<td><strong>' + esc(desc) + '</strong></td>' +
                    '<td>' + estadoCell + '</td>' +
                    '<td>' +
                        '<div style="font-size:0.8rem; color:#64748b;">' + esc(fecha) + '</div>' +
                        (usr ? '<div style="font-size:0.75rem; color:#94a3b8;">por ' + esc(usr) + '</div>' : '') +
                    '</td>' +
                    '<td class="actions-cell">' +
                        '<button type="button" class="ci-card-action view"   title="Detalle"><i class="bi bi-eye"></i></button> ' +
                        '<button type="button" class="ci-card-action edit"   title="Editar"><i class="bi bi-pencil"></i></button> ' +
                        '<button type="button" class="ci-card-action delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</td>' +
                '</tr>'
            );
            $body.append($tr);
        });
    }

    function render() {
        renderCatalogInfo();
        if (state.view === 'cards') {
            $('#ci-cards-view').prop('hidden', false);
            $('#ci-table-view').prop('hidden', true);
            renderCards();
        } else {
            $('#ci-cards-view').prop('hidden', true);
            $('#ci-table-view').prop('hidden', false);
            renderTable();
        }
    }

    // --------------------------------------------------
    // Modal: Crear / Editar
    // --------------------------------------------------
    function openEditModal(row) {
        var cat = currentCatalog();
        var isNew = !row;
        $('#ci-edit-id').val(isNew ? 0 : row[cat.fieldId]);
        $('#ci-edit-descripcion')
            .val(isNew ? '' : (row[cat.fieldDesc] || ''))
            .attr('placeholder', cat.placeholder);
        $('#ci-edit-hint').text(cat.hint);

        $('#ci-edit-title').text((isNew ? 'Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' : 'Editar ') + cat.singular);
        $('#ci-edit-subtitle').text((isNew ? 'Complete los datos para crear un nuevo registro en el catálogo ' : 'Modifique los datos del registro en el catálogo ') + cat.plural.toLowerCase());

        // Toggle estado (sólo para NivelesEducativos y Semestres)
        if (cat.fieldEsActivo) {
            $('#ci-edit-estado-wrap').prop('hidden', false);
            var activo = isNew ? true : !!row[cat.fieldEsActivo];
            $('#ci-edit-esactivo').prop('checked', activo);
            $('#ci-edit-esactivo-label').text(activo ? 'Activo' : 'Inactivo');
        } else {
            $('#ci-edit-estado-wrap').prop('hidden', true);
        }

        $('#ci-edit-modal').modal('show');
        setTimeout(function () { $('#ci-edit-descripcion').trigger('focus'); }, 250);
    }

    function submitEdit() {
        var cat = currentCatalog();
        var id = parseInt($('#ci-edit-id').val(), 10) || 0;
        var desc = $('#ci-edit-descripcion').val().trim();

        if (!desc) {
            notify('warning', 'La descripción es requerida.');
            $('#ci-edit-descripcion').trigger('focus');
            return;
        }

        var payload = {};
        payload[cat.fieldId] = id;
        payload[cat.fieldDesc] = desc;
        if (cat.fieldEsActivo) {
            payload[cat.fieldEsActivo] = $('#ci-edit-esactivo').prop('checked');
        }

        var $btn = $('#ci-edit-save').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Guardando...');

        $.ajax({
            url: url(cat, 'CreateAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            // Backend usa boolean invertido: item=true significa éxito
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', (id === 0 ? 'Registro creado.' : 'Registro actualizado.'));
                $('#ci-edit-modal').modal('hide');
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
        $('#ci-detail-catalog-name').text('Catálogo · ' + cat.plural);
        $('#ci-detail-id').text('#' + id);
        $('#ci-detail-descripcion').text('Cargando...');
        $('#ci-detail-usuarioregistra').text('—');
        $('#ci-detail-fecharegistra').text('');
        $('#ci-detail-usuariomodifica').text('—');
        $('#ci-detail-fechamodifica').text('');
        $('#ci-detail-modal').modal('show');

        $.ajax({
            url: url(cat, 'DetailAsync'),
            type: 'GET',
            data: { id: id },
            dataType: 'json'
        }).done(function (resp) {
            var item = (resp && resp.item) ? resp.item : (resp && resp.data) ? resp.data : resp;
            if (!item) {
                $('#ci-detail-descripcion').text('No se encontraron datos.');
                return;
            }
            $('#ci-detail-descripcion').text(item[cat.fieldDesc] || '—');
            $('#ci-detail-usuarioregistra').text(item[cat.fieldUsuarioRegistraNombre] || '—');
            $('#ci-detail-fecharegistra').text(fmtDate(item[cat.fieldFechaRegistra]));
            $('#ci-detail-usuariomodifica').text(item[cat.fieldUsuarioModificaNombre] || '—');
            $('#ci-detail-fechamodifica').text(fmtDate(item[cat.fieldFechaModifica]));
        }).fail(function () {
            $('#ci-detail-descripcion').text('Error al cargar detalle.');
        });
    }

    // --------------------------------------------------
    // Modal: Eliminar
    // --------------------------------------------------
    function openDeleteModal(id, desc) {
        var cat = currentCatalog();
        $('#ci-delete-id').val(id);
        $('#ci-delete-name').text('"' + (desc || 'este registro') + '"');
        $('#ci-delete-catalog').text(cat.plural);
        $('#ci-delete-modal').modal('show');
    }

    function submitDelete() {
        var cat = currentCatalog();
        var id = parseInt($('#ci-delete-id').val(), 10) || 0;
        if (!id) return;

        var payload = {};
        payload[cat.fieldId] = id;

        var $btn = $('#ci-delete-confirm').prop('disabled', true).text('Eliminando...');

        $.ajax({
            url: url(cat, 'DeleteAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', 'Registro eliminado.');
                $('#ci-delete-modal').modal('hide');
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
        $(document).on('click', '.ci-tab-item', function (e) {
            e.preventDefault();
            var key = $(this).data('catalog');
            if (!key || key === state.activeKey) return;
            $('.ci-tab-item').removeClass('active');
            $(this).addClass('active');
            state.activeKey = key;
            state.search = '';
            $('#ci-search').val('');
            render();
        });

        // Cascada pills en el hero
        $(document).on('click', '.ci-cascade-pill', function () {
            var key = $(this).data('jump');
            $('.ci-tab-item[data-catalog="' + key + '"]').trigger('click');
            $('html, body').animate({ scrollTop: $('.ci-tabs-card').offset().top - 20 }, 300);
        });

        // Toggle vista
        $(document).on('click', '.ci-view-btn', function () {
            var v = $(this).data('view');
            if (v === state.view) return;
            state.view = v;
            $('.ci-view-btn').removeClass('active');
            $(this).addClass('active');
            render();
        });

        // Búsqueda
        var searchTimer;
        $('#ci-search').on('input', function () {
            var v = this.value;
            clearTimeout(searchTimer);
            searchTimer = setTimeout(function () {
                state.search = v;
                render();
            }, 150);
        });

        // Reload
        $('#ci-reload').on('click', function () {
            $(this).find('i').addClass('bi-arrow-clockwise').css('animation', 'spin 0.6s linear');
            fetchCatalog(state.activeKey).done(function () {
                render();
                notify('success', 'Datos actualizados.');
            });
        });

        // Nuevo
        $('#ci-new-btn').on('click', function () { openEditModal(null); });

        // Card acciones y card "nueva"
        $(document).on('click', '.ci-card-new', function () { openEditModal(null); });

        $(document).on('click', '.ci-card, .ci-table tbody tr', function (e) {
            // Los clicks en botones se manejan aparte
            if ($(e.target).closest('.ci-card-action').length) return;
        });

        $(document).on('click', '.ci-card-action.view', function (e) {
            e.stopPropagation();
            var id = $(this).closest('[data-id]').data('id');
            openDetailModal(id);
        });

        $(document).on('click', '.ci-card-action.edit', function (e) {
            e.stopPropagation();
            var id = $(this).closest('[data-id]').data('id');
            var row = findRow(id);
            if (row) openEditModal(row);
        });

        $(document).on('click', '.ci-card-action.delete', function (e) {
            e.stopPropagation();
            var $host = $(this).closest('[data-id]');
            var id = $host.data('id');
            var row = findRow(id);
            var cat = currentCatalog();
            openDeleteModal(id, row ? row[cat.fieldDesc] : '');
        });

        // Form Edit
        $('#ci-edit-form').on('submit', function (e) { e.preventDefault(); submitEdit(); });
        $('#ci-edit-save').on('click', function (e) { e.preventDefault(); submitEdit(); });

        // Switch label
        $('#ci-edit-esactivo').on('change', function () {
            $('#ci-edit-esactivo-label').text(this.checked ? 'Activo' : 'Inactivo');
        });

        // Confirm Delete
        $('#ci-delete-confirm').on('click', function () { submitDelete(); });
    }

    // --------------------------------------------------
    // Init
    // --------------------------------------------------
    $(function () {
        bindEvents();
        fetchAll().always(function () { render(); });
    });

})();
