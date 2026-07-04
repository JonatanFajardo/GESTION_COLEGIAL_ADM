/* ============================================================
   Catálogos Comunes — Hub single-table + chips
   Consume endpoints: /Estados/*, /Parentescos/*, /Titulos/*, /Cargos/*
   ============================================================ */
(function () {
    'use strict';

    var CATALOGS = {
        estados: {
            controller: 'Estados',
            singular: 'Estado',
            plural: 'Estados',
            article: 'el',
            genero: 'm',
            hint: 'Estados del sistema (activo, inactivo, retirado, suspendido, etc.).',
            fieldId: 'Est_Id',
            fieldDesc: 'Est_Descripcion',
            fieldUsuarioRegistraNombre: 'Est_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Est_FechaRegistra',
            fieldUsuarioModificaNombre: 'Est_UsuarioModificaNombre',
            fieldFechaModifica: 'Est_FechaModifica'
        },
        parentescos: {
            controller: 'Parentescos',
            singular: 'Parentesco',
            plural: 'Parentescos',
            article: 'el',
            genero: 'm',
            hint: 'Relaciones familiares entre alumno y encargado (padre, madre, tutor, tío, etc.).',
            fieldId: 'Par_Id',
            fieldDesc: 'Par_Descripcion',
            fieldUsuarioRegistraNombre: 'Par_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Par_FechaRegistra',
            fieldUsuarioModificaNombre: 'Par_UsuarioModificaNombre',
            fieldFechaModifica: 'Par_FechaModifica'
        },
        titulos: {
            controller: 'Titulos',
            singular: 'Título',
            plural: 'Títulos',
            article: 'el',
            genero: 'm',
            hint: 'Títulos académicos que puede tener un docente o empleado (Licenciado, Máster, PhD, etc.).',
            fieldId: 'Tit_Id',
            fieldDesc: 'Tit_Descripcion',
            fieldUsuarioRegistraNombre: 'Tit_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Tit_FechaRegistra',
            fieldUsuarioModificaNombre: 'Tit_UsuarioModificaNombre',
            fieldFechaModifica: 'Tit_FechaModifica'
        },
        cargos: {
            controller: 'Cargos',
            singular: 'Cargo',
            plural: 'Cargos',
            article: 'el',
            genero: 'm',
            hint: 'Cargos institucionales de los empleados (Director, Coordinador, Docente, Contador, etc.).',
            fieldId: 'Car_Id',
            fieldDesc: 'Car_Descripcion',
            fieldUsuarioRegistraNombre: 'Car_UsuarioRegistraNombre',
            fieldFechaRegistra: 'Car_FechaRegistra',
            fieldUsuarioModificaNombre: 'Car_UsuarioModificaNombre',
            fieldFechaModifica: 'Car_FechaModifica'
        }
    };

    var state = {
        activeTipo: 'todos',
        search: '',
        rows: []
    };

    // -------------------- Utils --------------------
    function esc(s) {
        if (s === null || s === undefined) return '';
        return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#39;');
    }

    function fmtDate(v) {
        if (!v) return '—';
        try {
            var m = /\/Date\((\d+)/.exec(v);
            var d = m ? new Date(parseInt(m[1], 10)) : new Date(v);
            if (isNaN(d.getTime())) return String(v);
            var pad = function (n) { return n < 10 ? '0' + n : n; };
            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear() + ' ' + pad(d.getHours()) + ':' + pad(d.getMinutes());
        } catch (e) { return String(v); }
    }

    function notify(type, msg) {
        try {
            if (window.toastr) { (toastr[type] || toastr.info)(msg); return; }
            if (window.Swal) { Swal.fire({ icon: type === 'success' ? 'success' : type === 'error' ? 'error' : 'info', title: msg, toast: true, position: 'top-end', showConfirmButton: false, timer: 2600 }); return; }
        } catch (e) {}
        console.log('[' + type + '] ' + msg);
    }

    // -------------------- Data --------------------
    function fetchCatalog(tipo) {
        var cat = CATALOGS[tipo];
        return $.ajax({
            url: '/' + cat.controller + '/ListAsync',
            type: 'GET',
            dataType: 'json'
        }).then(function (resp) {
            var rows = Array.isArray(resp) ? resp : (resp && resp.data) ? resp.data : [];
            // Normalizar cada fila con { _tipo, id, descripcion, fecha, usuario, _raw }
            return rows.map(function (r) {
                return {
                    _tipo: tipo,
                    id: r[cat.fieldId],
                    descripcion: r[cat.fieldDesc],
                    fecha: r[cat.fieldFechaRegistra],
                    usuario: r[cat.fieldUsuarioRegistraNombre],
                    _raw: r
                };
            });
        }).catch(function () { return []; });
    }

    function fetchAll() {
        var jobs = Object.keys(CATALOGS).map(fetchCatalog);
        return $.when.apply($, jobs).then(function () {
            var all = [];
            for (var i = 0; i < arguments.length; i++) {
                all = all.concat(arguments[i] || []);
            }
            state.rows = all;
            updateCounts();
            return all;
        });
    }

    function updateCounts() {
        var counts = { todos: state.rows.length, estados: 0, parentescos: 0, titulos: 0, cargos: 0 };
        state.rows.forEach(function (r) { counts[r._tipo] = (counts[r._tipo] || 0) + 1; });
        Object.keys(counts).forEach(function (k) {
            $('[data-count="' + k + '"]').text(counts[k]);
        });
        $('#cc-stat-total').text(state.rows.length);
    }

    // -------------------- Filter + render --------------------
    function filteredRows() {
        var rows = state.rows;
        if (state.activeTipo !== 'todos') {
            rows = rows.filter(function (r) { return r._tipo === state.activeTipo; });
        }
        var q = state.search.trim().toLowerCase();
        if (q) {
            rows = rows.filter(function (r) {
                var d = (r.descripcion || '').toString().toLowerCase();
                var id = String(r.id).toLowerCase();
                var t = r._tipo.toLowerCase();
                return d.indexOf(q) >= 0 || id.indexOf(q) >= 0 || t.indexOf(q) >= 0;
            });
        }
        return rows;
    }

    function render() {
        var rows = filteredRows();
        var $body = $('#cc-table-body').empty();

        if (rows.length === 0) {
            $('#cc-empty').prop('hidden', false);
            $('#cc-table').prop('hidden', true);
            return;
        }
        $('#cc-empty').prop('hidden', true);
        $('#cc-table').prop('hidden', false);

        rows.forEach(function (r) {
            var cat = CATALOGS[r._tipo];
            var $tr = $(
                '<tr data-id="' + esc(r.id) + '" data-tipo="' + esc(r._tipo) + '">' +
                    '<td><span class="cc-badge-tipo" data-type="' + esc(r._tipo) + '">' + esc(cat.plural) + '</span></td>' +
                    '<td><span class="cc-badge-id">#' + esc(r.id) + '</span></td>' +
                    '<td class="cc-desc-cell"><strong>' + esc(r.descripcion || '(sin descripción)') + '</strong></td>' +
                    '<td>' +
                        '<div class="cc-meta-line">' + esc(fmtDate(r.fecha)) + '</div>' +
                        (r.usuario ? '<div class="cc-meta-line"><small>por ' + esc(r.usuario) + '</small></div>' : '') +
                    '</td>' +
                    '<td class="cc-actions-cell">' +
                        '<button type="button" class="cc-action-btn view"   title="Detalle"><i class="bi bi-eye"></i></button>' +
                        '<button type="button" class="cc-action-btn edit"   title="Editar"><i class="bi bi-pencil"></i></button>' +
                        '<button type="button" class="cc-action-btn delete" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</td>' +
                '</tr>'
            );
            $body.append($tr);
        });
    }

    // -------------------- Modales --------------------
    function findRow(tipo, id) {
        for (var i = 0; i < state.rows.length; i++) {
            if (state.rows[i]._tipo === tipo && String(state.rows[i].id) === String(id)) return state.rows[i];
        }
        return null;
    }

    function openEdit(tipo, row) {
        var cat = CATALOGS[tipo];
        var isNew = !row;
        $('#cc-edit-tipo').val(tipo);
        $('#cc-edit-id').val(isNew ? 0 : row.id);
        $('#cc-edit-descripcion').val(isNew ? '' : (row.descripcion || ''));
        $('#cc-edit-hint').text(cat.hint);
        $('#cc-edit-title').text((isNew ? 'Nuev' + (cat.genero === 'f' ? 'a ' : 'o ') : 'Editar ') + cat.singular);
        $('#cc-edit-subtitle').text('Catálogo · ' + cat.plural);
        $('#cc-edit-modal').modal('show');
        setTimeout(function () { $('#cc-edit-descripcion').trigger('focus'); }, 250);
    }

    function submitEdit() {
        var tipo = $('#cc-edit-tipo').val();
        var cat = CATALOGS[tipo];
        var id = parseInt($('#cc-edit-id').val(), 10) || 0;
        var desc = $('#cc-edit-descripcion').val().trim();
        if (!desc) { notify('warning', 'La descripción es requerida.'); return; }

        var payload = {};
        payload[cat.fieldId] = id;
        payload[cat.fieldDesc] = desc;

        var $btn = $('#cc-edit-save').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Guardando...');

        $.ajax({
            url: '/' + cat.controller + '/CreateAsync',
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', id === 0 ? 'Registro creado.' : 'Registro actualizado.');
                $('#cc-edit-modal').modal('hide');
                fetchAll().done(render);
            } else {
                notify('error', (resp && resp.message) || 'No se pudo guardar.');
            }
        }).fail(function () {
            notify('error', 'Error de conexión al guardar.');
        }).always(function () {
            $btn.prop('disabled', false).html('<i class="bi bi-check2"></i> Guardar');
        });
    }

    function openDetail(tipo, id) {
        var cat = CATALOGS[tipo];
        $('#cc-detail-catalog-name').text('Catálogo · ' + cat.plural);
        $('#cc-detail-id').text('#' + id);
        $('#cc-detail-descripcion').text('Cargando...');
        $('#cc-detail-usuarioregistra').text('—');
        $('#cc-detail-fecharegistra').text('');
        $('#cc-detail-usuariomodifica').text('—');
        $('#cc-detail-fechamodifica').text('');
        $('#cc-detail-modal').modal('show');

        $.ajax({
            url: '/' + cat.controller + '/DetailAsync',
            type: 'GET',
            data: { id: id },
            dataType: 'json'
        }).done(function (resp) {
            var item = (resp && resp.item) ? resp.item : (resp && resp.data) ? resp.data : resp;
            if (!item) { $('#cc-detail-descripcion').text('Sin datos.'); return; }
            $('#cc-detail-descripcion').text(item[cat.fieldDesc] || '—');
            $('#cc-detail-usuarioregistra').text(item[cat.fieldUsuarioRegistraNombre] || '—');
            $('#cc-detail-fecharegistra').text(fmtDate(item[cat.fieldFechaRegistra]));
            $('#cc-detail-usuariomodifica').text(item[cat.fieldUsuarioModificaNombre] || '—');
            $('#cc-detail-fechamodifica').text(fmtDate(item[cat.fieldFechaModifica]));
        }).fail(function () {
            $('#cc-detail-descripcion').text('Error al cargar detalle.');
        });
    }

    function openDelete(tipo, id, desc) {
        var cat = CATALOGS[tipo];
        $('#cc-delete-id').val(id);
        $('#cc-delete-tipo').val(tipo);
        $('#cc-delete-name').text('"' + (desc || 'este registro') + '"');
        $('#cc-delete-catalog').text(cat.plural);
        $('#cc-delete-modal').modal('show');
    }

    function submitDelete() {
        var tipo = $('#cc-delete-tipo').val();
        var cat = CATALOGS[tipo];
        var id = parseInt($('#cc-delete-id').val(), 10) || 0;
        if (!id) return;
        var payload = {};
        payload[cat.fieldId] = id;

        var $btn = $('#cc-delete-confirm').prop('disabled', true).text('Eliminando...');

        $.ajax({
            url: '/' + cat.controller + '/DeleteAsync',
            type: 'POST',
            data: payload,
            dataType: 'json'
        }).done(function (resp) {
            var ok = resp && (resp.item === true || resp.success === true);
            if (ok) {
                notify('success', 'Registro eliminado.');
                $('#cc-delete-modal').modal('hide');
                fetchAll().done(render);
            } else {
                notify('error', (resp && resp.message) || 'No se pudo eliminar. Puede estar en uso.');
            }
        }).fail(function () {
            notify('error', 'Error de conexión al eliminar.');
        }).always(function () {
            $btn.prop('disabled', false).text('Eliminar');
        });
    }

    // -------------------- Eventos --------------------
    function bind() {
        // Chips
        $(document).on('click', '.cc-chip', function () {
            var t = $(this).data('type');
            $('.cc-chip').removeClass('active');
            $(this).addClass('active');
            state.activeTipo = t;
            render();
        });

        // Search
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
            fetchAll().done(function () { render(); notify('success', 'Datos actualizados.'); });
        });

        // Dropdown "Nuevo"
        $('#cc-new-btn').on('click', function (e) {
            e.stopPropagation();
            $('#cc-new-menu').toggleClass('show');
        });
        $(document).on('click', function (e) {
            if (!$(e.target).closest('.cc-new-wrap').length) $('#cc-new-menu').removeClass('show');
        });
        $(document).on('click', '.cc-new-menu button', function () {
            var tipo = $(this).data('new-type');
            $('#cc-new-menu').removeClass('show');
            openEdit(tipo, null);
        });

        // Acciones fila
        $(document).on('click', '.cc-action-btn.view', function (e) {
            e.stopPropagation();
            var $tr = $(this).closest('tr');
            openDetail($tr.data('tipo'), $tr.data('id'));
        });
        $(document).on('click', '.cc-action-btn.edit', function (e) {
            e.stopPropagation();
            var $tr = $(this).closest('tr');
            var row = findRow($tr.data('tipo'), $tr.data('id'));
            if (row) openEdit(row._tipo, row);
        });
        $(document).on('click', '.cc-action-btn.delete', function (e) {
            e.stopPropagation();
            var $tr = $(this).closest('tr');
            var row = findRow($tr.data('tipo'), $tr.data('id'));
            if (row) openDelete(row._tipo, row.id, row.descripcion);
        });

        // Click en fila abre detalle
        $(document).on('click', '.cc-table tbody tr', function (e) {
            if ($(e.target).closest('.cc-action-btn').length) return;
            openDetail($(this).data('tipo'), $(this).data('id'));
        });

        // Form submit
        $('#cc-edit-form').on('submit', function (e) { e.preventDefault(); submitEdit(); });
        $('#cc-edit-save').on('click', function (e) { e.preventDefault(); submitEdit(); });

        $('#cc-delete-confirm').on('click', submitDelete);
    }

    // -------------------- Init --------------------
    $(function () {
        bind();
        fetchAll().always(render);
    });

})();
