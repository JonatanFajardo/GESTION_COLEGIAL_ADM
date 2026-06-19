var DevTools = (function () {
    var obj = {}, cfg = {}, currentDelta = 0;

    // Tablas en el orden que se procesarán
    var TABLAS = [
        "finanza.tbPagos",
        "finanza.tbCuentasCobrar",
        "finanza.tbArqueosCaja",
        "finanza.tbMoratorias",
        "finanza.tbRecibos",
        "finanza.tbCuentasCobrarHistorial",
        "finanza.tbPagosHistorial",
        "app.tbAsistencia",
        "app.tbAsistenciaEmpleados",
        "app.tbNotas",
        "app.tbTareas",
        "app.tbDocumentosAlumno",
        "app.tbVacaciones",
        "app.tbAnuncios",
        "app.tbMensajes",
        "app.tbNotificaciones",
        "bitacoras.tbAlumnosHistorial",
        "app.tbAlumnos"
    ];

    obj.init = function (config) {
        cfg = config;
        renderSteps();
        bind();
        detectar();
        estadoBD();
        cargarRoles();
    };

    // -------------------- SINCRONIZADOR --------------------
    function renderSteps() {
        var html = TABLAS.map(function (t) {
            return '<div class="step-row pending" data-tabla="' + t + '">' +
                   '<span class="icon"><i class="fa-regular fa-circle"></i></span>' +
                   '<span class="name">' + t + '</span>' +
                   '<span class="meta"></span>' +
                   '</div>';
        }).join("");
        $("#sync-steps").html(html);
        $("#sync-progress").css("width", "0%");
        $("#sync-progress-text").text("0 de " + TABLAS.length);
        $("#sync-progress-percent").text("0%");
    }

    // Lee un campo de un objeto sin importar el casing (PascalCase, camelCase, etc.)
    function get(obj, key) {
        if (!obj || obj[key] !== undefined) return obj ? obj[key] : undefined;
        var lower = key.charAt(0).toLowerCase() + key.slice(1);
        if (obj[lower] !== undefined) return obj[lower];
        var upper = key.charAt(0).toUpperCase() + key.slice(1);
        if (obj[upper] !== undefined) return obj[upper];
        // Búsqueda case-insensitive last resort
        var keys = Object.keys(obj);
        for (var i = 0; i < keys.length; i++) {
            if (keys[i].toLowerCase() === key.toLowerCase()) return obj[keys[i]];
        }
        return undefined;
    }

    function detectar() {
        $("#sync-status").html('<span class="text-muted small"><i class="fa-solid fa-spinner fa-spin"></i> detectando...</span>');
        $.ajax({ url: cfg.urlDetectar, type: "GET" })
            .done(function (res) {
                console.log("[DevTools] detectar response:", res);
                var d = (res.data && res.data[0]) || {};
                var anioActual   = get(d, "AnioActual");
                var anioObjetivo = get(d, "AnioObjetivo");
                var delta        = get(d, "Delta");
                var sincronizado = get(d, "Sincronizado");

                $("#sync-anio-actual").text(anioActual != null ? anioActual : "—");
                $("#sync-anio-objetivo").text(anioObjetivo != null ? anioObjetivo : "—");
                $("#sync-delta").html(delta === 0
                    ? '<span class="badge-sync">✓ sincronizado</span>'
                    : (delta != null
                        ? '<span class="badge-delta">' + (delta > 0 ? "+" : "") + delta + ' año(s)</span>'
                        : '<span class="badge-delta">undefined año(s)</span>'));
                currentDelta = delta || 0;
                $("#btn-sync-run").prop("disabled", !!sincronizado || currentDelta === 0);
                $("#sync-status").html(sincronizado
                    ? '<span class="badge-sync"><i class="fa-solid fa-check"></i> al día</span>'
                    : "");
            })
            .fail(function (xhr) {
                console.error("[DevTools] detectar failed:", xhr.status, xhr.responseText);
                $("#sync-status").html('<span class="text-danger small">error al detectar (' + xhr.status + ')</span>');
            });
    }

    function runSync() {
        if (currentDelta === 0) return;
        $("#btn-sync-detectar, #btn-sync-run").prop("disabled", true);
        renderSteps();
        var totalRows = 0, totalMs = 0, i = 0;
        var startAll = Date.now();

        function next() {
            if (i >= TABLAS.length) {
                finalize(totalRows, Date.now() - startAll);
                return;
            }
            var tabla = TABLAS[i];
            var $row = $('.step-row[data-tabla="' + tabla + '"]');
            $row.removeClass("pending").addClass("running");
            $row.find(".icon").html('<i class="fa-solid fa-spinner fa-spin"></i>');
            $row.find(".meta").text("ejecutando...");

            $.ajax({
                url: cfg.urlPaso + "?Tabla=" + encodeURIComponent(tabla) + "&Delta=" + currentDelta,
                type: "POST"
            })
            .done(function (res) {
                var r = (res.data && res.data[0]) || {};
                var rows = get(r, "RowsAffected") || 0;
                var ms   = get(r, "DurationMs") || 0;
                $row.removeClass("running").addClass("completed");
                $row.find(".icon").html('<i class="fa-solid fa-circle-check"></i>');
                animateRows($row.find(".meta"), rows, ms);
                totalRows += rows;
                totalMs += ms;
                advanceProgress(i + 1);
                i++;
                setTimeout(next, 120);
            })
            .fail(function (xhr) {
                $row.removeClass("running").addClass("error");
                $row.find(".icon").html('<i class="fa-solid fa-circle-xmark"></i>');
                $row.find(".meta").text("error " + xhr.status);
                i++;
                setTimeout(next, 120);
            });
        }
        next();
    }

    function advanceProgress(done) {
        var pct = Math.round((done / TABLAS.length) * 100);
        $("#sync-progress").css("width", pct + "%");
        $("#sync-progress-text").text(done + " de " + TABLAS.length);
        $("#sync-progress-percent").text(pct + "%");
    }

    function animateRows($el, target, ms) {
        var start = 0, duration = 600, t0 = performance.now();
        function step(t) {
            var p = Math.min(1, (t - t0) / duration);
            var val = Math.floor(start + (target - start) * easeOutCubic(p));
            $el.text(val.toLocaleString() + " filas · " + ms + "ms");
            if (p < 1) requestAnimationFrame(step);
        }
        if (target === 0) { $el.text("0 filas · " + ms + "ms"); return; }
        requestAnimationFrame(step);
    }

    function easeOutCubic(t) { return 1 - Math.pow(1 - t, 3); }

    function finalize(totalRows, totalMs) {
        $("#sync-status").html(
            '<span class="badge-sync"><i class="fa-solid fa-sparkles"></i> ' +
            totalRows.toLocaleString() + ' filas · ' + (totalMs / 1000).toFixed(2) + 's</span>'
        );
        $("#card-sync").addClass("pulse-success");
        setTimeout(function () { $("#card-sync").removeClass("pulse-success"); }, 1000);
        $("#btn-sync-detectar").prop("disabled", false);
        setTimeout(function () { detectar(); estadoBD(); }, 600);
    }

    // -------------------- ESTADO BD --------------------
    function estadoBD() {
        $("#estado-grid [data-stat]").text("…");
        $.ajax({ url: cfg.urlEstado, type: "GET" }).done(function (res) {
            console.log("[DevTools] estadoBD response:", res);
            var d = (res.data && res.data[0]) || {};
            $("#estado-grid [data-stat]").each(function () {
                var k = $(this).data("stat");
                var v = get(d, k);
                if (v == null) { $(this).text("—"); return; }
                if (k === "MontoPagadoAnioActual") v = "L. " + parseFloat(v || 0).toFixed(2);
                else if (typeof v === "number") v = v.toLocaleString();
                $(this).text(v);
            });
        });
    }

    // -------------------- IMPERSONAR ROL --------------------
    function cargarRoles() {
        $.ajax({ url: cfg.urlRoles, type: "GET" }).done(function (res) {
            console.log("[DevTools] roles response:", res);
            var roles = res.data || [];
            var $sel = $("#imp-rol").empty();
            $sel.append('<option value="">-- selecciona un rol --</option>');
            roles.forEach(function (r) {
                var id   = get(r, "Rol_Id");
                var desc = get(r, "Rol_Descripcion");
                var cant = get(r, "CantidadPantallas");
                $sel.append('<option value="' + id + '">' + desc + ' (' + cant + ' pantallas)</option>');
            });
        });
    }

    function impersonar() {
        var rolId = $("#imp-rol").val();
        if (!rolId) { alert("Selecciona un rol"); return; }

        $("#btn-imp-cargar, #btn-imp-restaurar").prop("disabled", true);
        $("#imp-status").show().removeClass("alert-info alert-warning alert-danger").addClass("alert-info")
            .html('<i class="fa-solid fa-spinner fa-spin"></i> Aplicando impersonación...');

        $.ajax({ url: cfg.urlImpersonar, type: "POST", data: { rolId: rolId } })
            .done(function (res) {
                if (res && res.success) {
                    window.location.reload();
                } else {
                    $("#imp-status").removeClass("alert-info").addClass("alert-danger")
                        .html('<i class="fa-solid fa-triangle-exclamation"></i> Error al impersonar el rol.');
                    $("#btn-imp-cargar, #btn-imp-restaurar").prop("disabled", false);
                }
            })
            .fail(function () {
                $("#imp-status").removeClass("alert-info").addClass("alert-danger")
                    .html('<i class="fa-solid fa-triangle-exclamation"></i> Error de comunicación.');
                $("#btn-imp-cargar, #btn-imp-restaurar").prop("disabled", false);
            });
    }

    function restaurar() {
        $("#btn-imp-cargar, #btn-imp-restaurar").prop("disabled", true);
        $("#imp-status").show().removeClass("alert-info alert-warning alert-danger").addClass("alert-info")
            .html('<i class="fa-solid fa-spinner fa-spin"></i> Restaurando sesión admin...');

        $.ajax({ url: cfg.urlRestaurar, type: "POST" })
            .done(function () { window.location.reload(); })
            .fail(function () {
                $("#imp-status").removeClass("alert-info").addClass("alert-danger")
                    .html('<i class="fa-solid fa-triangle-exclamation"></i> Error al restaurar.');
                $("#btn-imp-cargar, #btn-imp-restaurar").prop("disabled", false);
            });
    }

    // -------------------- BIND --------------------
    function bind() {
        $("#btn-sync-detectar").on("click", detectar);
        $("#btn-sync-run").on("click", runSync);
        $("#btn-estado-refresh").on("click", estadoBD);
        $("#btn-imp-cargar").on("click", impersonar);
        $("#btn-imp-restaurar").on("click", restaurar);
    }

    return obj;
}());
