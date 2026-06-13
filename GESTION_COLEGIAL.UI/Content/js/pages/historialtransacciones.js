var HistorialTransacciones = (function () {
    var obj = {};
    var listUrl;
    var allRows = [];

    var COLORS = ["#4f46e5", "#10b981", "#f59e0b", "#06b6d4", "#8b5cf6", "#f43f5e", "#22d3ee", "#fb923c"];

    function fmt(v) {
        var val = parseFloat(v) || 0;
        return "L. " + val.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }
    function fmtShort(v) {
        var n = parseFloat(v) || 0;
        if (n >= 1000000) return "L. " + (n / 1000000).toFixed(1) + "M";
        if (n >= 1000)    return "L. " + (n / 1000).toFixed(1) + "K";
        return "L. " + n.toFixed(0);
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
        var p = ["", "av-emerald", "av-amber", "av-violet", "av-rose"];
        return p[i % p.length];
    }
    function parseDate(s) {
        if (!s) return null;
        var d = new Date(s);
        return isNaN(d.getTime()) ? null : d;
    }
    function dayKey(d) { return d.toISOString().substring(0, 10); }
    function dayLabel(s) {
        var d = new Date(s + "T00:00:00");
        if (isNaN(d.getTime())) return s;
        return d.toLocaleDateString("es-HN", { day: "2-digit", month: "short" });
    }

    function render(rows) {
        var total = 0, maximo = 0, mayorAlumno = "";
        var formas = {}, cajeros = {}, alumnos = {}, dias = {};

        rows.forEach(function (r) {
            var m = parseFloat(r.Pag_MontoTotal) || 0;
            total += m;
            if (m > maximo) { maximo = m; mayorAlumno = r.Alu_NombreCompleto || ""; }

            var f = r.Fpa_Descripcion || "Sin definir";
            if (!formas[f]) formas[f] = { c: 0, v: 0 };
            formas[f].c++; formas[f].v += m;

            var cj = r.CajeroNombre || "Sin asignar";
            if (!cajeros[cj]) cajeros[cj] = { c: 0, v: 0 };
            cajeros[cj].c++; cajeros[cj].v += m;

            if (r.Alu_NombreCompleto) alumnos[r.Alu_NombreCompleto] = true;

            var d = parseDate(r.Pag_FechaPago);
            if (d) {
                var k = dayKey(d);
                if (!dias[k]) dias[k] = { c: 0, v: 0 };
                dias[k].c++; dias[k].v += m;
            }
        });

        var count = rows.length;
        var promedio = count > 0 ? total / count : 0;

        // Día más activo
        var diasKeys = Object.keys(dias).sort();
        var topDia = null, topDiaV = 0;
        diasKeys.forEach(function (k) {
            if (dias[k].v > topDiaV) { topDiaV = dias[k].v; topDia = k; }
        });
        var totalDias = diasKeys.length;
        var promDia = totalDias > 0 ? total / totalDias : 0;

        // Hero
        $("#hero-total").text(fmt(total));
        var rangoDesde = $("#input-fecha-desde").val();
        var rangoHasta = $("#input-fecha-hasta").val();
        $("#hero-rango").text((rangoDesde || "—") + "  →  " + (rangoHasta || "—") + " · " + totalDias + (totalDias === 1 ? " día con actividad" : " días con actividad"));
        $("#hero-trans").text(count);
        $("#hero-trans-sub").text(Object.keys(alumnos).length + " alumnos");
        $("#hero-dia").text(topDia ? dayLabel(topDia) : "—");
        $("#hero-dia-sub").text(topDia ? fmt(topDiaV) : "Sin datos");
        $("#hero-prom-dia").text(fmt(promDia));
        $("#hero-prom-sub").text("sobre " + totalDias + " días");

        // KPIs
        $("#kpi-mayor").text(fmt(maximo));
        $("#kpi-mayor-sub").text(mayorAlumno || "—");
        $("#kpi-prom").text(fmt(promedio));
        $("#kpi-alumnos").text(Object.keys(alumnos).length);
        $("#kpi-alumnos-sub").text("hicieron pagos");
        $("#kpi-cajeros").text(Object.keys(cajeros).length);
        $("#kpi-cajeros-sub").text("operadores activos");
        $("#chip-count").text(count + (count === 1 ? " transacción" : " transacciones"));

        // Cols diarios
        var colsWrap = $("#cols-dia").empty();
        var maxDia = 0;
        diasKeys.forEach(function (k) { if (dias[k].v > maxDia) maxDia = dias[k].v; });
        if (diasKeys.length === 0) {
            colsWrap.html('<div class="fd-empty"><i class="fa-regular fa-calendar"></i><p>Sin movimientos en el rango</p></div>');
        } else {
            var maxCols = 30;
            var visibles = diasKeys.length > maxCols ? diasKeys.slice(-maxCols) : diasKeys;
            visibles.forEach(function (k) {
                var v = dias[k].v;
                var pct = maxDia > 0 ? (v / maxDia * 100) : 0;
                colsWrap.append(
                    '<div class="fd-col">' +
                        '<div class="fd-col-bar" style="height:' + pct.toFixed(1) + '%" data-val="' + esc(fmt(v)) + '"></div>' +
                        '<span class="fd-col-label">' + esc(dayLabel(k)) + '</span>' +
                    '</div>'
                );
            });
        }

        // Donut formas
        var formasArr = Object.keys(formas).map(function (k) { return { name: k, c: formas[k].c, v: formas[k].v }; });
        formasArr.sort(function (a, b) { return b.c - a.c; });
        $("#donut-num").text(count);

        var donut = $("#donut-formas");
        if (count === 0 || formasArr.length === 0) {
            donut.css("background", "conic-gradient(#e2e8f0 0% 100%)");
            $("#donut-legend").html('<div class="fd-empty" style="padding:1rem 0;"><i class="fa-solid fa-credit-card"></i><p>Sin datos</p></div>');
        } else {
            var acc = 0;
            var stops = formasArr.map(function (it, i) {
                var from = (acc / count) * 100;
                acc += it.c;
                var to = (acc / count) * 100;
                return COLORS[i % COLORS.length] + " " + from.toFixed(2) + "% " + to.toFixed(2) + "%";
            });
            donut.css("background", "conic-gradient(" + stops.join(",") + ")");

            var legend = $("#donut-legend").empty();
            formasArr.forEach(function (it, i) {
                var pct = (it.c / count * 100).toFixed(1);
                legend.append(
                    '<div class="fd-legend-item">' +
                        '<span class="fd-legend-dot" style="background:' + COLORS[i % COLORS.length] + '"></span>' +
                        '<span>' + esc(it.name) + ' <small style="color:#94a3b8">(' + it.c + ')</small></span>' +
                        '<span class="fd-legend-val">' + pct + '%</span>' +
                    '</div>'
                );
            });
        }

        // Top cajeros
        var cajArr = Object.keys(cajeros).map(function (k) { return { name: k, c: cajeros[k].c, v: cajeros[k].v }; });
        cajArr.sort(function (a, b) { return b.v - a.v; });
        cajArr = cajArr.slice(0, 6);
        var rank = $("#rank-cajeros").empty();
        if (cajArr.length === 0) {
            rank.html('<div class="fd-empty"><i class="fa-solid fa-user-tie"></i><p>Sin actividad</p></div>');
        } else {
            cajArr.forEach(function (c, i) {
                rank.append(
                    '<div class="fd-rank-item">' +
                        '<div class="fd-rank-num">' + (i + 1) + '</div>' +
                        '<div>' +
                            '<p class="fd-rank-name">' + esc(c.name) + '</p>' +
                            '<p class="fd-rank-sub">' + c.c + (c.c === 1 ? ' operación' : ' operaciones') + '</p>' +
                        '</div>' +
                        '<div class="fd-rank-amt" style="color:#10b981">' + esc(fmtShort(c.v)) + '</div>' +
                    '</div>'
                );
            });
        }

        // Feed
        var sorted = rows.slice().sort(function (a, b) {
            var da = parseDate(a.Pag_FechaPago), db = parseDate(b.Pag_FechaPago);
            return (db ? db.getTime() : 0) - (da ? da.getTime() : 0);
        });
        var feed = $("#feed-trans").empty();
        if (sorted.length === 0) {
            feed.html('<div class="fd-empty"><i class="fa-solid fa-inbox"></i><p>Sin transacciones para los filtros aplicados</p></div>');
        } else {
            sorted.forEach(function (r, idx) {
                var d = parseDate(r.Pag_FechaPago);
                var fecha = d ? d.toLocaleDateString("es-HN", { day: "2-digit", month: "short", year: "numeric" }) : "—";
                var hora  = d ? d.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" }) : "";
                var cant  = parseInt(r.CantidadConceptos) || 0;
                feed.append(
                    '<div class="fd-feed-item">' +
                        '<div class="fd-feed-avatar ' + avatarPalette(idx) + '">' + esc(initials(r.Alu_NombreCompleto)) + '</div>' +
                        '<div class="fd-feed-body">' +
                            '<p class="fd-feed-title">' + esc(r.Alu_NombreCompleto || "—") + '</p>' +
                            '<p class="fd-feed-meta">' +
                                '<span><i class="fa-regular fa-calendar"></i>' + esc(fecha) + (hora ? ' · ' + esc(hora) : '') + '</span>' +
                                '<span><i class="fa-solid fa-tag"></i>' + cant + (cant === 1 ? ' concepto' : ' conceptos') + '</span>' +
                                '<span><i class="fa-solid fa-credit-card"></i>' + esc(r.Fpa_Descripcion || "—") + '</span>' +
                                '<span><i class="fa-solid fa-user-tie"></i>' + esc(r.CajeroNombre || "—") + '</span>' +
                            '</p>' +
                        '</div>' +
                        '<div class="fd-feed-right">' +
                            '<p class="fd-feed-amount">' + esc(fmt(r.Pag_MontoTotal)) + '</p>' +
                            '<span class="fd-chip c-indigo">Transacción</span>' +
                        '</div>' +
                    '</div>'
                );
            });
        }
    }

    function aplicarFiltro() {
        var q = ($("#input-alumno").val() || "").toLowerCase().trim();
        if (!q) return render(allRows);
        render(allRows.filter(function (r) {
            return (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0;
        }));
    }

    function cargar() {
        $.get(listUrl, {
            aluId: null,
            fechaDesde: $("#input-fecha-desde").val(),
            fechaHasta: $("#input-fecha-hasta").val()
        }, function (resp) {
            allRows = (resp && resp.data) ? resp.data : [];
            aplicarFiltro();
        }).fail(function () {
            allRows = [];
            render([]);
        });
    }

    obj.init = function (url, fechaDesdeDefault, fechaHastaDefault) {
        listUrl = url;
        if (!$("#input-fecha-desde").val()) $("#input-fecha-desde").val(fechaDesdeDefault);
        if (!$("#input-fecha-hasta").val()) $("#input-fecha-hasta").val(fechaHastaDefault);
        $("#btn-buscar").on("click", cargar);
        $("#input-fecha-desde, #input-fecha-hasta").on("change", cargar);
        $("#input-alumno").on("input", aplicarFiltro);
        cargar();
    };

    return obj;
}());
