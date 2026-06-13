var PendientesCobrar = (function () {
    var obj = {};
    var listUrl;
    var allRows = [];

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
    function avatarPalette(i) {
        var p = ["av-rose", "av-amber", "av-violet", "av-emerald", ""];
        return p[i % p.length];
    }

    function bucket(dias) {
        if (dias <= 0)    return 0;   // por vencer / al día
        if (dias <= 30)   return 1;
        if (dias <= 60)   return 2;
        if (dias <= 90)   return 3;
        return 4;
    }

    var BUCKETS = [
        { label: "Al día / por vencer", cls: "" },
        { label: "1 - 30 días",         cls: "a-31" },
        { label: "31 - 60 días",        cls: "a-61" },
        { label: "61 - 90 días",        cls: "a-91" },
        { label: "+90 días",            cls: "a-120" }
    ];

    function render(rows) {
        var total = 0, totalVencido = 0, sumDias = 0;
        var vencidas = 0, hoy = 0, prox7 = 0;
        var conceptos = {}, alumnos = {}, deudores = {};
        var bk = [{c:0,v:0},{c:0,v:0},{c:0,v:0},{c:0,v:0},{c:0,v:0}];

        rows.forEach(function (r) {
            var pend = parseFloat(r.Cco_MontoPendiente) || 0;
            total += pend;
            var dias = parseInt(r.DiasVencida);
            if (isNaN(dias)) dias = 0;

            if (dias > 0) { vencidas++; totalVencido += pend; sumDias += dias; }
            if (dias === 0) hoy++;
            if (dias < 0 && dias >= -7) prox7++;

            var bi = bucket(dias);
            bk[bi].c++; bk[bi].v += pend;

            var c = r.Cpa_Descripcion || "Sin concepto";
            conceptos[c] = (conceptos[c] || 0) + pend;

            var nombre = r.Alu_NombreCompleto || "—";
            alumnos[nombre] = true;
            if (!deudores[nombre]) deudores[nombre] = { name: nombre, total: 0, cuentas: 0, peor: -9999 };
            deudores[nombre].total += pend;
            deudores[nombre].cuentas += 1;
            if (dias > deudores[nombre].peor) deudores[nombre].peor = dias;
        });

        var count = rows.length;
        var promedio = count > 0 ? total / count : 0;
        var antiMedia = vencidas > 0 ? Math.round(sumDias / vencidas) : 0;

        // Hero
        $("#hero-total").text(fmt(total));
        $("#hero-sub").text(count + (count === 1 ? " cuenta abierta" : " cuentas abiertas") + " · " + fmt(totalVencido) + " vencidos");
        $("#hero-cuentas").text(count);
        $("#hero-cuentas-sub").text(Object.keys(alumnos).length + " alumnos");
        $("#hero-prom").text(fmt(promedio));
        $("#hero-anti").text(antiMedia + (antiMedia === 1 ? " día" : " días"));

        // KPIs
        $("#kpi-vencidas").text(vencidas);
        $("#kpi-vencidas-sub").text(fmt(totalVencido));
        $("#kpi-hoy").text(hoy);
        $("#kpi-hoy-sub").text(hoy === 0 ? "Sin vencimientos hoy" : "requieren gestión");
        $("#kpi-prox").text(prox7);
        $("#kpi-prox-sub").text("dentro de 7 días");
        $("#kpi-alumnos").text(Object.keys(alumnos).length);
        $("#kpi-alumnos-sub").text("con saldo abierto");
        $("#chip-criticas").text(bk[4].c + " cuentas críticas (+90 d)");
        $("#chip-count").text(count + (count === 1 ? " cuenta" : " cuentas"));

        // Aging buckets
        var agingWrap = $("#aging-row").empty();
        BUCKETS.forEach(function (b, i) {
            agingWrap.append(
                '<div class="fd-aging ' + b.cls + '">' +
                    '<p class="fd-aging-label">' + esc(b.label) + '</p>' +
                    '<p class="fd-aging-amount">' + esc(fmt(bk[i].v)) + '</p>' +
                    '<p class="fd-aging-count">' + bk[i].c + (bk[i].c === 1 ? ' cuenta' : ' cuentas') + '</p>' +
                '</div>'
            );
        });

        // Top deudores
        var arr = Object.keys(deudores).map(function (k) { return deudores[k]; });
        arr.sort(function (a, b) { return b.total - a.total; });
        arr = arr.slice(0, 8);
        var rank = $("#rank-deudores").empty();
        if (arr.length === 0) {
            rank.html('<div class="fd-empty"><i class="fa-solid fa-user-group"></i><p>Sin deudores registrados</p></div>');
        } else {
            arr.forEach(function (d, i) {
                rank.append(
                    '<div class="fd-rank-item">' +
                        '<div class="fd-rank-num">' + (i + 1) + '</div>' +
                        '<div>' +
                            '<p class="fd-rank-name">' + esc(d.name) + '</p>' +
                            '<p class="fd-rank-sub">' + d.cuentas + (d.cuentas === 1 ? ' cuenta' : ' cuentas') +
                            (d.peor > 0 ? ' · peor ' + d.peor + ' días vencido' : '') + '</p>' +
                        '</div>' +
                        '<div class="fd-rank-amt">' + esc(fmt(d.total)) + '</div>' +
                    '</div>'
                );
            });
        }

        // Conceptos bars
        var barClasses = ["b-rose", "b-amber", "b-cyan", "b-violet", "b-emerald", ""];
        var concArr = Object.keys(conceptos).map(function (k) { return { name: k, val: conceptos[k] }; });
        concArr.sort(function (a, b) { return b.val - a.val; });
        concArr = concArr.slice(0, 7);
        var bars = $("#bars-conceptos").empty();
        if (concArr.length === 0) {
            bars.html('<div class="fd-empty"><i class="fa-solid fa-tags"></i><p>Sin conceptos pendientes</p></div>');
        } else {
            concArr.forEach(function (it, idx) {
                var pct = total > 0 ? (it.val / total * 100) : 0;
                bars.append(
                    '<div class="fd-bar-item">' +
                        '<div class="fd-bar-top"><span class="fd-bar-name">' + esc(it.name) + '</span>' +
                        '<span class="fd-bar-val">' + esc(fmt(it.val)) + ' · ' + pct.toFixed(1) + '%</span></div>' +
                        '<div class="fd-bar-track"><div class="fd-bar-fill ' + barClasses[idx % barClasses.length] + '" style="width:' + pct.toFixed(1) + '%"></div></div>' +
                    '</div>'
                );
            });
        }

        // Feed
        var sorted = rows.slice().sort(function (a, b) {
            return (parseInt(b.DiasVencida) || 0) - (parseInt(a.DiasVencida) || 0);
        });
        var feed = $("#feed-cuentas").empty();
        if (sorted.length === 0) {
            feed.html('<div class="fd-empty"><i class="fa-solid fa-circle-check"></i><p>No hay cuentas pendientes con los filtros actuales</p></div>');
        } else {
            sorted.forEach(function (r, idx) {
                var dias = parseInt(r.DiasVencida);
                var chip, chipCls;
                if (isNaN(dias))      { chip = "Sin fecha";   chipCls = ""; }
                else if (dias > 30)   { chip = dias + " días vencido"; chipCls = "c-rose"; }
                else if (dias > 0)    { chip = dias + " días vencido"; chipCls = "c-amber"; }
                else if (dias === 0)  { chip = "Vence hoy";   chipCls = "c-amber"; }
                else                  { chip = Math.abs(dias) + " días por vencer"; chipCls = "c-cyan"; }

                var orig = parseFloat(r.Cco_MontoOriginal) || 0;
                var pag  = parseFloat(r.Cco_MontoTotal)    || 0;
                var pend = parseFloat(r.Cco_MontoPendiente) || 0;
                var pct  = orig > 0 ? Math.min(100, (pag / orig) * 100) : 0;

                feed.append(
                    '<div class="fd-feed-item">' +
                        '<div class="fd-feed-avatar ' + avatarPalette(idx) + '">' + esc(initials(r.Alu_NombreCompleto)) + '</div>' +
                        '<div class="fd-feed-body">' +
                            '<p class="fd-feed-title">' + esc(r.Alu_NombreCompleto || "—") + '</p>' +
                            '<p class="fd-feed-meta">' +
                                '<span><i class="fa-solid fa-tag"></i>' + esc(r.Cpa_Descripcion || "—") + '</span>' +
                                '<span><i class="fa-solid fa-coins"></i>' + esc(fmt(orig)) + ' original</span>' +
                                '<span><i class="fa-solid fa-circle-check"></i>' + esc(fmt(pag)) + ' pagado (' + pct.toFixed(0) + '%)</span>' +
                            '</p>' +
                        '</div>' +
                        '<div class="fd-feed-right">' +
                            '<p class="fd-feed-amount amt-rose">' + esc(fmt(pend)) + '</p>' +
                            '<span class="fd-chip ' + chipCls + '">' + esc(chip) + '</span>' +
                        '</div>' +
                    '</div>'
                );
            });
        }
    }

    function aplicarFiltro() {
        var q = ($("#input-buscador").val() || "").toLowerCase().trim();
        var estado = $("#select-estado").val();

        var out = allRows.filter(function (r) {
            if (q) {
                var hay = (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0
                       || (r.Cpa_Descripcion || "").toLowerCase().indexOf(q) >= 0;
                if (!hay) return false;
            }
            var dias = parseInt(r.DiasVencida);
            if (isNaN(dias)) dias = 0;
            if (estado === "vencido" && dias <= 0) return false;
            if (estado === "hoy"     && dias !== 0) return false;
            if (estado === "proximo" && !(dias < 0 && dias >= -7)) return false;
            return true;
        });
        render(out);
    }

    function cargar() {
        $.get(listUrl, function (resp) {
            allRows = (resp && resp.data) ? resp.data : [];
            aplicarFiltro();
        }).fail(function () {
            allRows = [];
            render([]);
        });
    }

    obj.init = function (url) {
        listUrl = url;
        $("#btn-refrescar").on("click", cargar);
        $("#input-buscador").on("input", aplicarFiltro);
        $("#select-estado").on("change", aplicarFiltro);
        cargar();
    };

    return obj;
}());
