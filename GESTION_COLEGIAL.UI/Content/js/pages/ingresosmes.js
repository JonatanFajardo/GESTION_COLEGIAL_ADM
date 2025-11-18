var IngresosMes = (function () {
    var obj = {};
    var urls = {};
    var chart = null;

    obj.init = function (config) {
        urls = config;

        $(function () {
            // Establecer mes y año actual
            var fecha = new Date();
            $("#mes-select").val(fecha.getMonth() + 1);
            $("#anio-select").val(fecha.getFullYear());

            $("#btn-generar-reporte").click(function() {
                generarReporte();
            });

            // Generar reporte inicial
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
                } else {
                    alertConfig.alert(response.message, "error");
                }
            }
        });
    }

    function renderReporte(data) {
        // Actualizar resumen
        $("#total-ingresos").text("L. " + formatMoney(data.resumen.totalIngresos));
        $("#cantidad-pagos").text(data.resumen.cantidadPagos);
        $("#promedio-diario").text("L. " + formatMoney(data.resumen.promedioDiario));

        // Renderizar gráfico
        renderChart(data.ingresosPorDia);

        // Renderizar tabla por concepto
        renderTablaConceptos(data.ingresosPorConcepto, data.resumen.totalIngresos);
    }

    function renderChart(ingresosPorDia) {
        var labels = [];
        var valores = [];

        ingresosPorDia.forEach(function(item) {
            labels.push("Día " + item.dia);
            valores.push(item.monto);
        });

        var ctx = document.getElementById('chart-ingresos-dia').getContext('2d');

        // Destruir gráfico anterior si existe
        if (chart) {
            chart.destroy();
        }

        chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Ingresos (L.)',
                    data: valores,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
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
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return 'L. ' + formatMoney(context.parsed.y);
                            }
                        }
                    }
                }
            }
        });
    }

    function renderTablaConceptos(conceptos, totalGeneral) {
        var html = "";
        var totalCantidad = 0;

        conceptos.forEach(function(concepto) {
            var porcentaje = (concepto.monto / totalGeneral * 100).toFixed(2);
            totalCantidad += concepto.cantidad;

            html += "<tr>";
            html += "<td>" + concepto.concepto + "</td>";
            html += "<td>" + concepto.cantidad + "</td>";
            html += "<td>L. " + formatMoney(concepto.monto) + "</td>";
            html += "<td>" + porcentaje + "%</td>";
            html += "</tr>";
        });

        $("#tbody-conceptos").html(html);
        $("#total-cantidad").text(totalCantidad);
        $("#total-monto").text("L. " + formatMoney(totalGeneral));
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    return obj;
}());
