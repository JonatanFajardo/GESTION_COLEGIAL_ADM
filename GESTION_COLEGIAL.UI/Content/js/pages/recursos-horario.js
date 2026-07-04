/* ============================================================
   Recursos de Horario — Hub JS (accordion, no tabs)
   3 secciones: Aulas, Días, Horas.
   Cada sección se expande/colapsa de forma INDEPENDIENTE (no
   accordion tipo Bootstrap): el usuario puede tener las 3
   abiertas al mismo tiempo si quiere.

   Consume los endpoints existentes de cada catálogo:
     /{Controller}/ListAsync, /FindAsync, /CreateAsync, /DeleteAsync
   No añade lógica de negocio; solo orquesta UI.
   ============================================================ */
(function () {
    'use strict';

    // --------------------------------------------------
    // Configuración de los 3 recursos
    // --------------------------------------------------
    var CATALOGS = {
        aulas: {
            controller: 'Aulas',
            singular: 'Aula',
            plural: 'Aulas',
            article: 'la',
            genero: 'f',
            icon: 'bi-door-open',
            placeholder: 'Ej: Aula 101, Laboratorio Química...',
            descLabel: 'Descripción',
            descHint: 'Nombre o código con el que se identifica el aula.',
            hasHora: false,
            fieldId: 'Aul_Id',
            fieldDesc: 'Aul_Descripcion',
            fieldUsuarioRegistraNombre: 'Aul_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Aul_FechaRegistra',
            fieldUsuarioModificaNombre: 'Aul_UsuarioModificaNombre',
            fieldFechaModifica: 'Aul_FechaModifica'
        },
        dias: {
            controller: 'Dias',
            singular: 'Día',
            plural: 'Días',
            article: 'el',
            genero: 'm',
            icon: 'bi-calendar3',
            placeholder: 'Ej: Lunes, Martes, Miércoles...',
            descLabel: 'Descripción',
            descHint: 'Nombre del día laborable.',
            hasHora: false,
            fieldId: 'Dia_Id',
            fieldDesc: 'Dia_Descripcion',
            fieldUsuarioRegistraNombre: 'Dia_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Dia_FechaRegistra',
            fieldUsuarioModificaNombre: 'Dia_UsuarioModificaNombre',
            fieldFechaModifica: 'Dia_FechaModifica'
        },
        horas: {
            controller: 'Horas',
            singular: 'Hora',
            plural: 'Horas',
            article: 'la',
            genero: 'f',
            icon: 'bi-clock',
            placeholder: '07:00',
            descLabel: 'Hora',
            descHint: 'Bloque horario (HH:MM) que aparecerá como celda del horario.',
            hasHora: true,
            fieldId: 'Hor_Id',
            fieldDesc: 'Hor_Hora',
            fieldUsuarioRegistraNombre: 'Hor_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Hor_FechaRegistra',
            fieldUsuarioModificaNombre: 'Hor_UsuarioModificaNombre',
            fieldFechaModifica: 'Hor_FechaModifica'
        }
    };

    // Estado por sección
    var state = {
        activeEditKey: null,        // catálogo del modal Edit activo
        activeDeleteKey: null,      // catálogo del modal Delete activo
        search: { aulas: '', dias: '', horas: '' },
        data:   { aulas: [], dias: [], horas: [] }
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
            updateSectionCount(key, rows.length);
            return rows;
        }).catch(function () {
            state.data[key] = [];
            updateSectionCount(key, 0);
            return [];
        });
    }

    function fetchAll() {
        return $.when(
            fetchCatalog('aulas'),
            fetchCatalog('dias'),
            fetchCatalog('horas')
        );
    }

    // --------------------------------------------------
    // Rendering
    // --------------------------------------------------
    function updateSectionCount(key, count) {
        $('.rh-section[data-catalog="' + key + '"] [data-count]').text(count);
    }

    function filterRows(key) {
        var cat = CATALOGS[key];
        var rows = state.data[key] || [];
        var q = (state.search[key] || '').trim().toLowerCase();
        if (!q) return rows;
        return rows.filter(function (r) {
            var d = (r[cat.fieldDesc] || '').toString().toLowerCase();
            var id = (r[cat.fieldId] || '').toString().toLowerCase();
            return d.indexOf(q) >= 0 || id.indexOf(q) >= 0;
        });
    }

    function renderSection(key) {
        var cat = CATALOGS[key];
        var $grid = $('[data-grid="' + key + '"]').empty();
        var $empty = $('[data-empty="' + key + '"]');
        var rows = filterRows(key);

        if (rows.length === 0) {
            $empty.prop('hidden', false);
            $grid.prop('hidden', true);
            return;
        }
        $empty.prop('hidden', true);
        $grid.prop('hidden', false);

        rows.forEach(function (r) {
            var id = r[cat.fieldId];
            var desc = r[cat.fieldDesc] || '(sin descripción)';

            var $card = $(
                '<div class="rh-card" data-id="' + esc(id) + '" data-catalog="' + key + '">' +
                    '<div class="rh-card-head">' +
                        '<div class="rh-card-icon rh-icon-' + key + '"><i class="bi ' + cat.icon + '"></i></div>' +
                        '<span class="rh-card-id">#' + esc(id) + '</span>' +
                    '</div>' +
                    '<div class="rh-card-body">' +
                        '<h4>' + esc(desc) + '</h4>' +
                    '</div>' +
                    '<div class="rh-card-actions">' +
                        '<button type="button" class="rh-card-action view"   title="Ver detalle"><i class="bi bi-eye"></i></button>' +
                        '<button type="button" class="rh-card-action edit"   title="Editar"><i class="bi bi-pencil"></i></button>' +
                        '<button type="button" class="rh-card-action delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</div>' +
                '</div>'
            );
            $grid.append($card);
        });

        // Card "Nueva" al final
        $grid.append(
            '<div class="rh-card-new" data-new="' + key + '" role="button" tabindex="0">' +
                '<i class="bi bi-plus-circle"></i>' +
                '<span>Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' + cat.singular + '</span>' +
                '<small>Agregar un recurso nuevo</small>' +
            '</div>'
        );
    }

    function renderAll() {
        renderSection('aulas');
        renderSection('dias');
        renderSection('horas');
    }

    // --------------------------------------------------
    // Accordion (multi-open independiente)
    // --------------------------------------------------
    function toggleSection($section) {
        var isOpen = $section.hasClass('open');
        $section.toggleClass('open', !isOpen);
        $section.find('> .rh-section-header').attr('aria-expanded', !isOpen);
    }

    // --------------------------------------------------
    // Modal: Crear / Editar
    // --------------------------------------------------
    function openEditModal(key, row) {
        var cat = CATALOGS[key];
        state.activeEditKey = key;
        var isNew = !row;

        $('#rh-edit-id').val(isNew ? 0 : row[cat.fieldId]);
        $('#rh-edit-desc-label').text(cat.descLabel);
        $('#rh-edit-desc-hint').text(cat.descHint);

        if (cat.hasHora) {
            // Horas: usa <input type="time">
            $('#rh-edit-desc-wrap').prop('hidden', true);
            $('#rh-edit-hora-wrap').prop('hidden', false);
            var hVal = isNew ? '' : (row[cat.fieldDesc] || '');
            // Normaliza a HH:mm si viene con formato de hora
            var normalized = normalizeToTimeInput(hVal);
            $('#rh-edit-hora').val(normalized);
        } else {
            $('#rh-edit-desc-wrap').prop('hidden', false);
            $('#rh-edit-hora-wrap').prop('hidden', true);
            $('#rh-edit-descripcion')
                .val(isNew ? '' : (row[cat.fieldDesc] || ''))
                .attr('placeholder', cat.placeholder);
        }

        $('#rh-edit-title').text((isNew ? 'Nuev' + (cat.genero === 'f' ? 'a' : 'o') + ' ' : 'Editar ') + cat.singular);
        $('#rh-edit-subtitle').text((isNew ? 'Complete los datos para crear un nuevo recurso' : 'Modifique los datos del recurso') + ' — ' + cat.plural.toLowerCase());

        $('#rh-edit-modal').modal('show');
        setTimeout(function () {
            if (cat.hasHora) {
                $('#rh-edit-hora').trigger('focus');
            } else {
                $('#rh-edit-descripcion').trigger('focus');
            }
        }, 250);
    }

    function normalizeToTimeInput(v) {
        if (!v) return '';
        var s = String(v).trim();
        // Ya viene HH:MM o HH:MM:SS
        var m = /^(\d{1,2}):(\d{2})/.exec(s);
        if (m) {
            var h = parseInt(m[1], 10);
            var mi = parseInt(m[2], 10);
            var pad = function (n) { return n < 10 ? '0' + n : n; };
            return pad(h) + ':' + pad(mi);
        }
        return '';
    }

    function submitEdit() {
        var key = state.activeEditKey;
        if (!key) return;
        var cat = CATALOGS[key];
        var id = parseInt($('#rh-edit-id').val(), 10) || 0;
        var value;

        if (cat.hasHora) {
            value = ($('#rh-edit-hora').val() || '').trim();
            if (!value) {
                notify('warning', 'La hora es requerida.');
                $('#rh-edit-hora').trigger('focus');
                return;
            }
        } else {
            value = ($('#rh-edit-descripcion').val() || '').trim();
            if (!value) {
                notify('warning', 'La descripción es requerida.');
                $('#rh-edit-descripcion').trigger('focus');
                return;
            }
        }

        var payload = {};
        payload[cat.fieldId] = id;
        payload[cat.fieldDesc] = value;

        var $btn = $('#rh-edit-save').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Guardando...');

        $.ajax({
            url: url(cat, 'CreateAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', (id === 0 ? 'Recurso creado.' : 'Recurso actualizado.'));
                $('#rh-edit-modal').modal('hide');
                fetchCatalog(key).done(function () { renderSection(key); });
            } else {
                notify('error', (resp && resp.message) || 'No se pudo guardar el recurso.');
            }
        }).fail(function () {
            notify('error', 'Error de conexión al guardar.');
        }).always(function () {
            $btn.prop('disabled', false).html('<i class="bi bi-check2"></i> Guardar');
        });
    }

    // --------------------------------------------------
    // Modal: Detalle (usa FindAsync — no hay DetailAsync)
    // --------------------------------------------------
    function openDetailModal(key, id) {
        var cat = CATALOGS[key];
        $('#rh-detail-catalog-name').text('Recurso · ' + cat.plural);
        $('#rh-detail-id').text('#' + id);
        $('#rh-detail-descripcion-label').text(cat.descLabel);
        $('#rh-detail-descripcion').text('Cargando...');
        $('#rh-detail-usuarioregistra').text('—');
        $('#rh-detail-fecharegistra').text('');
        $('#rh-detail-usuariomodifica').text('—');
        $('#rh-detail-fechamodifica').text('');
        $('#rh-detail-modal').modal('show');

        $.ajax({
            url: url(cat, 'FindAsync'),
            type: 'GET',
            data: { id: id },
            dataType: 'json'
        }).done(function (resp) {
            var item = (resp && resp.item) ? resp.item : (resp && resp.data) ? resp.data : resp;
            if (!item) {
                $('#rh-detail-descripcion').text('No se encontraron datos.');
                return;
            }
            $('#rh-detail-descripcion').text(item[cat.fieldDesc] || '—');
            $('#rh-detail-usuarioregistra').text(item[cat.fieldUsuarioRegistraNombre] || '—');
            $('#rh-detail-fecharegistra').text(fmtDate(item[cat.fieldFechaRegistra]));
            $('#rh-detail-usuariomodifica').text(item[cat.fieldUsuarioModificaNombre] || '—');
            $('#rh-detail-fechamodifica').text(fmtDate(item[cat.fieldFechaModifica]));
        }).fail(function () {
            $('#rh-detail-descripcion').text('Error al cargar detalle.');
        });
    }

    // --------------------------------------------------
    // Modal: Eliminar
    // --------------------------------------------------
    function openDeleteModal(key, id, desc) {
        var cat = CATALOGS[key];
        state.activeDeleteKey = key;
        $('#rh-delete-id').val(id);
        $('#rh-delete-name').text('"' + (desc || 'este recurso') + '"');
        $('#rh-delete-catalog').text(cat.plural);
        $('#rh-delete-modal').modal('show');
    }

    function submitDelete() {
        var key = state.activeDeleteKey;
        if (!key) return;
        var cat = CATALOGS[key];
        var id = parseInt($('#rh-delete-id').val(), 10) || 0;
        if (!id) return;

        var payload = {};
        payload[cat.fieldId] = id;

        var $btn = $('#rh-delete-confirm').prop('disabled', true).text('Eliminando...');

        $.ajax({
            url: url(cat, 'DeleteAsync'),
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', 'Recurso eliminado.');
                $('#rh-delete-modal').modal('hide');
                fetchCatalog(key).done(function () { renderSection(key); });
            } else {
                notify('error', (resp && resp.message) || 'No se pudo eliminar. El recurso puede estar en uso.');
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
    function findRow(key, id) {
        var cat = CATALOGS[key];
        var rows = state.data[key] || [];
        for (var i = 0; i < rows.length; i++) {
            if (String(rows[i][cat.fieldId]) === String(id)) return rows[i];
        }
        return null;
    }

    function catalogOfEl($el) {
        return $el.closest('[data-catalog]').data('catalog');
    }

    // --------------------------------------------------
    // Event bindings
    // --------------------------------------------------
    function bindEvents() {
        // Accordion: toggle independiente por sección
        $(document).on('click', '.rh-section-header', function (e) {
            // Evitar toggle si clickearon un botón dentro del header (no hay,
            // pero por seguridad)
            if ($(e.target).closest('button, input').length) return;
            var $section = $(this).closest('.rh-section');
            toggleSection($section);
        });

        $(document).on('keydown', '.rh-section-header', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleSection($(this).closest('.rh-section'));
            }
        });

        // Chips del hero → abre + scroll a la sección
        $(document).on('click', '.rh-hero-chip', function () {
            var key = $(this).data('jump');
            var $section = $('.rh-section[data-catalog="' + key + '"]');
            if (!$section.hasClass('open')) toggleSection($section);
            $('html, body').animate({ scrollTop: $section.offset().top - 20 }, 300);
        });

        // Búsqueda por sección
        var searchTimers = {};
        $(document).on('input', '.rh-search', function () {
            var key = $(this).data('search');
            var v = this.value;
            clearTimeout(searchTimers[key]);
            searchTimers[key] = setTimeout(function () {
                state.search[key] = v;
                renderSection(key);
            }, 150);
        });

        // Reload por sección
        $(document).on('click', '[data-reload]', function () {
            var key = $(this).data('reload');
            var $icon = $(this).find('i');
            $icon.css('animation', 'rh-spin 0.6s linear');
            fetchCatalog(key).done(function () {
                renderSection(key);
                notify('success', 'Datos actualizados.');
            }).always(function () {
                setTimeout(function () { $icon.css('animation', ''); }, 700);
            });
        });

        // Nuevo (botones toolbar + card "nueva" + empty state)
        $(document).on('click', '[data-new]', function () {
            var key = $(this).data('new');
            openEditModal(key, null);
        });

        // Acciones sobre cards
        $(document).on('click', '.rh-card-action.view', function (e) {
            e.stopPropagation();
            var $card = $(this).closest('.rh-card');
            var key = $card.data('catalog');
            var id = $card.data('id');
            openDetailModal(key, id);
        });

        $(document).on('click', '.rh-card-action.edit', function (e) {
            e.stopPropagation();
            var $card = $(this).closest('.rh-card');
            var key = $card.data('catalog');
            var id = $card.data('id');
            var row = findRow(key, id);
            if (row) openEditModal(key, row);
        });

        $(document).on('click', '.rh-card-action.delete', function (e) {
            e.stopPropagation();
            var $card = $(this).closest('.rh-card');
            var key = $card.data('catalog');
            var id = $card.data('id');
            var row = findRow(key, id);
            var cat = CATALOGS[key];
            openDeleteModal(key, id, row ? row[cat.fieldDesc] : '');
        });

        // Form Edit
        $('#rh-edit-form').on('submit', function (e) { e.preventDefault(); submitEdit(); });
        $('#rh-edit-save').on('click', function (e) { e.preventDefault(); submitEdit(); });

        // Confirm Delete
        $('#rh-delete-confirm').on('click', function () { submitDelete(); });
    }

    // --------------------------------------------------
    // Init
    // --------------------------------------------------
    $(function () {
        bindEvents();
        fetchAll().always(function () { renderAll(); });
    });

})();
