var ComparativoAnios = (function () {
    var obj = {}, urlBase, chartInstance;

    function renderKPIs(data, anioBase, anioComp) {
        var totalBase = data.reduce(function (s, r) { return s + (r.TotalAnioBase        || 0); }, 0);
        var totalComp = data.reduce(function (s, r) { return s + (r.TotalAnioComparacion || 0); }, 0);
        var dif       = totalComp - totalBase;
        var pct       = totalBase > 0 ? (dif / totalBase * 100) : 0;

        $("#ca-kpi-base").text(totalBase);
        $("#ca-kpi-comp").text(totalComp);
        $("#ca-kpi-dif")
            .text((dif >= 0 ? "+" : "") + dif)
            .css("color", dif > 0 ? "#22c55e" : (dif < 0 ? "#ef4444" : "#94a3b8"));
        $("#ca-kpi-pct")
            .text((pct >= 0 ? "+" : "") + pct.toFixed(1) + "%")
            .css("color", pct > 0 ? "#22c55e" : (pct < 0 ? "#ef4444" : "#94a3b8"));
    }

    function renderChart(data, anioBase, anioComp) {
        var ctx = document.getElementById("ca-chart");
        if (!ctx) return;
        if (chartInstance) chartInstance.destroy();

        var labels = data.map(function (r) { return r.Cur_Descripcion || "—"; });
        var base   = data.map(function (r) { return r.TotalAnioBase        || 0; });
        var comp   = data.map(function (r) { return r.TotalAnioComparacion || 0; });

        $("#ca-chart-subtitle").text(anioBase + " vs " + anioComp);

        chartInstance = new Chart(ctx.getContext("2d"), {
            type: "bar",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: String(anioBase),
                        data: base,
                        backgroundColor: "rgba(14,165,233,.75)",
                        borderRadius: 4,
                        borderSkipped: false
                    },
                    {
                        label: String(anioComp),
                        data: comp,
                        backgroundColor: "rgba(20,184,166,.7)",
                        borderRadius: 4,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { position: "top", labels: { font: { size: 11 }, boxWidth: 12 } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 } } },
                    y: {
                        grid: { color: "#f1f5f9" },
                        ticks: { font: { size: 10 }, stepSize: 1 }
                    }
                }
            }
        });
    }

    function renderTable(data, anioBase, anioComp) {
        if (!data.length) {
            $("#ca-table-container").html('<div class="text-center text-muted py-5">Sin datos para los años seleccionados.</div>');
            return;
        }

        var rows = "";
        data.forEach(function (r) {
            var dif = r.Diferencia       != null ? r.Diferencia       : "—";
            var pct = r.PorcentajeCambio != null ? r.PorcentajeCambio : null;

            var difClass = "";
            var difHtml  = "—";
            if (typeof dif === "number") {
                difClass = dif > 0 ? "text-success font-weight-bold" : (dif < 0 ? "text-danger font-weight-bold" : "text-muted");
                var arrow = dif > 0
                    ? '<i class="fas fa-arrow-up mr-1"></i>'
                    : (dif < 0 ? '<i class="fas fa-arrow-down mr-1"></i>' : '');
                difHtml = '<span class="' + difClass + '">' + arrow + (dif > 0 ? "+" : "") + dif + '</span>';
            }

            var pctHtml = "—";
            if (pct !== null) {
                var pctClass = pct > 0 ? "text-success font-weight-bold" : (pct < 0 ? "text-danger font-weight-bold" : "text-muted");
                var badge    = pct > 0 ? "outline-badge-success" : (pct < 0 ? "outline-badge-danger" : "outline-badge-secondary");
                pctHtml = '<span class="badge ' + badge + '">' + (pct > 0 ? "+" : "") + parseFloat(pct).toFixed(1) + '%</span>';
            }

            rows += '<tr>';
            rows += '<td class="font-weight-bold">' + (r.Cur_Descripcion || "—") + '</td>';
            rows += '<td class="text-center">' + (r.TotalAnioBase        || 0) + '</td>';
            rows += '<td class="text-center">' + (r.TotalAnioComparacion || 0) + '</td>';
            rows += '<td class="text-center">' + difHtml  + '</td>';
            rows += '<td class="text-center">' + pctHtml  + '</td>';
            rows += '</tr>';
        });

        var html = '<div class="table-responsive"><table class="table table-hover mb-0">'
            + '<thead><tr>'
            + '<th>Curso</th>'
            + '<th class="text-center" id="th-anio-base">' + anioBase + '</th>'
            + '<th class="text-center" id="th-anio-comp">' + anioComp + '</th>'
            + '<th class="text-center">Diferencia</th>'
            + '<th class="text-center">% Cambio</th>'
            + '</tr></thead>'
            + '<tbody>' + rows + '</tbody></table></div>';

        $("#ca-table-container").html(html);
    }

    function cargarDatos() {
        var anioBase = $("#filtro-anio-base").val();
        var anioComp = $("#filtro-anio-comparacion").val();

        $("#ca-table-container").html('<div class="text-center text-muted py-5"><i class="fas fa-spinner fa-spin mr-2"></i>Cargando...</div>');

        $.ajax({
            url: urlBase,
            data: { AnioBase: anioBase, AnioComparacion: anioComp },
            dataType: "json",
            success: function (response) {
                var data = response.data || [];
                renderKPIs(data, anioBase, anioComp);
                renderChart(data, anioBase, anioComp);
                renderTable(data, anioBase, anioComp);
            },
            error: function () {
                $("#ca-table-container").html('<div class="text-center text-danger py-5">Error al cargar los datos.</div>');
            }
        });
    }

    obj.init = function (url) {
        urlBase = url;

        var anioActual = new Date().getFullYear();
        $("#filtro-anio-base").val(anioActual - 1);
        $("#filtro-anio-comparacion").val(anioActual);

        $("#btn-filtrar").on("click", function () {
            cargarDatos();
        });

        cargarDatos();
    };

    return obj;
}());
