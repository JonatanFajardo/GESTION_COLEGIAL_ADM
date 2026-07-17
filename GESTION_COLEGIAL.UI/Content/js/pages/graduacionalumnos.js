/* Modulo: Hub de Graduacion.
 * - Tab Candidatos: renderiza la tabla de candidatos, permite seleccionar varios
 *   y lanza Graduar en bucle. Captura los mensajes de RAISERROR del SP.
 * - initHub(): orquesta las tabs del hub (Candidatos / Directorio / Backfill).
 *   El contenido de Directorio y Backfill lo manejan alumnosgraduados.js y
 *   graduacionalumnos.backfill.js respectivamente (sin colisiones de ids).
 */
var GraduacionAlumnos = (function () {
    var cfg = null;
    var dt = null;

    function init(config) {
        cfg = config || {};
        $(document).ready(function () {
            build();
            loadNotaMinima();
            $('#btn-guardar-nota-minima').on('click', guardarNotaMinima);
        });
    }

    // ═══════════ Orquestación de tabs del hub ═══════════
    function initHub() {
        $(document).ready(function () {
            $('#grad-hub-tabs').on('click', '.grad-hub-tab', function () {
                activarTab($(this).data('tab'));
            });
            // Tab inicial por querystring: /GraduacionAlumnos?tab=directorio|backfill
            var tabInicial = '';
            try {
                tabInicial = (new URLSearchParams(window.location.search).get('tab') || '').toLowerCase();
            } catch (e) { /* navegador sin URLSearchParams: queda la tab default */ }
            if (tabInicial === 'directorio' || tabInicial === 'backfill') {
                activarTab(tabInicial);
            }
        });
    }

    function activarTab(tab) {
        if (!tab || !$('#hub-panel-' + tab).length) return;
        $('#grad-hub-tabs .grad-hub-tab').removeClass('active')
            .filter('[data-tab="' + tab + '"]').addClass('active');
        $('.grad-hub-panel').removeClass('active');
        $('#hub-panel-' + tab).addClass('active');
        // La tabla se inicializa en la tab default; al volver a mostrarla,
        // reajustar anchos de columnas (DataTables no mide bien en display:none).
        if (tab === 'candidatos' && dt) {
            dt.columns.adjust();
        }
    }

    function loadNotaMinima() {
        if (!cfg.notaMinimaUrl) return;
        $.getJSON(cfg.notaMinimaUrl, function (resp) {
            var valor = (resp && resp.valor !== undefined) ? resp.valor : 70;
            $('#nota-minima-input').val(valor);
        });
    }

    function guardarNotaMinima() {
        var valor = parseFloat($('#nota-minima-input').val());
        if (isNaN(valor) || valor < 0 || valor > 100) {
            $('#nota-minima-status').text('Valor invalido. Use 0-100.').css('color', '#c0392b');
            return;
        }
        $('#nota-minima-status').text('Guardando...').css('color', '#6c757d');
        $.ajax({
            url: cfg.notaMinimaUrl,
            type: 'POST',
            data: { valor: valor }
        }).done(function (resp) {
            if (resp && resp.success) {
                $('#nota-minima-status').text('Guardado. Recargando candidatos...').css('color', '#27ae60');
                if (dt) dt.ajax.reload();
                setTimeout(function () { $('#nota-minima-status').text(''); }, 3000);
            } else {
                var msg = (resp && resp.item && resp.item.mensaje) ? resp.item.mensaje : 'Error al guardar';
                $('#nota-minima-status').text(msg).css('color', '#c0392b');
            }
        }).fail(function () {
            $('#nota-minima-status').text('Error de red').css('color', '#c0392b');
        });
    }

    function build() {
        dt = $('#tbl-candidatos').DataTable({
            ajax: {
                url: cfg.listUrl,
                dataSrc: function (resp) {
                    return (resp && resp.data) ? resp.data : [];
                }
            },
            columns: [
                {
                    data: 'Alu_Id', orderable: false, searchable: false,
                    render: function (id, type, row) {
                        var disabled = (row.EstadoValidacion || '').toLowerCase() !== 'cumple' ? 'disabled' : '';
                        return '<input type="checkbox" class="chk-candidato" data-id="' + id + '" ' + disabled + ' />';
                    }
                },
                { data: 'NombreCompleto' },
                { data: 'Niv_Descripcion' },
                { data: 'Cur_Descripcion' },
                { data: 'Sec_Descripcion' },
                {
                    data: 'NotasOk',
                    render: function (v) { return v ? '<span class="badge badge-success">OK</span>' : '<span class="badge badge-danger">Reprobado</span>'; }
                },
                {
                    data: 'DeudaPendiente',
                    render: function (v, type, row) {
                        var txt = (v === null || v === undefined) ? '0.00' : Number(v).toFixed(2);
                        return row.DeudaOk
                            ? '<span class="badge badge-success">L. ' + txt + '</span>'
                            : '<span class="badge badge-warning">L. ' + txt + '</span>';
                    }
                },
                {
                    data: 'EstadoValidacion',
                    render: function (v) {
                        var clase = (v && v.toLowerCase() === 'cumple') ? 'badge-success' : 'badge-secondary';
                        return '<span class="badge ' + clase + '">' + (v || '') + '</span>';
                    }
                },
                { data: 'MotivosBloqueo', defaultContent: '' }
            ],
            pageLength: 25,
            language: dataTableEspanol()
        });

        $('#chk-all-candidatos').on('change', function () {
            var checked = this.checked;
            $('.chk-candidato:not(:disabled)').prop('checked', checked);
            updateBoton();
        });
        $('#tbl-candidatos tbody').on('change', '.chk-candidato', updateBoton);
        $('#btn-graduar-seleccionados').on('click', graduarSeleccionados);
    }

    function updateBoton() {
        var n = $('.chk-candidato:checked').length;
        $('#btn-graduar-seleccionados').prop('disabled', n === 0)
            .html('<i class="bi bi-mortarboard-fill"></i> Graduar seleccionados (' + n + ')');
    }

    function graduarSeleccionados() {
        var ids = $('.chk-candidato:checked').map(function () { return parseInt($(this).data('id'), 10); }).get();
        if (!ids.length) return;

        var mensaje = '¿Confirma graduar a ' + ids.length + ' alumno(s)? Esta operación archiva al alumno y no es reversible.';
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Confirmar graduación',
                text: mensaje,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí, graduar',
                cancelButtonText: 'Cancelar'
            }).then(function (r) { if (r.isConfirmed) ejecutarBucle(ids); });
        } else if (window.confirm(mensaje)) {
            ejecutarBucle(ids);
        }
    }

    function ejecutarBucle(ids) {
        var exitosos = 0, fallidos = 0, errores = [];
        var promesas = ids.map(function (id) {
            return $.ajax({
                url: cfg.graduarUrl,
                type: 'POST',
                data: { alumnoId: id }
            }).then(function (resp) {
                if (resp && resp.success) {
                    exitosos++;
                } else {
                    fallidos++;
                    var msg = (resp && resp.item && resp.item.Mensaje) ? resp.item.Mensaje : 'Error desconocido';
                    errores.push('Alu ' + id + ': ' + msg);
                }
            }, function () {
                fallidos++;
                errores.push('Alu ' + id + ': error de red');
            });
        });

        $.when.apply($, promesas).always(function () {
            dt.ajax.reload();
            var resumen = 'Exitosos: ' + exitosos + ' · Fallidos: ' + fallidos;
            if (errores.length) resumen += '\n\n' + errores.join('\n');
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Resultado',
                    text: resumen,
                    icon: fallidos === 0 ? 'success' : 'warning'
                });
            } else {
                alert(resumen);
            }
        });
    }

    function dataTableEspanol() {
        return {
            sProcessing: 'Procesando...',
            sLengthMenu: 'Mostrar _MENU_ registros',
            sZeroRecords: 'No se encontraron resultados',
            sEmptyTable: 'Ningún dato disponible',
            sInfo: '_START_ a _END_ de _TOTAL_',
            sInfoEmpty: '0 a 0 de 0',
            sInfoFiltered: '(filtrado de _MAX_)',
            sSearch: 'Buscar:',
            oPaginate: { sFirst: 'Primero', sLast: 'Último', sNext: 'Siguiente', sPrevious: 'Anterior' }
        };
    }

    return { init: init, initHub: initHub };
})();
