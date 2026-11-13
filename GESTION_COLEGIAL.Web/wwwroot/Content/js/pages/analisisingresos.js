var AnalisisIngresos = (function () {
    var obj = {}, urlBase;

    function fmt(v) {
        var n = parseFloat(v || 0);
        return "L. " + n.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }

    function pctColor(pct) {
        return pct >= 80 ? "#22c55e" : (pct >= 50 ? "#f59e0b" : "#ef4444");
    }

    function renderKPIs(data) {
        var cobrado   = data.reduce(function (s, r) { return s + (r.TotalCobrado   || 0); }, 0);
        var pendiente = data.reduce(function (s, r) { return s + (r.TotalPendiente || 0); }, 0);
        var total     = cobrado + pendiente;
        var pct       = total > 0 ? (cobrado / total * 100) : 0;

        $("#ai-kpi-cobrado").text(fmt(cobrado));
        $("#ai-kpi-pendiente").text(fmt(pendiente));
        $("#ai-kpi-pct").text(pct.toFixed(1) + "%").css("color", pctColor(pct));
        $("#ai-kpi-conceptos").text(data.length);
    }

    function renderBars(data) {
        if (!data.length) {
            $("#ai-bars-container").html('<div class="text-center text-muted py-5">Sin datos.</div>');
            return;
        }

        var sorted = data.slice().sort(function (a, b) {
            return (b.PorcentajeCobrado || 0) - (a.PorcentajeCobrado || 0);
        });

        var html = "";
        sorted.forEach(function (r) {
            var pct   = parseFloat(r.PorcentajeCobrado || 0);
            var color = pctColor(pct);
            html += '<div class="mb-3">';
            html += '<div class="d-flex justify-content-between small mb-1">';
            html += '<span class="font-weight-bold text-truncate mr-2" title="' + (r.ConceptoPago || "—") + '">' + (r.ConceptoPago || "—") + '</span>';
            html += '<span class="font-weight-bold" style="color:' + color + ';">' + pct.toFixed(0) + '%</span>';
            html += '</div>';
            html += '<div class="progress mb-0"><div class="progress-bar" role="progressbar" style="width:' + pct.toFixed(1) + '%;background:' + color + ';"></div></div>';
            html += '</div>';
        });

        $("#ai-bars-container").html(html);
    }

    function renderTable(data) {
        if (!data.length) {
            $("#ai-table-container").html('<div class="text-center text-muted py-5">Sin datos para el año seleccionado.</div>');
            return;
        }

        var rows = "";
        data.forEach(function (r) {
            var cobrado   = r.TotalCobrado   || 0;
            var pendiente = r.TotalPendiente || 0;
            var pct       = parseFloat(r.PorcentajeCobrado || 0);
            var color     = pctColor(pct);

            rows += '<tr>';
            rows += '<td class="font-weight-bold">' + (r.ConceptoPago || "—") + '</td>';
            rows += '<td class="text-right">' + fmt(cobrado) + '</td>';
            rows += '<td class="text-right">' + fmt(pendiente) + '</td>';
            rows += '<td>';
            rows += '<div class="d-flex align-items-center">';
            rows += '<div class="progress flex-grow-1 mb-0"><div class="progress-bar" role="progressbar" style="width:' + pct.toFixed(1) + '%;background:' + color + ';"></div></div>';
            rows += '<span class="small font-weight-bold text-right ml-2" style="min-width:38px;color:' + color + ';">' + pct.toFixed(1) + '%</span>';
            rows += '</div></td>';
            rows += '</tr>';
        });

        var html = '<div class="table-responsive"><table class="table table-hover mb-0">'
            + '<thead><tr><th>Concepto</th><th class="text-right">Cobrado</th><th class="text-right">Pendiente</th><th style="width:160px;">% Cobrado</th></tr></thead>'
            + '<tbody>' + rows + '</tbody></table></div>';

        $("#ai-table-container").html(html);
    }

    function cargar(anio) {
        $("#ai-bars-container, #ai-table-container").html('<div class="text-center text-muted py-5"><i class="fas fa-spinner fa-spin mr-2"></i>Cargando...</div>');

        $.ajax({
            url: urlBase + "?Anio=" + anio,
            dataType: "json",
            success: function (response) {
                var data = response.data || [];
                renderKPIs(data);
                renderBars(data);
                renderTable(data);
            },
            error: function () {
                $("#ai-bars-container, #ai-table-container").html('<div class="text-center text-danger py-5">Error al cargar los datos.</div>');
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
