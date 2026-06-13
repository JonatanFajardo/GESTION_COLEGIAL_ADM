var VisorGraficos = (function () {
    var obj = {};

    function buildFinancieroChart(data) {
        var ctx = document.getElementById("chart-financiero");
        if (!ctx || !data) return;
        new Chart(ctx, {
            type: "bar",
            data: {
                labels: ["Total Cobrado", "Total Pendiente", "Mora Total"],
                datasets: [{
                    label: "Lempiras",
                    data: [
                        parseFloat(data.TotalCobrado || data.totalCobrado || 0),
                        parseFloat(data.TotalPendiente || data.totalPendiente || 0),
                        parseFloat(data.TotalMora || data.totalMora || 0)
                    ],
                    backgroundColor: ["#28a745", "#dc3545", "#ffc107"]
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { callback: function(v){ return "L. " + v.toLocaleString(); } } }
                }
            }
        });
    }

    function buildAdminChart(data) {
        var ctx = document.getElementById("chart-admin");
        if (!ctx || !data) return;
        new Chart(ctx, {
            type: "doughnut",
            data: {
                labels: ["Alumnos", "Empleados", "Secciones activas"],
                datasets: [{
                    data: [
                        parseInt(data.TotalAlumnos || data.totalAlumnos || 0),
                        parseInt(data.TotalEmpleados || data.totalEmpleados || 0),
                        parseInt(data.TotalSecciones || data.totalSecciones || 0)
                    ],
                    backgroundColor: ["#007bff", "#28a745", "#17a2b8"]
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { position: "bottom" } }
            }
        });
    }

    obj.init = function (urlFinanciero, urlResumen) {
        var loaded = 0;
        function hideLoading() {
            loaded++;
            if (loaded >= 2) $("#loading-msg").hide();
        }

        $.getJSON(urlFinanciero, function (res) {
            buildFinancieroChart(res);
            hideLoading();
        }).fail(function () { hideLoading(); });

        $.getJSON(urlResumen, function (res) {
            buildAdminChart(res);
            hideLoading();
        }).fail(function () { hideLoading(); });
    };

    return obj;
}());
