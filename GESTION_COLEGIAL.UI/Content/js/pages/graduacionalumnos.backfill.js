/* Modulo: Backfill historico de graduados.
 * Ejecuta el SP app.spBackfillGraduadosHistoricos via API y renderiza el detalle.
 */
var GraduacionBackfill = (function () {
    var cfg = null;

    function init(config) {
        cfg = config || {};
        $(document).ready(function () {
            $('#btn-ejecutar-backfill').on('click', ejecutar);
        });
    }

    function ejecutar() {
        var anio = $('#bf-anio').val();
        var topN = parseInt($('#bf-topn').val(), 10) || 50;
        var dryRun = $('#bf-dryrun').is(':checked');

        $('#btn-ejecutar-backfill').prop('disabled', true).text('Ejecutando...');
        $('#bf-resumen').html('');
        $('#tbl-backfill tbody').empty();

        $.ajax({
            url: cfg.ejecutarUrl,
            type: 'POST',
            data: {
                anio: anio ? parseInt(anio, 10) : null,
                topN: topN,
                dryRun: dryRun
            }
        }).done(function (resp) {
            var data = (resp && resp.data) ? resp.data : null;
            if (!data) {
                $('#bf-resumen').html('<div class="alert alert-warning">Sin respuesta del API.</div>');
                return;
            }
            var modo = data.DryRun ? '<span class="badge badge-info">Simulación</span>' : '<span class="badge badge-danger">Real</span>';
            $('#bf-resumen').html(
                '<div class="alert alert-secondary">' +
                modo + ' &nbsp; Año: <b>' + (data.Anio || 'todos') + '</b> · Top N: <b>' + data.TopN + '</b> · ' +
                'Total: <b>' + data.Total + '</b> · ' +
                'Exitosos: <b>' + data.Exitosos + '</b> · Fallidos: <b>' + data.Fallidos + '</b>' +
                '</div>'
            );
            var tbody = $('#tbl-backfill tbody');
            (data.Detalle || []).forEach(function (d) {
                var clase = (d.Resultado || '').toLowerCase().indexOf('ok') >= 0 ? 'success' : 'warning';
                tbody.append(
                    '<tr>' +
                    '<td>' + (d.Alu_Id || '') + '</td>' +
                    '<td>' + (d.NombreCompleto || '') + '</td>' +
                    '<td>' + (d.AnioGraduacion || '') + '</td>' +
                    '<td><span class="badge badge-' + clase + '">' + (d.Resultado || '') + '</span></td>' +
                    '<td>' + (d.Mensaje || '') + '</td>' +
                    '</tr>'
                );
            });
        }).fail(function () {
            $('#bf-resumen').html('<div class="alert alert-danger">Error al ejecutar el backfill.</div>');
        }).always(function () {
            $('#btn-ejecutar-backfill').prop('disabled', false).html('<i class="bi bi-play-fill"></i> Ejecutar');
        });
    }

    return { init: init };
})();
