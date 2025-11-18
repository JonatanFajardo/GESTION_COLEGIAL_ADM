var ProyeccionCobros = (function () {
    var obj = {};
    var urls = {};
    var chart = null;

    obj.init = function (config) {
        urls = config;

        $(function () {
            var fecha = new Date();
            $("#mes-select").val(fecha.getMonth() + 1);
            $("#anio-select").val(fecha.getFullYear());

            $("#btn-generar").click(function() {
                generarReporte();
            });

            generarReporte();
        });
    }

    function generarReporte() {
        var mes = $("#mes-select").val();
        var anio = $("#anio-select").val();

        $.ajax({
            url: urls.urlReporte,
            data: { anio: anio, mes: mes },
            success: function(response) {
                if (response.success) {
                    renderReporte(response.data);
                }
            }
        });
    }

    function renderReporte(data) {
        $("#proyeccion-total").text("L. " + formatMoney(data.resumen.proyeccionTotal));
        $("#monto-cobrado").text("L. " + formatMoney(data.resumen.montoCobrado));
        $("#porcentaje-cumplimiento").text(data.resumen.porcentajeCumplimiento.toFixed(2) + "%");

        renderChart(data.resumen);
        renderTablaNiveles(data.desglosePorNivel);
    }

    function renderChart(resumen) {
        var ctx = document.getElementById('chart-proyeccion').getContext('2d');

        if (chart) {
            chart.destroy();
        }

        chart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Cobrado', 'Pendiente'],
                datasets: [{
                    data: [resumen.montoCobrado, resumen.proyeccionTotal - resumen.montoCobrado],
                    backgroundColor: ['rgba(75, 192, 192, 0.6)', 'rgba(255, 99, 132, 0.6)']
                }]
            }
        });
    }

    function renderTablaNiveles(niveles) {
        var html = "";
        niveles.forEach(function(nivel) {
            var porcentaje = (nivel.cobrado / nivel.proyeccion * 100).toFixed(2);
            html += "<tr>";
            html += "<td>" + nivel.nivel + "</td>";
            html += "<td>" + nivel.cantidadAlumnos + "</td>";
            html += "<td>L. " + formatMoney(nivel.proyeccion) + "</td>";
            html += "<td>L. " + formatMoney(nivel.cobrado) + "</td>";
            html += "<td>L. " + formatMoney(nivel.pendiente) + "</td>";
            html += "<td>" + porcentaje + "%</td>";
            html += "</tr>";
        });
        $("#tbody-niveles").html(html);
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    return obj;
}());
