var KPIsFinancieros = (function () {
    var obj = {}, urlFind, gaugeChart;

    function fmt(v) {
        var n = parseFloat(v || 0);
        return "L. " + n.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }

    function pctColor(pct) {
        return pct >= 80 ? "#22c55e" : (pct >= 50 ? "#f59e0b" : "#ef4444");
    }

    function renderGauge(pct) {
        var ctx = document.getElementById("kpi-gauge");
        if (!ctx) return;
        if (gaugeChart) gaugeChart.destroy();

        var color = pctColor(pct);
        var resto = 100 - pct;

        gaugeChart = new Chart(ctx.getContext("2d"), {
            type: "doughnut",
            data: {
                datasets: [{
                    data: [pct, resto],
                    backgroundColor: [color, "#f1f5f9"],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                cutout: "72%",
                plugins: {
                    legend: { display: false },
                    tooltip: { enabled: false }
                },
                animation: { animateRotate: true, duration: 900 }
            }
        });

        $("#kpi-gauge-label").text(pct.toFixed(1) + "%").css("color", color);
    }

    function renderDetail(item) {
        var cobrado    = item.TotalCobrado           != null ? parseFloat(item.TotalCobrado)           : null;
        var pendiente  = item.TotalPendiente          != null ? parseFloat(item.TotalPendiente)          : null;
        var morosos    = item.TotalMorosos            != null ? item.TotalMorosos                       : null;
        var pct        = item.PorcentajeRecuperacion  != null ? parseFloat(item.PorcentajeRecuperacion)  : null;

        var totalCartera = (cobrado || 0) + (pendiente || 0);
        var pctVal = pct !== null ? pct : (totalCartera > 0 ? (cobrado / totalCartera * 100) : 0);
        var color  = pctColor(pctVal);

        var rows = [
            {
                icon: "fa-circle-dollar-to-slot", iconClass: "teal",
                label: "Total Cobrado",
                val: cobrado !== null ? fmt(cobrado) : "—",
                sub: "Ingresos confirmados"
            },
            {
                icon: "fa-hourglass-half", iconClass: "amber",
                label: "Total Pendiente",
                val: pendiente !== null ? fmt(pendiente) : "—",
                sub: "Por recuperar"
            },
            {
                icon: "fa-user-slash", iconClass: "red",
                label: "Alumnos Morosos",
                val: morosos !== null ? morosos : "—",
                sub: "Con pagos vencidos"
            }
        ];

        var html = "";
        rows.forEach(function (r) {
            html += '<div class="kpi-detail-row">';
            html += '<div class="kpi-detail-icon kpi-icon-fan ' + r.iconClass + '"><i class="fa-solid ' + r.icon + '"></i></div>';
            html += '<div class="kpi-detail-info">';
            html += '<div class="kpi-detail-label">' + r.label + '</div>';
            html += '<div class="kpi-detail-val">'   + r.val   + '</div>';
            html += '<div class="kpi-detail-sub">'   + r.sub   + '</div>';
            html += '</div></div>';
        });

        html += '<div style="padding:14px 18px;">';
        html += '<div style="display:flex;justify-content:space-between;margin-bottom:4px;">';
        html += '<span style="font-size:.72rem;color:#64748b;font-weight:600;text-transform:uppercase;letter-spacing:.5px;">Progreso de Recuperación</span>';
        html += '<span style="font-size:.8rem;font-weight:800;color:' + color + ';">' + pctVal.toFixed(1) + '%</span>';
        html += '</div>';
        html += '<div class="rec-track"><div class="rec-fill" style="width:' + pctVal.toFixed(1) + '%;background:' + color + ';"></div></div>';
        html += '<div style="display:flex;justify-content:space-between;margin-top:4px;">';
        html += '<span style="font-size:.68rem;color:#94a3b8;">0%</span>';
        html += '<span style="font-size:.68rem;color:#94a3b8;">100%</span>';
        html += '</div></div>';

        $("#kpi-detail-container").html(html);
    }

    function resetAll() {
        $("#kpi-total-cobrado, #kpi-total-pendiente, #kpi-total-morosos, #kpi-porcentaje-recuperacion").text("—");
        $("#kpi-gauge-label").text("—").css("color", "#94a3b8");
        $("#kpi-detail-container").html('<div class="fan-loading">Sin datos.</div>');
        if (gaugeChart) { gaugeChart.destroy(); gaugeChart = null; }
    }

    function cargarKPIs() {
        var anio = $("#filtro-anio").val();
        var mes  = $("#filtro-mes").val();

        var data = { Anio: anio };
        if (mes && mes !== "") data.Mes = mes;

        $("#kpi-detail-container").html('<div class="fan-loading"><i class="fa-solid fa-spinner fa-spin me-2"></i>Cargando...</div>');

        $.ajax({
            url: urlFind,
            data: data,
            dataType: "json",
            success: function (response) {
                var item = response.data;
                if (!item) { resetAll(); return; }

                var cobrado    = item.TotalCobrado           != null ? parseFloat(item.TotalCobrado)           : null;
                var pendiente  = item.TotalPendiente          != null ? parseFloat(item.TotalPendiente)          : null;
                var morosos    = item.TotalMorosos            != null ? item.TotalMorosos                       : null;
                var pct        = item.PorcentajeRecuperacion  != null ? parseFloat(item.PorcentajeRecuperacion)  : null;

                $("#kpi-total-cobrado").text(cobrado    !== null ? fmt(cobrado)    : "—");
                $("#kpi-total-pendiente").text(pendiente !== null ? fmt(pendiente) : "—");
                $("#kpi-total-morosos").text(morosos     !== null ? morosos        : "—");
                $("#kpi-porcentaje-recuperacion").text(pct !== null ? pct.toFixed(1) + "%" : "—");

                var pctVal = pct !== null ? pct : 0;
                renderGauge(pctVal);
                renderDetail(item);
            },
            error: function () {
                resetAll();
            }
        });
    }

    obj.init = function (url) {
        urlFind = url;
        $("#filtro-anio").val(new Date().getFullYear());

        $("#btn-filtrar").on("click", function () {
            cargarKPIs();
        });

        cargarKPIs();
    };

    return obj;
}());
