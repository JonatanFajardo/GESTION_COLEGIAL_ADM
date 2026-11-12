var FlujoCaja = (function () {
    var obj = {}, urlBase, chartInstance;

    function fmt(v) {
        var n = parseFloat(v || 0);
        return "L. " + n.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }

    function pctColor(pct) {
        return pct >= 80 ? "#22c55e" : (pct >= 50 ? "#f59e0b" : "#ef4444");
    }

    function renderKPIs(data) {
        var cobrado   = data.reduce(function (s, r) { return s + (r.TotalIngresos  || 0); }, 0);
        var pendiente = data.reduce(function (s, r) { return s + (r.TotalPendiente || 0); }, 0);
        var neto      = data.reduce(function (s, r) { return s + (r.SaldoNeto      || 0); }, 0);
        var total     = cobrado + pendiente;
        var pct       = total > 0 ? (cobrado / total * 100) : 0;

        $("#fc-kpi-cobrado").text(fmt(cobrado));
        $("#fc-kpi-pendiente").text(fmt(pendiente));
        $("#fc-kpi-neto").text(fmt(neto)).css("color", neto >= 0 ? "#22c55e" : "#ef4444");
        $("#fc-kpi-porcentaje").text(pct.toFixed(1) + "%");
    }

    function renderChart(data) {
        var ctx = document.getElementById("fc-chart");
        if (!ctx) return;
        if (chartInstance) chartInstance.destroy();

        var labels    = data.map(function (r) { return (r.NombreMes || "").substring(0, 3); });
        var cobrados  = data.map(function (r) { return r.TotalIngresos  || 0; });
        var pendientes = data.map(function (r) { return r.TotalPendiente || 0; });

        chartInstance = new Chart(ctx.getContext("2d"), {
            type: "bar",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "Cobrado",
                        data: cobrados,
                        backgroundColor: "rgba(20,184,166,.75)",
                        borderRadius: 5,
                        borderSkipped: false
                    },
                    {
                        label: "Pendiente",
                        data: pendientes,
                        backgroundColor: "rgba(245,158,11,.55)",
                        borderRadius: 5,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { position: "top", labels: { font: { size: 11 }, boxWidth: 12 } },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return " " + c.dataset.label + ": " + fmt(c.raw);
                            }
                        }
                    }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 11 } } },
                    y: {
                        grid: { color: "#f1f5f9" },
                        ticks: {
                            font: { size: 10 },
                            callback: function (v) {
                                if (v >= 1000000) return "L." + (v / 1000000).toFixed(1) + "M";
                                if (v >= 1000)    return "L." + (v / 1000).toFixed(0) + "k";
                                return "L." + v;
                            }
                        }
                    }
                }
            }
        });
    }

    function renderTable(data) {
        if (!data.length) {
            $("#fc-table-container").html('<div class="fan-loading">Sin datos para el año seleccionado.</div>');
            return;
        }

        var rows = "";
        data.forEach(function (r) {
            var cobrado   = r.TotalIngresos  || 0;
            var pendiente = r.TotalPendiente || 0;
            var neto      = r.SaldoNeto      || 0;
            var total     = cobrado + pendiente;
            var pct       = total > 0 ? (cobrado / total * 100) : 0;
            var color     = pctColor(pct);
            var netoClass = neto >= 0 ? "diff-pos" : "diff-neg";

            rows += '<tr>';
            rows += '<td class="col-label">' + (r.NombreMes || "—") + '</td>';
            rows += '<td>' + fmt(cobrado) + '</td>';
            rows += '<td>' + fmt(pendiente) + '</td>';
            rows += '<td>';
            rows += '<div style="display:flex;align-items:center;gap:8px;">';
            rows += '<div class="fan-progress" style="flex:1;"><div class="fan-progress-bar" style="width:' + pct.toFixed(1) + '%;background:' + color + ';"></div></div>';
            rows += '<span style="font-size:.75rem;font-weight:700;color:' + color + ';min-width:38px;text-align:right;">' + pct.toFixed(0) + '%</span>';
            rows += '</div></td>';
            rows += '<td class="' + netoClass + '">' + fmt(neto) + '</td>';
            rows += '</tr>';
        });

        var html = '<table class="fan-table">'
            + '<thead><tr>'
            + '<th>Mes</th><th>Cobrado</th><th>Pendiente</th>'
            + '<th style="width:160px;">Recuperación</th><th>Saldo Neto</th>'
            + '</tr></thead>'
            + '<tbody>' + rows + '</tbody>'
            + '</table>';

        $("#fc-table-container").html(html);
    }

    function cargar(anio) {
        $("#fc-table-container").html('<div class="fan-loading"><i class="fa-solid fa-spinner fa-spin me-2"></i>Cargando...</div>');

        $.ajax({
            url: urlBase + "?Anio=" + anio,
            dataType: "json",
            success: function (response) {
                var data = response.data || [];
                renderKPIs(data);
                renderChart(data);
                renderTable(data);
            },
            error: function () {
                $("#fc-table-container").html('<div class="fan-loading text-danger">Error al cargar los datos.</div>');
            }
        });
    }

    obj.init = function (url, anioActual) {
        urlBase = url;

        $("#btn-consultar").on("click", function () {
            var anio = $("#f-anio").val();
            if (anio) cargar(anio);
        });

        $("#f-anio").on("keydown", function (e) {
            if (e.key === "Enter") $("#btn-consultar").trigger("click");
        });

        cargar(anioActual);
    };

    return obj;
}());
