var PagosDelDia = (function () {
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
        var a = p[0] ? p[0][0] : "";
        var b = p.length > 1 ? p[p.length - 1][0] : "";
        return (a + b).toUpperCase();
    }

    function avatarPalette(i) {
        var p = ["", "av-rose", "av-emerald", "av-amber", "av-violet"];
        return p[i % p.length];
    }

    function parseHora(s) {
        if (!s) return null;
        var d = new Date(s);
        return isNaN(d.getTime()) ? null : d;
    }

    function fechaLarga(s) {
        if (!s) return "—";
        var d = new Date(s + "T00:00:00");
        if (isNaN(d.getTime())) return s;
        return d.toLocaleDateString("es-HN", {
            weekday: "long", year: "numeric", month: "long", day: "numeric"
        });
    }

    function render(rows) {
        var total = 0, maxPago = 0, maxAlumno = "";
        var formaTot = {}, conceptoTot = {}, alumnosUnicos = {};
        var horaCount = new Array(24).fill(0);
        var horaTot   = new Array(24).fill(0);

        rows.forEach(function (r) {
            var m = parseFloat(r.Pag_MontoTotal) || 0;
            total += m;
            if (m > maxPago) { maxPago = m; maxAlumno = r.Alu_NombreCompleto || ""; }

            var f = r.Fpa_Descripcion || "Sin definir";
            formaTot[f] = (formaTot[f] || 0) + m;

            var c = r.ConceptosCobrados || "Sin concepto";
            conceptoTot[c] = (conceptoTot[c] || 0) + m;

            if (r.Alu_NombreCompleto) alumnosUnicos[r.Alu_NombreCompleto] = true;

            var d = parseHora(r.Pag_FechaPago);
            if (d) {
                var h = d.getHours();
                horaCount[h] += 1;
                horaTot[h]   += m;
            }
        });

        var count = rows.length;
        var promedio = count > 0 ? total / count : 0;

        // Hora pico
        var picoH = -1, picoV = 0;
        for (var i = 0; i < 24; i++) {
            if (horaTot[i] > picoV) { picoV = horaTot[i]; picoH = i; }
        }

        // Top forma
        var topForma = "—", topFormaV = 0;
        Object.keys(formaTot).forEach(function (k) {
            if (formaTot[k] > topFormaV) { topFormaV = formaTot[k]; topForma = k; }
        });

        // Top concepto
        var topConcepto = "—", topConceptoV = 0;
        Object.keys(conceptoTot).forEach(function (k) {
            if (conceptoTot[k] > topConceptoV) { topConceptoV = conceptoTot[k]; topConcepto = k; }
        });

        // Hero
        $("#hero-total").text(fmt(total));
        $("#hero-fecha").text(fechaLarga($("#input-fecha").val()));
        $("#hero-trans").text(count);
        $("#hero-trans-sub").text(count === 1 ? "1 operación registrada" : count + " operaciones registradas");
        $("#hero-prom").text(fmt(promedio));
        $("#hero-hora").text(picoH >= 0 ? (String(picoH).padStart(2, "0") + ":00") : "—");
        $("#hero-hora-sub").text(picoH >= 0 ? fmt(picoV) + " cobrados" : "Sin movimientos");

        // KPIs
        $("#kpi-mayor").text(fmt(maxPago));
        $("#kpi-mayor-sub").text(maxAlumno || "—");
        $("#kpi-alumnos").text(Object.keys(alumnosUnicos).length);
        $("#kpi-forma").text(topForma);
        $("#kpi-forma-sub").text(topFormaV > 0 ? fmt(topFormaV) + " procesados" : "—");
        $("#kpi-concepto").text(topConcepto.length > 22 ? topConcepto.substring(0, 22) + "…" : topConcepto);
        $("#kpi-concepto-sub").text(topConceptoV > 0 ? fmt(topConceptoV) : "—");
        $("#chip-pico").text(picoH >= 0 ? "Pico " + String(picoH).padStart(2, "0") + ":00" : "Sin datos");
        $("#chip-count").text(count + (count === 1 ? " movimiento" : " movimientos"));

        // Cols hora
        var horaWrap = $("#cols-hora").empty();
        var horaMax = Math.max.apply(null, horaTot);
        if (horaMax === 0) {
            horaWrap.html('<div class="fd-empty"><i class="fa-regular fa-clock"></i><p>Sin operaciones registradas en el rango</p></div>');
        } else {
            for (var h = 6; h <= 20; h++) {
                var v = horaTot[h];
                var pct = horaMax > 0 ? (v / horaMax * 100) : 0;
                horaWrap.append(
                    '<div class="fd-col">' +
                        '<div class="fd-col-bar" style="height:' + pct.toFixed(1) + '%" data-val="' + esc(fmt(v)) + '"></div>' +
                        '<span class="fd-col-label">' + String(h).padStart(2, "0") + '</span>' +
                    '</div>'
                );
            }
        }

        // Bars formas
        var barClasses = ["", "b-emerald", "b-amber", "b-cyan", "b-violet", "b-rose"];
        var formasArr = Object.keys(formaTot).map(function (k) { return { name: k, val: formaTot[k] }; });
        formasArr.sort(function (a, b) { return b.val - a.val; });
        var barsForma = $("#bars-formas").empty();
        if (formasArr.length === 0) {
            barsForma.html('<div class="fd-empty"><i class="fa-solid fa-credit-card"></i><p>Sin pagos para analizar</p></div>');
        } else {
            formasArr.forEach(function (it, idx) {
                var pct = total > 0 ? (it.val / total * 100) : 0;
                barsForma.append(
                    '<div class="fd-bar-item">' +
                        '<div class="fd-bar-top"><span class="fd-bar-name">' + esc(it.name) + '</span>' +
                        '<span class="fd-bar-val">' + esc(fmt(it.val)) + ' · ' + pct.toFixed(1) + '%</span></div>' +
                        '<div class="fd-bar-track"><div class="fd-bar-fill ' + barClasses[idx % barClasses.length] + '" style="width:' + pct.toFixed(1) + '%"></div></div>' +
                    '</div>'
                );
            });
        }

        // Bars conceptos
        var concArr = Object.keys(conceptoTot).map(function (k) { return { name: k, val: conceptoTot[k] }; });
        concArr.sort(function (a, b) { return b.val - a.val; });
        concArr = concArr.slice(0, 6);
        var barsConc = $("#bars-conceptos").empty();
        if (concArr.length === 0) {
            barsConc.html('<div class="fd-empty"><i class="fa-solid fa-tags"></i><p>Sin conceptos para analizar</p></div>');
        } else {
            concArr.forEach(function (it, idx) {
                var pct = total > 0 ? (it.val / total * 100) : 0;
                barsConc.append(
                    '<div class="fd-bar-item">' +
                        '<div class="fd-bar-top"><span class="fd-bar-name">' + esc(it.name) + '</span>' +
                        '<span class="fd-bar-val">' + esc(fmt(it.val)) + '</span></div>' +
                        '<div class="fd-bar-track"><div class="fd-bar-fill ' + barClasses[idx % barClasses.length] + '" style="width:' + pct.toFixed(1) + '%"></div></div>' +
                    '</div>'
                );
            });
        }

        // Feed
        var feed = $("#feed-pagos").empty();
        if (count === 0) {
            feed.html('<div class="fd-empty"><i class="fa-solid fa-inbox"></i><p>No se registraron pagos en esta fecha</p></div>');
        } else {
            var sorted = rows.slice().sort(function (a, b) {
                var da = parseHora(a.Pag_FechaPago), db = parseHora(b.Pag_FechaPago);
                return (db ? db.getTime() : 0) - (da ? da.getTime() : 0);
            });
            sorted.forEach(function (r, idx) {
                var d = parseHora(r.Pag_FechaPago);
                var hora = d ? d.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" }) : "—";
                var nombre = r.Alu_NombreCompleto || "—";
                feed.append(
                    '<div class="fd-feed-item">' +
                        '<div class="fd-feed-avatar ' + avatarPalette(idx) + '">' + esc(initials(nombre)) + '</div>' +
                        '<div class="fd-feed-body">' +
                            '<p class="fd-feed-title">' + esc(nombre) + '</p>' +
                            '<p class="fd-feed-meta">' +
                                '<span><i class="fa-solid fa-tag"></i>' + esc(r.ConceptosCobrados || "Sin concepto") + '</span>' +
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
    }

    function aplicarFiltro() {
        var q = ($("#input-buscador").val() || "").toLowerCase().trim();
        if (!q) return render(allRows);
        render(allRows.filter(function (r) {
            return (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0
                || (r.ConceptosCobrados || "").toLowerCase().indexOf(q) >= 0
                || (r.Fpa_Descripcion || "").toLowerCase().indexOf(q) >= 0;
        }));
    }

    function cargar() {
        $.get(listUrl, { fecha: $("#input-fecha").val() }, function (resp) {
            allRows = (resp && resp.data) ? resp.data : [];
            aplicarFiltro();
        }).fail(function () {
            allRows = [];
            render([]);
        });
    }

    function cargarUltimaFechaConDatos(fecha, intentos) {
        if (intentos > 60) { aplicarFiltro(); return; }
        var fechaStr = fecha.toISOString().split("T")[0];
        $.get(listUrl, { fecha: fechaStr }, function (resp) {
            allRows = (resp && resp.data) ? resp.data : [];
            if (allRows.length === 0) {
                var anterior = new Date(fecha);
                anterior.setDate(anterior.getDate() - 1);
                cargarUltimaFechaConDatos(anterior, intentos + 1);
            } else {
                $("#input-fecha").val(fechaStr);
                aplicarFiltro();
            }
        }).fail(function () {
            allRows = [];
            render([]);
        });
    }

    obj.init = function (url, fechaDefault) {
        listUrl = url;
        if (!$("#input-fecha").val()) $("#input-fecha").val(fechaDefault);
        $("#btn-buscar").on("click", cargar);
        $("#input-fecha").on("change", cargar);
        $("#input-buscador").on("input", aplicarFiltro);
        cargarUltimaFechaConDatos(new Date(fechaDefault + "T00:00:00"), 0);
    };

    return obj;
}());
