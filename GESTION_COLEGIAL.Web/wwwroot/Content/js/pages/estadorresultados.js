var EstadoResultados = (function () {
    var obj = {}, urlBase, chartInstance;

    var PALETA = [
        "#14b8a6","#6366f1","#f59e0b","#22c55e","#0ea5e9",
        "#a855f7","#f43f5e","#f97316","#84cc16","#06b6d4"
    ];

    function fmt(v) {
        var n = parseFloat(v || 0);
        return "L. " + n.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }

    function renderKPIs(data) {
        var ingresos = 0, egresos = 0;
        data.forEach(function (r) {
            var m = parseFloat(r.Monto || 0);
            if (m >= 0) ingresos += m; else egresos += Math.abs(m);
        });
        var resultado = ingresos - egresos;

        $("#er-kpi-ingresos").text(fmt(ingresos));
        $("#er-kpi-egresos").text(fmt(egresos));
        $("#er-kpi-resultado").text(fmt(resultado)).css("color", resultado >= 0 ? "#22c55e" : "#ef4444");
        $("#er-kpi-conceptos").text(data.length);
    }

    function renderChart(data) {
        var ctx = document.getElementById("er-chart");
        if (!ctx) return;
        if (chartInstance) chartInstance.destroy();

        var labels = data.map(function (r) { return r.Concepto || "—"; });
        var values = data.map(function (r) { return Math.abs(parseFloat(r.Monto || 0)); });
        var colors = labels.map(function (_, i) { return PALETA[i % PALETA.length]; });

        chartInstance = new Chart(ctx.getContext("2d"), {
            type: "doughnut",
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderWidth: 2,
                    borderColor: "#fff"
                }]
            },
            options: {
                responsive: true,
                cutout: "62%",
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return " " + c.label + ": " + fmt(c.raw);
                            }
                        }
                    }
                }
            }
        });
    }

    function renderTable(data) {
        if (!data.length) {
            $("#er-table-container").html('<div class="text-center text-muted py-5">Sin datos para el período seleccionado.</div>');
            return;
        }

        var total = data.reduce(function (s, r) { return s + parseFloat(r.Monto || 0); }, 0);
        var rows  = "";

        data.forEach(function (r, i) {
            var monto = parseFloat(r.Monto || 0);
            var color = PALETA[i % PALETA.length];
            var montoClass = monto >= 0 ? "text-success font-weight-bold" : "text-danger font-weight-bold";

            rows += '<tr>';
            rows += '<td>';
            rows += '<i class="fas fa-circle small mr-2" style="color:' + color + ';"></i>';
            rows += '<span class="font-weight-bold">' + (r.Concepto || "—") + '</span></td>';
            rows += '<td class="text-right ' + montoClass + '">' + fmt(monto) + '</td>';
            rows += '</tr>';
        });

        rows += '<tr class="table-success font-weight-bold">';
        rows += '<td class="font-weight-bold">TOTAL</td>';
        rows += '<td class="text-right">' + fmt(total) + '</td>';
        rows += '</tr>';

        var html = '<div class="table-responsive"><table class="table table-hover mb-0">'
            + '<thead><tr><th>Concepto</th><th class="text-right">Monto (L.)</th></tr></thead>'
            + '<tbody>' + rows + '</tbody></table></div>';

        $("#er-table-container").html(html);
    }

    function cargar(anio, mes) {
        $("#er-table-container").html('<div class="text-center text-muted py-5"><i class="fas fa-spinner fa-spin mr-2"></i>Cargando...</div>');

        $.ajax({
            url: urlBase + "?Anio=" + anio + "&Mes=" + mes,
            dataType: "json",
            success: function (response) {
                var data = Array.isArray(response) ? response : (response.data || []);
                renderKPIs(data);
                renderChart(data);
                renderTable(data);
            },
            error: function () {
                $("#er-table-container").html('<div class="text-center text-danger py-5">Error al cargar los datos.</div>');
            }
        });
    }

    obj.init = function (url, anioActual, mesActual) {
        urlBase = url;

        $("#btn-consultar").on("click", function () {
            var anio = $("#f-anio").val();
            var mes  = $("#f-mes").val();
            if (anio && mes) cargar(anio, mes);
        });

        cargar(anioActual, mesActual);
    };

    return obj;
}());
