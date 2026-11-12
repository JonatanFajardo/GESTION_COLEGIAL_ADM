var ComparativaAnual = (function () {
    var obj = {};
    var urls = {};
    var chart = null;

    obj.init = function (config) {
        urls = config;

        $(function () {
            var anioActual = new Date().getFullYear();
            $("#anio-inicio").val(anioActual - 1);
            $("#anio-fin").val(anioActual);

            cargarConceptos();

            $("#btn-generar").click(function() {
                generarReporte();
            });

            generarReporte();
        });
    }

    function cargarConceptos() {
        $.ajax({
            url: urls.urlConceptos,
            success: function(data) {
                var html = '<option value="">-- Todos --</option>';
                data.forEach(function(item) {
                    html += '<option value="' + item.value + '">' + item.text + '</option>';
                });
                $("#concepto-filtro").html(html);
            }
        });
    }

    function generarReporte() {
        var anioInicio = $("#anio-inicio").val();
        var anioFin = $("#anio-fin").val();
        var conceptoId = $("#concepto-filtro").val();

        $.ajax({
            url: urls.urlReporte,
            data: {
                anioInicio: anioInicio,
                anioFin: anioFin,
                conceptoId: conceptoId
            },
            success: function(response) {
                if (response.success) {
                    renderReporte(response.data);
                }
            }
        });
    }

    function renderReporte(data) {
        renderChart(data.datosPorAnio);
        renderTabla(data.datosPorAnio);
        renderIndicadores(data.indicadores);
    }

    function renderChart(datosPorAnio) {
        var ctx = document.getElementById('chart-comparativa').getContext('2d');

        var datasets = [];
        var colors = [
            'rgba(255, 99, 132, 0.6)',
            'rgba(54, 162, 235, 0.6)',
            'rgba(255, 206, 86, 0.6)',
            'rgba(75, 192, 192, 0.6)',
            'rgba(153, 102, 255, 0.6)'
        ];

        var colorIndex = 0;
        for (var anio in datosPorAnio) {
            datasets.push({
                label: anio,
                data: datosPorAnio[anio],
                borderColor: colors[colorIndex],
                backgroundColor: colors[colorIndex],
                fill: false
            });
            colorIndex++;
        }

        if (chart) {
            chart.destroy();
        }

        chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                datasets: datasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return 'L. ' + formatMoney(value);
                            }
                        }
                    }
                }
            }
        });
    }

    function renderTabla(datosPorAnio) {
        var html = "";
        for (var anio in datosPorAnio) {
            var total = datosPorAnio[anio].reduce((a, b) => a + b, 0);
            html += "<tr>";
            html += "<td><strong>" + anio + "</strong></td>";
            datosPorAnio[anio].forEach(function(monto) {
                html += "<td>L. " + formatMoney(monto) + "</td>";
            });
            html += "<td><strong>L. " + formatMoney(total) + "</strong></td>";
            html += "</tr>";
        }
        $("#tbody-comparativa").html(html);
    }

    function renderIndicadores(indicadores) {
        $("#crecimiento").text(indicadores.crecimiento.toFixed(2) + "%");
        $("#crecimiento").removeClass("text-success text-danger");
        $("#crecimiento").addClass(indicadores.crecimiento >= 0 ? "text-success" : "text-danger");

        $("#mejor-mes").text(indicadores.mejorMes);
        $("#peor-mes").text(indicadores.peorMes);
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    return obj;
}());
