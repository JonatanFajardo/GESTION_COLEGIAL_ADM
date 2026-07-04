/* ============================================================
   Programa Académico — Hub JS
   Consolida 4 catálogos: Grados Académicos, Cursos, Materias, Duraciones.
   Reusa endpoints existentes de cada controlador:
     /{Controller}/ListAsync, /DetailAsync, /CreateAsync, /DeleteAsync
   No añade lógica de negocio; solo orquesta UI.

   Estrategia por catálogo:
   - gradosacademicos (CursosNiveles):
       Simple + FK a NivelEducativo → modal con Descripcion + dropdown de Nivel.
   - cursos:
       Complejo (5+ FKs / multi-selects) → modal solo muestra aviso y
       botón "Ver formulario completo" que redirige a /Cursos.
   - materias:
       Complejo (FK a Duración + EsActivo) → igual que cursos, redirige a /Materias.
   - duraciones:
       Simple puro → Id + Descripcion.
   ============================================================ */
(function () {
    'use strict';

    // --------------------------------------------------
    // Configuración de los 4 catálogos (orden = cascada)
    // --------------------------------------------------
    var CATALOGS = {
        gradosacademicos: {
            controller: 'CursosNiveles',
            singular: 'Grado Académico',
            plural: 'Grados Académicos',
            article: 'el',
            genero: 'm',
            icon: 'bi-mortarboard-fill',
            hint: 'Grados que componen cada nivel educativo (1° a 6° de Primaria, 1° a 3° de Bachillerato).',
            placeholder: 'Ej: 1° Primaria, 6° Primaria, 3° Bachillerato...',
            fieldId: 'Cun_Id',
            fieldDesc: 'Cun_Descripcion',
            fieldEsActivo: null,
            fieldUsuarioRegistra: 'Cun_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Cun_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Cun_FechaRegistra',
            fieldUsuarioModificaNombre: 'Cun_UsuarioModificaNombre',
            fieldFechaModifica: 'Cun_FechaModifica',
            // Extras específicos de este catálogo
            complex: false,
            requiresNivel: true,
            fieldNivelId: 'Niv_Id',
            fieldNivelDesc: 'Niv_Descripcion',
            descLabel: 'Descripción del Grado'
        },
        cursos: {
            controller: 'Cursos',
            singular: 'Curso',
            plural: 'Cursos',
            article: 'el',
            genero: 'm',
            icon: 'bi-journal-text',
            hint: 'Cursos ofertados en el año lectivo (relaciona materia, nivel, sección y modalidad).',
            placeholder: 'Ej: Matemáticas 1° A Presencial...',
            fieldId: 'Cur_Id',
            fieldDesc: 'Cur_Nombre',
            fieldEsActivo: 'Cur_EsActivo',
            fieldUsuarioRegistra: 'Cur_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Cur_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Cur_FechaRegistra',
            fieldUsuarioModificaNombre: 'Cur_UsuarioModificaNombre',
            fieldFechaModifica: 'Cur_FechaModifica',
            complex: true,
            fullFormUrl: '/Cursos',
            complexReason: 'Un curso requiere seleccionar Materia, Nivel Educativo, Sección y Modalidad. Para gestionar esas relaciones abra el formulario completo del módulo Cursos.'
        },
        materias: {
            controller: 'Materias',
            singular: 'Materia',
            plural: 'Materias',
            article: 'la',
            genero: 'f',
            icon: 'bi-book-half',
            hint: 'Asignaturas que se enseñan en los cursos (Matemáticas, Español, Ciencias, etc.).',
            placeholder: 'Ej: Matemáticas, Español, Ciencias Naturales...',
            fieldId: 'Mat_Id',
            fieldDesc: 'Mat_Nombre',
            fieldEsActivo: 'Mat_EsActivo',
            fieldUsuarioRegistra: 'Mat_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Mat_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Mat_FechaRegistra',
            fieldUsuarioModificaNombre: 'Mat_UsuarioModificaNombre',
            fieldFechaModifica: 'Mat_FechaModifica',
            complex: true,
            fullFormUrl: '/Materias',
            complexReason: 'Una materia depende de una Duración y controla el estado Activo. Abra el formulario completo del módulo Materias para asignarla correctamente.'
        },
        duraciones: {
            controller: 'Duraciones',
            singular: 'Duración',
            plural: 'Duraciones',
            article: 'la',
            genero: 'f',
            icon: 'bi-hourglass-split',
            hint: 'Períodos de tiempo en que se dicta una materia (Anual, Semestral, Trimestral).',
            placeholder: 'Ej: Anual, Semestral, Trimestral...',
            fieldId: 'Dur_Id',
            fieldDesc: 'Dur_Descripcion',
            fieldEsActivo: null,
            fieldUsuarioRegistra: 'Dur_UsuarioRegistra',
            fieldUsuarioRegistraNombre: 'Dur_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Dur_FechaRegistra',
            fieldUsuarioModificaNombre: 'Dur_UsuarioModificaNombre',
            fieldFechaModifica: 'Dur_FechaModifica',
            complex: false
        }
    };

    // --------------------------------------------------
    // Estado
    // --------------------------------------------------
    var state = {
        activeKey: 'gradosacademicos',
        view: 'cards',
        search: '',
        data: {},          // { key: [rows] }
        niveles: []        // catálogo auxiliar de niveles educativos (para dropdown de Grados)
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
                Swal.fire({
                    icon: type === 'success' ? 'success' : type === 'error' ? 'error' : 'info',
                    title: msg, toast: true, position: 'top-end',
                    showConfirmButton: false, timer: 2600
                });
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

    function fetchNiveles() {
        return $.ajax({
            url: '/NivelesEducativos/ListAsync',
            type: 'GET',
            dataType: 'json'
        }).then(function (resp) {
            var rows = Array.isArray(resp) ? resp
                       : (resp && resp.data) ? resp.data
                       : [];
            state.niveles = rows;
            return rows;
        }).catch(function () {
            state.niveles = [];
            return [];
        });
    }

    function fetchAll() {
        var keys = Object.keys(CATALOGS);
        var jobs = keys.map(fetchCatalog);
        jobs.push(fetchNiveles());
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

            // Meta extra: nivel educativo del grado
            var extraMeta = '';
            if (cat.requiresNivel && r[cat.fieldNivelDesc]) {
                extraMeta = '<span class="ci-card-badge" style="background:#eef2ff;color:#4338ca;border:1px solid #c7d2fe;">' +
                            '<i class="bi bi-diagram-2"></i> ' + esc(r[cat.fieldNivelDesc]) + '</span>';
            }

            var $card = $(
                '<div class="ci-card ' + (esActivo === false ? 'ci-card-inactive' : '') + '" data-id="' + esc(id) + '">' +
                    '<div class="ci-card-head">' +
                        '<div class="ci-card-icon"><i class="bi ' + cat.icon + '"></i></div>' +
                        '<span class="ci-card-id">#' + esc(id) + '</span>' +
                    '</div>' +
                    '<div class="ci-card-body">' +
                        '<h4>' + esc(desc) + '</h4>' +
                        '<div class="ci-card-meta">' + extraMeta + badge + '</div>' +
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

        // Card "Nuevo/a" al final
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

            // Para Grados Académicos: mostrar el nivel como sufijo de la descripción
            var descCell = '<strong>' + esc(desc) + '</strong>';
            if (cat.requiresNivel && r[cat.fieldNivelDesc]) {
                descCell += '<div style="font-size:0.75rem;color:#4338ca;margin-top:2px;">' +
                            '<i class="bi bi-diagram-2"></i> ' + esc(r[cat.fieldNivelDesc]) + '</div>';
            }

            var $tr = $(
                '<tr data-id="' + esc(id) + '">' +
                    '<td><span class="badge-id">#' + esc(id) + '</span></td>' +
                    '<td>' + descCell + '</td>' +
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
    // Populate Nivel dropdown (para Grados Académicos)
    // --------------------------------------------------
    function populateNivelSelect(selectedId) {
        var $sel = $('#pa-nivel-select').empty();
        $sel.append('<option value="">— Seleccione un nivel —</option>');
        (state.niveles || []).forEach(function (n) {
            var id = n.Niv_Id;
            var desc = n.Niv_Descripcion || '';
            var opt = '<option value="' + esc(id) + '"' +
                      (String(id) === String(selectedId) ? ' selected' : '') + '>' +
                      esc(desc) + '</option>';
            $sel.append(opt);
        });
    }

    // --------------------------------------------------
    // Modal: Crear / Editar
    // --------------------------------------------------
    function openEditModal(row) {
        var cat = currentCatalog();
        var isNew = !row;

        // Reset visibilidades
        $('#pa-complex-notice').prop('hidden', true);
        $('#pa-desc-wrap').prop('hidden', false);
        $('#pa-nivel-wrap').prop('hidden', true);
        $('#ci-edit-estado-wrap').prop('hidden', true);
        $('#pa-open-full-form').prop('hidden', true);
        $('#ci-edit-save').prop('hidden', false);

        // Título
        $('#ci-edit-title').text((isNew ? 'Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' : 'Editar ') + cat.singular);
        $('#ci-edit-subtitle').text((isNew ? 'Complete los datos para crear un nuevo registro en el catálogo ' : 'Modifique los datos del registro en el catálogo ') + cat.plural.toLowerCase());

        // Caso "complejo" → mostrar aviso + botón "Ver formulario completo"
        if (cat.complex) {
            $('#pa-complex-notice').prop('hidden', false);
            $('#pa-complex-title').text('Este catálogo requiere el formulario completo');
            $('#pa-complex-desc').text(cat.complexReason);

            // Ocultar campos simples
            $('#pa-desc-wrap').prop('hidden', true);
            $('#ci-edit-estado-wrap').prop('hidden', true);
            $('#pa-nivel-wrap').prop('hidden', true);

            // Botón redirige a la pantalla completa
            var target = cat.fullFormUrl || ('/' + cat.controller);
            $('#pa-open-full-form')
                .prop('hidden', false)
                .attr('href', target);

            // Ocultar botón Guardar (no aplica)
            $('#ci-edit-save').prop('hidden', true);

            $('#ci-edit-id').val(isNew ? 0 : row[cat.fieldId]);
            $('#ci-edit-modal').modal('show');
            return;
        }

        // Caso simple (Grados o Duraciones)
        $('#ci-edit-id').val(isNew ? 0 : row[cat.fieldId]);
        $('#pa-desc-label').text(cat.descLabel || 'Descripción');
        $('#ci-edit-descripcion')
            .val(isNew ? '' : (row[cat.fieldDesc] || ''))
            .attr('placeholder', cat.placeholder);
        $('#ci-edit-hint').text(cat.hint);

        // Dropdown de nivel (solo para Grados Académicos)
        if (cat.requiresNivel) {
            $('#pa-nivel-wrap').prop('hidden', false);
            populateNivelSelect(isNew ? '' : row[cat.fieldNivelId]);
        }

        // Toggle estado (si el catálogo tiene EsActivo)
        if (cat.fieldEsActivo) {
            $('#ci-edit-estado-wrap').prop('hidden', false);
            var activo = isNew ? true : !!row[cat.fieldEsActivo];
            $('#ci-edit-esactivo').prop('checked', activo);
            $('#ci-edit-esactivo-label').text(activo ? 'Activo' : 'Inactivo');
        }

        $('#ci-edit-modal').modal('show');
        setTimeout(function () { $('#ci-edit-descripcion').trigger('focus'); }, 250);
    }

    function submitEdit() {
        var cat = currentCatalog();
        if (cat.complex) return; // no debería llegar aquí, el botón está oculto

        var id = parseInt($('#ci-edit-id').val(), 10) || 0;
        var desc = $('#ci-edit-descripcion').val().trim();

        if (!desc) {
            notify('warning', 'La descripción es requerida.');
            $('#ci-edit-descripcion').trigger('focus');
            return;
        }

        // Validación FK nivel para Grados Académicos
        var nivelId = 0;
        if (cat.requiresNivel) {
            nivelId = parseInt($('#pa-nivel-select').val(), 10) || 0;
            if (!nivelId) {
                notify('warning', 'Seleccione el nivel educativo.');
                $('#pa-nivel-select').trigger('focus');
                return;
            }
        }

        var payload = {};
        payload[cat.fieldId] = id;
        payload[cat.fieldDesc] = desc;
        if (cat.fieldEsActivo) {
            payload[cat.fieldEsActivo] = $('#ci-edit-esactivo').prop('checked');
        }
        if (cat.requiresNivel) {
            payload[cat.fieldNivelId] = nivelId;
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
            var descText = item[cat.fieldDesc] || '—';
            if (cat.requiresNivel && item[cat.fieldNivelDesc]) {
                descText += '  ·  ' + item[cat.fieldNivelDesc];
            }
            $('#ci-detail-descripcion').text(descText);
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
            fetchCatalog(state.activeKey).done(function () {
                render();
                notify('success', 'Datos actualizados.');
            });
        });

        // Nuevo
        $('#ci-new-btn').on('click', function () { openEditModal(null); });

        // Card "nueva"
        $(document).on('click', '.ci-card-new', function () { openEditModal(null); });

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
