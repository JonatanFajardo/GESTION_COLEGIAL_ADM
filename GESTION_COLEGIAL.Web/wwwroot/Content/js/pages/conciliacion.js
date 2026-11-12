var Conciliacion = (function () {
    var obj = {};
    var urlPagos, urlPendientes;
    var allCobros = [];
    var allPendientes = [];

    function fmt(v) {
        var val = parseFloat(v) || 0;
        return "L. " + val.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }
    function esc(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }
    function initials(name) {
        if (!name) return "?";
        var p = String(name).trim().split(/\s+/);
        return ((p[0] ? p[0][0] : "") + (p.length > 1 ? p[p.length - 1][0] : "")).toUpperCase();
    }

    function parseHora(s) {
        if (!s) return null;
        var d = new Date(s);
        return isNaN(d.getTime()) ? null : d;
    }

    function render() {
        var q = ($("#f-buscar").val() || "").toLowerCase().trim();

        var cobros = allCobros.filter(function (r) {
            return !q || (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0;
        });
        var pendientes = allPendientes.filter(function (r) {
            return !q || (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0;
        });

        var totalCobrado = 0, totalPendiente = 0;
        var vencidas = 0, totalVencido = 0;
        var alumnos = {};

        cobros.forEach(function (r) {
            totalCobrado += parseFloat(r.Pag_MontoTotal) || 0;
            if (r.Alu_NombreCompleto) alumnos[r.Alu_NombreCompleto] = true;
        });
        pendientes.forEach(function (r) {
            var pend = parseFloat(r.Cco_MontoPendiente) || 0;
            totalPendiente += pend;
            var dias = parseInt(r.DiasVencida);
            if (!isNaN(dias) && dias > 0) { vencidas++; totalVencido += pend; }
            if (r.Alu_NombreCompleto) alumnos[r.Alu_NombreCompleto] = true;
        });

        var balance = totalCobrado - totalPendiente;
        var totalUniverso = totalCobrado + totalPendiente;
        var tasa = totalUniverso > 0 ? (totalCobrado / totalUniverso * 100) : 0;
        var prom = cobros.length > 0 ? totalCobrado / cobros.length : 0;

        // Balance hero
        $("#lbl-cobrado").text(fmt(totalCobrado));
        $("#lbl-pendiente").text(fmt(totalPendiente));
        $("#lbl-cobrado-sub").text(cobros.length + (cobros.length === 1 ? " operación" : " operaciones"));
        $("#lbl-pendiente-sub").text(pendientes.length + (pendientes.length === 1 ? " cuenta" : " cuentas"));
        $("#lbl-balance").text(fmt(balance));
        $("#lbl-balance-state").text(balance >= 0 ? "Favorable" : "Deficitario");

        var pctCobrado   = totalUniverso > 0 ? (totalCobrado   / totalUniverso * 100) : 0;
        var pctPendiente = totalUniverso > 0 ? (totalPendiente / totalUniverso * 100) : 0;
        $("#bb-cobrado").css("width",   pctCobrado.toFixed(2)   + "%");
        $("#bb-pendiente").css("width", pctPendiente.toFixed(2) + "%");

        // KPIs
        $("#kpi-tasa").text(tasa.toFixed(1) + "%");
        $("#kpi-prom").text(fmt(prom));
        $("#kpi-vencidas").text(vencidas);
        $("#kpi-vencidas-sub").text(vencidas > 0 ? fmt(totalVencido) + " vencidos" : "Sin vencidos");
        $("#kpi-alumnos").text(Object.keys(alumnos).length);
        $("#chip-cobros").text(cobros.length + (cobros.length === 1 ? " cobro" : " cobros"));
        $("#chip-pendientes").text(pendientes.length + (pendientes.length === 1 ? " pendiente" : " pendientes"));

        // Feed cobros
        var feedC = $("#feed-cobros").empty();
        if (cobros.length === 0) {
            feedC.html('<div class="fd-empty"><i class="fa-solid fa-inbox"></i><p>Sin cobros para mostrar</p></div>');
        } else {
            var sortedC = cobros.slice().sort(function (a, b) {
                var da = parseHora(a.Pag_FechaPago), db = parseHora(b.Pag_FechaPago);
                return (db ? db.getTime() : 0) - (da ? da.getTime() : 0);
            });
            sortedC.forEach(function (r) {
                var d = parseHora(r.Pag_FechaPago);
                var hora = d ? d.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" }) : "—";
                feedC.append(
                    '<div class="fd-feed-item">' +
                        '<div class="fd-feed-avatar av-emerald">' + esc(initials(r.Alu_NombreCompleto)) + '</div>' +
                        '<div class="fd-feed-body">' +
                            '<p class="fd-feed-title">' + esc(r.Alu_NombreCompleto || "—") + '</p>' +
                            '<p class="fd-feed-meta">' +
                                '<span><i class="fa-solid fa-tag"></i>' + esc(r.ConceptosCobrados || "—") + '</span>' +
                                '<span><i class="fa-solid fa-credit-card"></i>' + esc(r.Fpa_Descripcion || "—") + '</span>' +
                                '<span><i class="fa-regular fa-clock"></i>' + esc(hora) + '</span>' +
                            '</p>' +
                        '</div>' +
                        '<div class="fd-feed-right">' +
                            '<p class="fd-feed-amount">' + esc(fmt(r.Pag_MontoTotal)) + '</p>' +
                            '<span class="fd-chip c-emerald">Cobrado</span>' +
                        '</div>' +
                    '</div>'
                );
            });
        }

        // Feed pendientes
        var feedP = $("#feed-pendientes").empty();
        if (pendientes.length === 0) {
            feedP.html('<div class="fd-empty"><i class="fa-solid fa-circle-check"></i><p>Sin pendientes — todo cobrado</p></div>');
        } else {
            var sortedP = pendientes.slice().sort(function (a, b) {
                return (parseInt(b.DiasVencida) || 0) - (parseInt(a.DiasVencida) || 0);
            });
            sortedP.forEach(function (r) {
                var dias = parseInt(r.DiasVencida);
                var chip, chipCls;
                if (isNaN(dias))     { chip = "Sin fecha";   chipCls = ""; }
                else if (dias > 30)  { chip = dias + " d vencido"; chipCls = "c-rose"; }
                else if (dias > 0)   { chip = dias + " d vencido"; chipCls = "c-amber"; }
                else if (dias === 0) { chip = "Vence hoy";   chipCls = "c-amber"; }
                else                 { chip = Math.abs(dias) + " d por vencer"; chipCls = "c-cyan"; }

                feedP.append(
                    '<div class="fd-feed-item">' +
                        '<div class="fd-feed-avatar av-rose">' + esc(initials(r.Alu_NombreCompleto)) + '</div>' +
                        '<div class="fd-feed-body">' +
                            '<p class="fd-feed-title">' + esc(r.Alu_NombreCompleto || "—") + '</p>' +
                            '<p class="fd-feed-meta">' +
                                '<span><i class="fa-solid fa-tag"></i>' + esc(r.Cpa_Descripcion || "—") + '</span>' +
                                '<span><i class="fa-solid fa-coins"></i>' + esc(fmt(r.Cco_MontoOriginal)) + ' original</span>' +
                            '</p>' +
                        '</div>' +
                        '<div class="fd-feed-right">' +
                            '<p class="fd-feed-amount amt-rose">' + esc(fmt(r.Cco_MontoPendiente)) + '</p>' +
                            '<span class="fd-chip ' + chipCls + '">' + esc(chip) + '</span>' +
                        '</div>' +
                    '</div>'
                );
            });
        }
    }

    function cargar() {
        var fecha = $("#f-fecha").val();
        var p1 = $.get(urlPagos, { fecha: fecha });
        var p2 = $.get(urlPendientes);
        $.when(p1, p2).done(function (a, b) {
            allCobros     = (a[0] && a[0].data) ? a[0].data : [];
            allPendientes = (b[0] && b[0].data) ? b[0].data : [];
            render();
        }).fail(function () {
            allCobros = []; allPendientes = []; render();
        });
    }

    function cargarUltimaFechaConDatos(fecha, intentos) {
        if (intentos > 60) { cargarPendientes(); return; }
        var fechaStr = fecha.toISOString().split("T")[0];
        $.get(urlPagos, { fecha: fechaStr }, function (resp) {
            var cobros = (resp && resp.data) ? resp.data : [];
            if (cobros.length === 0) {
                var anterior = new Date(fecha);
                anterior.setDate(anterior.getDate() - 1);
                cargarUltimaFechaConDatos(anterior, intentos + 1);
            } else {
                $("#f-fecha").val(fechaStr);
                allCobros = cobros;
                cargarPendientes();
            }
        }).fail(function () {
            allCobros = []; allPendientes = []; render();
        });
    }

    function cargarPendientes() {
        $.get(urlPendientes, function (resp) {
            allPendientes = (resp && resp.data) ? resp.data : [];
            render();
        }).fail(function () {
            allPendientes = []; render();
        });
    }

    obj.init = function (urlP, urlPend, fechaDefault) {
        urlPagos      = urlP;
        urlPendientes = urlPend;
        if (!$("#f-fecha").val()) $("#f-fecha").val(fechaDefault);

        $("#btn-buscar").on("click", cargar);
        $("#f-fecha").on("change", cargar);
        $("#f-buscar").on("input", render);
        cargarUltimaFechaConDatos(new Date(fechaDefault + "T00:00:00"), 0);
    };

    return obj;
}());
