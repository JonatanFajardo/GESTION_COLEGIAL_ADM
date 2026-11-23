/**
 * Centro de reportes: catálogo con buscador, formulario de parámetros (con la cascada
 * Año lectivo → Nivel → Grado → Sección), vista previa en GcTabla, Excel / PDF y "Recientes".
 * Todo lo que llega del servidor se escapa antes de pintarlo.
 */
(function () {
    "use strict";

    var datos = JSON.parse(document.getElementById("cr-catalogo").textContent || "{}");
    var REPORTES = {};
    (datos.reportes || []).forEach(function (r) { REPORTES[r.Clave] = r; });
    var MESES = datos.meses || [];
    var usuario = $("#cr-usuario").val() || "anonimo";

    var actual = null;      // reporte elegido
    var tabla = null;       // instancia GcTabla del último resultado
    var resultado = null;   // última respuesta del servidor
    var recientes = [];
    var CASCADA = ["anioLectivo", "nivel", "grado", "seccion"];

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    // ───────────── Catálogo y buscador ─────────────
    $("#cr-buscar").on("input", function () {
        var q = $(this).val().toString().trim().toLowerCase();
        var algun = false;
        $(".cr-area").each(function () {
            var visibles = 0;
            $(this).find(".cr-item").each(function () {
                var ok = !q || $(this).data("buscar").toString().indexOf(q) >= 0;
                $(this).toggleClass("d-none", !ok);
                if (ok) visibles++;
            });
            $(this).toggleClass("d-none", visibles === 0);
            if (visibles) algun = true;
        });
        $("#cr-sin-resultados").toggleClass("d-none", algun);
    });

    $(document).on("click", ".cr-item", function (e) {
        e.preventDefault();
        seleccionar($(this).data("clave"));
    });

    // ───────────── Formulario de parámetros ─────────────
    function idCampo(p) { return "cr-p-" + p.Nombre; }

    function campoHtml(p) {
        var id = idCampo(p), req = p.Requerido ? " required" : "";
        var etiqueta = '<label for="' + id + '">' + esc(p.Etiqueta) + (p.Requerido ? ' <span class="text-danger">*</span>' : "") + "</label>";
        var control;
        switch (p.Tipo) {
            case "anio":
            case "numero":
                control = '<input type="number" class="form-control" id="' + id + '" name="' + esc(p.Nombre) + '"' +
                    (p.Min !== null ? ' min="' + p.Min + '"' : "") + (p.Max !== null ? ' max="' + p.Max + '"' : "") + req + " />";
                break;
            case "fecha":
                control = '<input type="date" class="form-control" id="' + id + '" name="' + esc(p.Nombre) + '"' + req + " />";
                break;
            case "mes":
                control = '<select class="custom-select" id="' + id + '" name="' + esc(p.Nombre) + '"' + req + ">" +
                    MESES.map(function (m, i) { return '<option value="' + (i + 1) + '">' + esc(m) + "</option>"; }).join("") + "</select>";
                break;
            case "opcion":
                control = '<select class="custom-select" id="' + id + '" name="' + esc(p.Nombre) + '"' + req + ">" +
                    (p.Opciones || []).map(function (o) { return '<option value="' + esc(o.Valor) + '">' + esc(o.Texto) + "</option>"; }).join("") + "</select>";
                break;
            default: // listas que vienen del servidor
                control = '<select class="custom-select" id="' + id + '" name="' + esc(p.Nombre) + '" data-tipo="' + esc(p.Tipo) + '" disabled' + req + "></select>";
        }
        var ayuda = p.Ayuda ? '<small class="form-text text-muted">' + esc(p.Ayuda) + "</small>" : "";
        return '<div class="form-group col-md-6 col-xl-4">' + etiqueta + control + ayuda + "</div>";
    }

    function param(nombre) { return (actual.Parametros || []).filter(function (p) { return p.Nombre === nombre; })[0]; }

    /** Texto de la primera opción de una lista opcional ("Todos los niveles"…). */
    function textoTodos(p) {
        return { nivel: "Todos los niveles", grado: "Todos los grados", seccion: "Todas las secciones",
                 concepto: "Todos los conceptos", formaPago: "Todas las formas de pago" }[p.Tipo] || "Todos";
    }

    /** Llena una lista pidiendo sus opciones ya filtradas por el padre. Devuelve una promesa. */
    function cargarLista(p, extra) {
        var $s = $("#" + idCampo(p));
        $s.prop("disabled", true).html('<option value="">Cargando…</option>');
        var qs = $.extend({ tipo: p.Tipo, clave: actual.Clave }, extra || {});
        return $.getJSON("/CentroReportes/OpcionesAsync", qs).then(function (r) {
            var ops = (r && r.data) || [];
            var html = p.Requerido ? "" : '<option value="">' + esc(textoTodos(p)) + "</option>";
            html += ops.map(function (o) { return '<option value="' + esc(o.Valor) + '">' + esc(o.Texto) + "</option>"; }).join("");
            if (p.Requerido && !ops.length) html = '<option value="">Sin opciones</option>';
            $s.html(html).prop("disabled", ops.length === 0 && p.Requerido === true ? true : false);
        }, function () {
            $s.html('<option value="">No se pudo cargar</option>');
        });
    }

    /** Deja un hijo de la cascada vacío y bloqueado (hasta que su padre tenga valor). */
    function bloquear(nombre) {
        var p = param(nombre);
        if (!p) return;
        $("#" + idCampo(p)).html('<option value="">' + esc(textoTodos(p)) + "</option>").prop("disabled", true);
    }

    function valor(nombre) { var p = param(nombre); return p ? ($("#" + idCampo(p)).val() || "") : ""; }

    /** Al cambiar un eslabón de la cascada: limpia los de abajo y recarga solo el siguiente. */
    function alCambiarCascada(nombre) {
        var i = CASCADA.indexOf(nombre);
        CASCADA.slice(i + 1).forEach(bloquear);
        var sig = CASCADA[i + 1], p = param(sig);
        if (!p || !valor(nombre)) return $.Deferred().resolve().promise();
        var extra = { aniId: valor("anioLectivo") };
        if (sig === "grado") extra.nivId = valor("nivel");
        if (sig === "seccion") extra.cunId = valor("grado");
        return cargarLista(p, extra);
    }

    /** Pone los valores (por defecto o de un reciente), respetando la cascada paso a paso. */
    function aplicarValores(valores) {
        var promesas = [];
        (actual.Parametros || []).forEach(function (p) {
            if (CASCADA.indexOf(p.Tipo) >= 0) return;
            var v = valores[p.Nombre];
            var $c = $("#" + idCampo(p));
            if (p.Tipo === "concepto" || p.Tipo === "formaPago") {
                promesas.push(cargarLista(p).then(function () { $c.val(v || ""); }));
            } else if (v !== undefined && v !== null) {
                $c.val(v);
            }
        });
        if (param("anioLectivo")) {
            var cadena = cargarLista(param("anioLectivo"));
            CASCADA.forEach(function (nombre) {
                if (!param(nombre)) return;
                cadena = cadena.then(function () {
                    var v = valores[nombre];
                    var $c = $("#" + idCampo(param(nombre)));
                    if (v && $c.find('option[value="' + v.replace(/"/g, "") + '"]').length) $c.val(v);
                    return alCambiarCascada(nombre);
                });
            });
            promesas.push(cadena);
        }
        return $.when.apply($, promesas);
    }

    function defectos() {
        var v = {};
        (actual.Parametros || []).forEach(function (p) { if (p.Defecto) v[p.Nombre] = p.Defecto; });
        return v;
    }

    function seleccionar(clave, valores, generarAlFinal) {
        var r = REPORTES[clave];
        if (!r) return;
        actual = r;
        // Elegido = "active" de la plantilla; su texto e ícono pasan a blanco para que se lean sobre el color.
        $(".cr-item.active").removeClass("active").removeAttr("aria-current")
            .find(".cr-tono").removeClass("text-white").filter("i").addClass("text-primary").end().filter("small").addClass("text-muted");
        $('.cr-item[data-clave="' + clave + '"]').addClass("active").attr("aria-current", "true")
            .find(".cr-tono").removeClass("text-primary text-muted").addClass("text-white");
        try { history.replaceState(null, "", "?r=" + encodeURIComponent(clave)); } catch (x) { }

        $("#cr-vacio").addClass("d-none");
        $("#cr-reporte").removeClass("d-none");
        $("#cr-titulo").text(r.Titulo);
        $("#cr-descripcion").text(r.Descripcion);
        $("#cr-origen").text(r.Origen);
        $("#cr-parametros").html((r.Parametros || []).map(campoHtml).join(""));
        $("#cr-sin-parametros").toggleClass("d-none", (r.Parametros || []).length > 0);
        limpiarResultado();

        CASCADA.forEach(function (n) {
            var p = param(n);
            if (p) $("#" + idCampo(p)).on("change", function () { alCambiarCascada(n); });
        });
        aplicarValores($.extend(defectos(), valores || {})).always(function () {
            if (generarAlFinal) generar();
        });
        if (window.innerWidth < 992) $("html, body").animate({ scrollTop: $("#cr-reporte").offset().top - 80 }, 200);
    }

    $("#cr-restablecer").on("click", function () {
        if (actual) seleccionar(actual.Clave);
    });

    // ───────────── Generar ─────────────
    function limpiarResultado() {
        if (tabla && tabla.tabla) { try { tabla.tabla.destroy(); } catch (x) { } }
        tabla = null;
        resultado = null;
        $("#cr-tabla-caja").empty();
        $("#cr-estado").empty();
        $("#cr-resultado").addClass("d-none");
    }

    function valoresFormulario() {
        var v = {};
        (actual.Parametros || []).forEach(function (p) {
            var x = ($("#" + idCampo(p)).val() || "").toString().trim();
            if (x) v[p.Nombre] = x;
        });
        return v;
    }

    function estado(html) { $("#cr-estado").html(html); }

    $("#cr-form").on("submit", function (e) { e.preventDefault(); generar(); });

    function generar() {
        if (!actual) return;
        var form = document.getElementById("cr-form");
        if (form.checkValidity && !form.checkValidity()) { form.reportValidity && form.reportValidity(); return; }

        var clave = actual.Clave, valores = valoresFormulario();
        limpiarResultado();
        $("#cr-resultado").removeClass("d-none");
        estado('<div class="alert alert-light-primary mb-3"><span class="spinner-border spinner-border-sm mr-2" role="status" aria-hidden="true"></span>Generando el reporte…</div>');
        $("#cr-generar").prop("disabled", true);

        $.ajax({ url: "/CentroReportes/EjecutarAsync", data: $.extend({ clave: clave }, valores), dataType: "json" })
            .done(function (r) {
                if (!actual || actual.Clave !== clave) return;
                resultado = r;
                pintarResultado(r);
                var arriba = $("#cr-resultado").offset().top - 80;
                if (arriba > $(window).scrollTop() + window.innerHeight * 0.6) $("html, body").animate({ scrollTop: arriba }, 250);
            })
            .fail(function (x) {
                var m = (x.responseJSON && x.responseJSON.mensaje) || "No se pudo generar el reporte.";
                estado('<div class="alert alert-light-danger mb-0"><i class="bi bi-x-circle mr-1"></i><strong>Falló.</strong> ' + esc(m) + "</div>");
            })
            .always(function () {
                $("#cr-generar").prop("disabled", false);
                cargarRecientes();
            });
    }

    // ───────────── Tabla ─────────────
    var DINERO = new Intl.NumberFormat("es-HN", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var ENTERO = new Intl.NumberFormat("es-HN", { maximumFractionDigits: 0 });

    function dosDigitos(n) { return (n < 10 ? "0" : "") + n; }
    function fechaTexto(v, conHora) {
        if (!v) return "";
        var m = /^(\d{4})-(\d{2})-(\d{2})(?:T(\d{2}):(\d{2}))?/.exec(v);
        if (!m) return v;
        var t = m[3] + "/" + m[2] + "/" + m[1];
        if (conHora && m[4]) {
            var h = parseInt(m[4], 10), ampm = h < 12 ? "a. m." : "p. m.";
            t += " " + ((h % 12) || 12) + ":" + m[5] + " " + ampm;
        }
        return t;
    }
    function ordenFecha(a, b) {
        function k(s) { var m = /^(\d{2})\/(\d{2})\/(\d{4})/.exec(s || ""); return m ? m[3] + m[2] + m[1] + (s.length > 10 ? s.substring(10) : "") : ""; }
        return k(a) < k(b) ? -1 : k(a) > k(b) ? 1 : 0;
    }

    function columna(c) {
        var col = { title: c.Titulo, field: c.Campo };
        switch (c.Tipo) {
            case "entero":
                $.extend(col, { hozAlign: "right", headerHozAlign: "right", sorter: "number",
                    formatter: function (cell) { var v = cell.getValue(); return v === null || v === undefined ? "" : ENTERO.format(v); } });
                break;
            case "dinero":
                $.extend(col, { hozAlign: "right", headerHozAlign: "right", sorter: "number",
                    formatter: function (cell) { var v = cell.getValue(); return v === null || v === undefined ? "" : "L " + DINERO.format(v); } });
                break;
            case "porcentaje":
                $.extend(col, { hozAlign: "right", headerHozAlign: "right", sorter: "number",
                    formatter: function (cell) { var v = cell.getValue(); return v === null || v === undefined ? "—" : DINERO.format(v) + " %"; } });
                break;
            case "fecha":
            case "fechahora":
                $.extend(col, { sorter: ordenFecha, mutator: function (v) { return fechaTexto(v, c.Tipo === "fechahora"); } });
                break;
            case "sino":
                col.mutator = function (v) { return v === true ? "Sí" : v === false ? "No" : ""; };
                break;
            default:
                col.formatter = function (cell) { return esc(cell.getValue()); };
        }
        if (c.Total) {
            col.bottomCalc = "sum";
            col.bottomCalcFormatter = function (cell) {
                var v = cell.getValue() || 0;
                return "<strong>" + (c.Tipo === "dinero" ? "L " + DINERO.format(v) : ENTERO.format(v)) + "</strong>";
            };
        }
        return col;
    }

    /** Color del botón principal de la plantilla (sigue el tema elegido) para el encabezado del PDF. */
    function colorPrimario() {
        var m = /rgba?\((\d+),\s*(\d+),\s*(\d+)/.exec($("#cr-generar").css("background-color") || "");
        return m ? [+m[1], +m[2], +m[3]] : [80, 80, 80];
    }

    function nombreHoja(t) { return (t || "Reporte").replace(/[\\\/\?\*\[\]:]/g, " ").substring(0, 31); }

    function exportar(tipo, rango) {
        if (!tabla || !resultado) return;
        var archivo = resultado.archivo + (rango === "selected" ? "-seleccion" : "");
        if (tipo === "xlsx") {
            tabla.tabla.download("xlsx", archivo + ".xlsx", { sheetName: nombreHoja(resultado.titulo) }, rango);
            return;
        }
        tabla.tabla.download("pdf", archivo + ".pdf", {
            orientation: "landscape",
            autoTable: function (doc) {
                var ancho = doc.internal.pageSize.getWidth() - 80;
                doc.setFontSize(14);
                doc.text(resultado.titulo, 40, 40);
                doc.setFontSize(9);
                var lineas = doc.splitTextToSize(resultado.resumen + "  ·  Generado el " + resultado.generado, ancho);
                doc.text(lineas, 40, 58);
                return {
                    startY: 66 + lineas.length * 11,
                    margin: { left: 40, right: 40 },
                    styles: { fontSize: 8, cellPadding: 3 },
                    headStyles: { fillColor: colorPrimario() }
                };
            }
        }, rango);
    }

    function pintarResultado(r) {
        var n = (r.filas || []).length;
        estado('<div class="d-flex flex-wrap align-items-center justify-content-between mb-3">' +
            '<div class="mr-3"><span class="badge badge-success mr-2">Listo</span><strong>' + ENTERO.format(n) + (n === 1 ? " fila" : " filas") + "</strong>" +
            '<span class="text-muted"> · generado el ' + esc(r.generado) + "</span>" +
            '<div class="text-muted small mt-1">' + esc(r.resumen) + "</div></div></div>");

        $("#cr-tabla-caja").html('<div id="cr-tabla" data-usuario="' + esc(usuario) + '"></div>');
        var columnas = (r.columnas || []).map(columna);
        tabla = GcTabla.crear({
            elemento: "#cr-tabla",
            nombre: "reporte-" + actual.Clave,
            columnas: columnas,
            buscarEn: (r.columnas || []).map(function (c) { return c.Campo; }),
            archivo: r.archivo,
            tituloPdf: r.titulo,
            vacio: "El reporte no trajo datos con estos filtros.",
            agrupar: (r.agrupar || []).map(function (campo) {
                var c = (r.columnas || []).filter(function (x) { return x.Campo === campo; })[0];
                return { campos: [campo], texto: c ? c.Titulo : campo };
            })
        });
        var poner = function () { tabla.tabla.setData(r.filas || []); };
        if (tabla.tabla.initialized) poner(); else tabla.tabla.on("tableBuilt", poner);

        // La barra de GcTabla exporta sin título ni filtros: aquí se reemplaza por la del reporte.
        $("#cr-tabla-excel").off("click").on("click", function () { exportar("xlsx", "active"); });
        $("#cr-tabla-pdf").off("click").on("click", function () { exportar("pdf", "active"); });
        $("#cr-tabla-sel-excel").off("click").on("click", function () { exportar("xlsx", "selected"); });
        $("#cr-tabla-sel-pdf").off("click").on("click", function () { exportar("pdf", "selected"); });
        $("#cr-tabla-recargar").off("click").attr("title", "Volver a generar").on("click", generar);
    }

    // ───────────── Recientes ─────────────
    function cargarRecientes() {
        $.getJSON("/CentroReportes/RecientesAsync").done(function (r) {
            recientes = (r && r.data) || [];
            if (!recientes.length) {
                $("#cr-recientes").html('<p class="text-muted mb-0">Todavía no ha generado reportes. Los que genere aparecerán aquí para repetirlos con un clic.</p>');
                return;
            }
            $("#cr-recientes").html('<ul class="list-unstyled mb-0">' + recientes.map(function (x, i) {
                var badge = x.estado === "listo"
                    ? '<span class="badge badge-success">Listo</span>'
                    : '<span class="badge badge-danger">Falló</span>';
                var filas = x.estado === "listo" ? " · " + ENTERO.format(x.filas || 0) + (x.filas === 1 ? " fila" : " filas") : "";
                var veces = x.veces > 1 ? " · " + x.veces + " veces" : "";
                return '<li class="d-flex align-items-center justify-content-between py-2' + (i ? " border-top" : "") + '">' +
                    '<div class="mr-2 text-break"><div class="font-weight-bold">' + esc(x.titulo) + "</div>" +
                    '<div class="small text-muted">' + esc(x.resumen) + "</div>" +
                    '<div class="small text-muted mt-1">' + badge + " " + esc(x.fecha) + esc(filas) + esc(veces) + "</div></div>" +
                    '<button type="button" class="btn btn-sm btn-outline-primary text-nowrap cr-repetir" data-i="' + i + '"><i class="bi bi-arrow-repeat mr-1"></i>Repetir</button>' +
                    "</li>";
            }).join("") + "</ul>");
        }).fail(function () {
            $("#cr-recientes").html('<p class="text-muted mb-0">No se pudieron cargar sus reportes recientes.</p>');
        });
    }

    $(document).on("click", ".cr-repetir", function () {
        var x = recientes[$(this).data("i")];
        if (x) seleccionar(x.clave, x.parametros || {}, true);
    });

    // ───────────── Inicio ─────────────
    cargarRecientes();
    var inicial = $("#cr-inicial").val();
    if (inicial && REPORTES[inicial]) seleccionar(inicial);
})();
