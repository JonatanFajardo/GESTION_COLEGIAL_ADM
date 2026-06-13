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
            $("#ai-bars-container").html('<div class="fan-loading">Sin datos.</div>');
            return;
        }

        var sorted = data.slice().sort(function (a, b) {
            return (b.PorcentajeCobrado || 0) - (a.PorcentajeCobrado || 0);
        });

        var html = "";
        sorted.forEach(function (r) {
            var pct   = parseFloat(r.PorcentajeCobrado || 0);
            var color = pctColor(pct);
            html += '<div class="ai-bar-row">';
            html += '<span class="ai-bar-label" title="' + (r.ConceptoPago || "—") + '">' + (r.ConceptoPago || "—") + '</span>';
            html += '<div class="ai-bar-track"><div class="ai-bar-fill" style="width:' + pct.toFixed(1) + '%;background:' + color + ';"></div></div>';
            html += '<span class="ai-bar-pct" style="color:' + color + ';">' + pct.toFixed(0) + '%</span>';
            html += '</div>';
        });

        $("#ai-bars-container").html(html);
    }

    function renderTable(data) {
        if (!data.length) {
            $("#ai-table-container").html('<div class="fan-loading">Sin datos para el año seleccionado.</div>');
            return;
        }

        var rows = "";
        data.forEach(function (r) {
            var cobrado   = r.TotalCobrado   || 0;
            var pendiente = r.TotalPendiente || 0;
            var pct       = parseFloat(r.PorcentajeCobrado || 0);
            var color     = pctColor(pct);
            var badge     = pct >= 80 ? "up" : (pct >= 50 ? "fan-badge" : "down");

            rows += '<tr>';
            rows += '<td class="col-label">' + (r.ConceptoPago || "—") + '</td>';
            rows += '<td style="text-align:right;">' + fmt(cobrado) + '</td>';
            rows += '<td style="text-align:right;">' + fmt(pendiente) + '</td>';
            rows += '<td>';
            rows += '<div style="display:flex;align-items:center;gap:6px;">';
            rows += '<div class="fan-progress" style="flex:1;"><div class="fan-progress-bar" style="width:' + pct.toFixed(1) + '%;background:' + color + ';"></div></div>';
            rows += '<span style="font-size:.75rem;font-weight:700;color:' + color + ';min-width:38px;text-align:right;">' + pct.toFixed(1) + '%</span>';
            rows += '</div></td>';
            rows += '</tr>';
        });

        var html = '<table class="fan-table">'
            + '<thead><tr><th>Concepto</th><th style="text-align:right;">Cobrado</th><th style="text-align:right;">Pendiente</th><th style="width:160px;">% Cobrado</th></tr></thead>'
            + '<tbody>' + rows + '</tbody></table>';

        $("#ai-table-container").html(html);
    }

    function cargar(anio) {
        $("#ai-bars-container, #ai-table-container").html('<div class="fan-loading"><i class="fa-solid fa-spinner fa-spin me-2"></i>Cargando...</div>');

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
                $("#ai-bars-container, #ai-table-container").html('<div class="fan-loading text-danger">Error al cargar los datos.</div>');
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
