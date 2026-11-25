/* Cierre del año escolar — asistente de 4 pasos (Revisión · Resultados finales · Decisiones de promoción ·
   Cerrar el año). Cada paso pinta lo suyo desde los endpoints de /CierreAnio; las acciones van por POST con
   el token antifalsificación y el servidor vuelve a validar permisos y estado. Filtros en cascada
   Nivel → Grado → Sección. Todo texto que viene de la base se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    var $raiz = $("#ca-raiz");
    if (!$raiz.length) return;

    function url(n) { return $raiz.data("url-" + n); }
    var ID = parseInt($raiz.data("id"), 10);
    var ANIO = parseInt($raiz.data("anio"), 10);
    var PASO = parseInt($raiz.data("paso"), 10);
    var ESTADO = parseInt($raiz.data("estado"), 10);
    var PUEDE = String($raiz.data("puede-cerrar")) === "1";
    var EDITABLE = String($raiz.data("editable")) === "1";
    var SIGUIENTE = String($raiz.data("siguiente") || "");

    var secciones = [];
    var seaActual = null;

    // ───────────────────────────── utilidades ─────────────────────────────
    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function num(n, dec) {
        if (n === null || n === undefined || n === "") return "—";
        return Number(n).toLocaleString("es-HN", { minimumFractionDigits: dec || 0, maximumFractionDigits: dec || 0 });
    }
    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    function fechaHora(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1] + ", " + m[4] + ":" + m[5] : "";
    }
    function datos(r) { return (r && r.data) ? r.data : (Array.isArray(r) ? r : []); }
    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }
    function enviar(nombre, cuerpo) {
        cuerpo.Ani_Id = ID;
        return $.ajax({
            url: url(nombre), type: "POST", contentType: "application/json; charset=utf-8",
            data: JSON.stringify(cuerpo), headers: { RequestVerificationToken: token() }, dataType: "json"
        });
    }
    function avisar(ok, msg) {
        msg = msg || (ok ? "Listo." : "Ocurrió un error. Intente de nuevo.");
        try { if (window.toastr) { toastr[ok ? "success" : "error"](msg); return; } } catch (e) { }
        try { if (window.Swal) { Swal.fire({ icon: ok ? "success" : "error", text: msg, timer: ok ? 2500 : undefined }); return; } } catch (e) { }
        window.alert(msg);
    }
    function recargar(paso) {
        window.location.href = url("index") + "?id=" + ID + "&paso=" + (paso || PASO);
    }
    function fila(colspan, texto) { return '<tr><td colspan="' + colspan + '" class="text-center text-muted">' + esc(texto) + "</td></tr>"; }

    var TXT_DEC = { promovido: "Promovido", repite: "Repite", gradua: "Se gradúa", pendiente: "Pendiente" };
    var CLS_DEC = { promovido: "badge-success", repite: "badge-danger", gradua: "badge-info", pendiente: "badge-warning" };
    function badgeDec(d) { return '<span class="badge ' + (CLS_DEC[d] || "badge-secondary") + '">' + esc(TXT_DEC[d] || d || "—") + "</span>"; }
    var TXT_RES = { aprobado: "Aprobada", recuperacion: "En recuperación", reprobado: "Reprobada", sin_notas: "Sin notas" };
    var CLS_RES = { aprobado: "badge-success", recuperacion: "badge-warning", reprobado: "badge-danger", sin_notas: "badge-secondary" };

    // ───────────────────────────── ventana de confirmación ─────────────────────────────
    var modal = { alAceptar: null };
    function abrirModal(op) {
        $("#ca-modal-titulo").text(op.titulo);
        $("#ca-modal-texto").html(op.texto);
        $("#ca-modal-error").prop("hidden", true).text("");
        var $dg = $("#ca-modal-decision-grupo").prop("hidden", !op.decisiones);
        if (op.decisiones) {
            var $s = $("#ca-modal-decision").empty();
            op.decisiones.forEach(function (d) { $s.append($("<option>").val(d).text(TXT_DEC[d])); });
            $s.val(op.decisionActual || op.decisiones[0]);
        }
        $dg.toggleClass("mb-3", !!op.decisiones);
        $("#ca-modal-motivo-grupo").prop("hidden", !op.motivo);
        $("#ca-modal-motivo-label").html(esc(op.motivoTexto || "Motivo") + (op.motivoObligatorio ? ' <span class="text-danger">*</span>' : ""));
        $("#ca-modal-motivo-ayuda").text(op.motivoAyuda || "Queda en el historial.");
        $("#ca-modal-motivo").val(op.motivoInicial || "");
        $("#ca-modal-aceptar").removeClass("btn-primary btn-danger").addClass(op.peligro ? "btn-danger" : "btn-primary")
            .text(op.boton || "Aceptar").prop("disabled", false);
        modal.alAceptar = op.alAceptar;
        $("#ca-modal").modal("show");
    }
    function errorModal(msg) { $("#ca-modal-error").prop("hidden", false).text(msg); }
    $("#ca-modal-aceptar").on("click", function () {
        if (!modal.alAceptar) return;
        var $b = $(this).prop("disabled", true);
        modal.alAceptar($.trim($("#ca-modal-motivo").val()), $("#ca-modal-decision").val())
            .always(function () { $b.prop("disabled", false); });
    });

    /** Cambia el estado del año (iniciar, cancelar, cerrar, reabrir, archivar) y recarga el paso indicado. */
    function cambiarEstado(accion, extra, pasoDestino) {
        return enviar("estado", $.extend({ Accion: accion }, extra || {}))
            .done(function (r) {
                if (!r || !r.success) { errorModal(r && r.message || "No se pudo cambiar el estado del año."); return; }
                $("#ca-modal").modal("hide");
                avisar(true, r.message);
                setTimeout(function () { recargar(pasoDestino); }, 700);
            })
            .fail(function () { errorModal("No se pudo conectar con el servidor."); });
    }

    // ───────────────────────────── revisión (pasos 1 y 4) ─────────────────────────────
    var ICONO = {
        ok: '<i class="bi bi-check-circle-fill text-success" aria-hidden="true"></i>',
        aviso: '<i class="bi bi-exclamation-triangle-fill text-warning" aria-hidden="true"></i>',
        bloquea: '<i class="bi bi-x-octagon-fill text-danger" aria-hidden="true"></i>'
    };
    var TXT_NIVEL = { ok: "En orden", aviso: "Revisar", bloquea: "Impide cerrar" };
    var VE = { parciales: String($raiz.data("ve-parciales")) === "1", inscripcion: String($raiz.data("ve-inscripcion")) === "1", anios: String($raiz.data("ve-anios")) === "1" };
    function enlace(clave) {
        if ((clave === "parciales" || clave === "inscripcion") && !VE[clave]) return null;
        if ((clave === "secciones" || clave === "anios") && !VE.anios) return null;
        switch (clave) {
            case "parciales": return { href: url("parciales"), texto: "Cierre de parciales" };
            case "inscripcion": return { href: url("inscripcion"), texto: "Inscripción y promociones" };
            case "secciones": return { href: url("anios"), texto: "Años lectivos" };
            case "anios": return { href: url("anios"), texto: "Años lectivos" };
            case "resultados": return ESTADO >= 4 ? { href: url("index") + "?id=" + ID + "&paso=2", texto: "Resultados finales" } : null;
            case "decisiones": return ESTADO >= 4 ? { href: url("index") + "?id=" + ID + "&paso=3", texto: "Decisiones" } : null;
            default: return null;
        }
    }
    function pintarPuntos(lista) {
        if (!lista.length) return '<p class="text-muted mb-0">No se pudo leer la revisión.</p>';
        var html = '<ul class="list-group">';
        lista.forEach(function (p) {
            var e = p.Nivel === "ok" ? null : enlace(p.Enlace);
            html += '<li class="list-group-item d-flex flex-wrap align-items-start justify-content-between">' +
                '<div class="d-flex align-items-start mr-3">' +
                '<span class="mr-2 mt-1">' + (ICONO[p.Nivel] || "") + '<span class="sr-only">' + esc(TXT_NIVEL[p.Nivel] || "") + "</span></span>" +
                '<div><div class="font-weight-bold">' + esc(p.Titulo) + '</div><div class="text-muted small">' + esc(p.Detalle) + "</div></div></div>" +
                (e ? '<a class="btn btn-sm btn-outline-primary mt-1" href="' + esc(e.href) + '">' + esc(e.texto) + "</a>" : "") +
                "</li>";
        });
        return html + "</ul>";
    }
    function cargarRevision() {
        return $.getJSON(url("revision"), { id: ID }).then(function (r) { return datos(r); });
    }

    function paso1() {
        cargarRevision().done(function (lista) {
            var antes = lista.filter(function (p) { return p.Grupo === "antes"; });
            var cierre = lista.filter(function (p) { return p.Grupo !== "antes"; });
            $("#ca-revision").html(
                '<h6 class="mb-2">Datos del año</h6>' + pintarPuntos(antes) +
                '<h6 class="mt-4 mb-2">Pasos del cierre</h6>' + pintarPuntos(cierre));
        }).fail(function () { $("#ca-revision").html('<p class="text-danger mb-0">No se pudo leer la revisión.</p>'); });

        $("#ca-iniciar").on("click", function () {
            abrirModal({
                titulo: "Iniciar el cierre de " + ANIO,
                texto: "El año pasa a <strong>«Cerrando»</strong>: se habilitan los resultados finales y las decisiones de promoción. " +
                    "Los docentes siguen pudiendo corregir notas en los parciales que Dirección reabra. Se puede cancelar mientras no haya decisiones confirmadas.",
                boton: "Iniciar el cierre",
                alAceptar: function () { return cambiarEstado("iniciar", {}, 2); }
            });
        });
        $("#ca-cancelar").on("click", function () {
            abrirModal({
                titulo: "Cancelar el cierre de " + ANIO,
                texto: "El año vuelve a estar <strong>abierto</strong>. Los resultados calculados se conservan y se pueden recalcular después.",
                motivo: true, motivoTexto: "Motivo (opcional)",
                boton: "Cancelar el cierre",
                alAceptar: function (motivo) { return cambiarEstado("cancelar", { Motivo: motivo }, 1); }
            });
        });
    }

    // ───────────────────────────── filtros en cascada ─────────────────────────────
    function opciones($sel, items, vacio) {
        $sel.empty().append($("<option>").val("").text(vacio));
        items.forEach(function (it) { $sel.append($("<option>").val(it.v).text(it.t)); });
    }
    function unicos(lista, clave, texto, orden) {
        var vistos = {}, salida = [];
        lista.forEach(function (s) {
            var v = s[clave];
            if (v === null || v === undefined || vistos[v]) return;
            vistos[v] = true;
            salida.push({ v: v, t: s[texto], o: orden ? s[orden] : 0 });
        });
        return salida.sort(function (a, b) { return a.o - b.o || String(a.t).localeCompare(String(b.t)); });
    }
    function filtradas() {
        var niv = $("#ca-f-nivel").val(), gra = $("#ca-f-grado").val();
        return secciones.filter(function (s) {
            if (niv && String(s.Niv_Id) !== niv) return false;
            if (gra && String(s.Cun_Id) !== gra) return false;
            return true;
        });
    }
    function iniciarFiltros(alCambiar, alElegir) {
        var $niv = $("#ca-f-nivel"), $gra = $("#ca-f-grado"), $sec = $("#ca-f-seccion");
        opciones($niv, unicos(secciones.filter(function (s) { return s.Sea_Id > 0; }), "Niv_Id", "Niv_Descripcion", "Niv_Id"), "Todos los niveles");
        function llenarSecciones() {
            var lista = filtradas().map(function (s) { return { v: s.Sea_Id, t: s.Sea_Nombre }; });
            opciones($sec, lista, "Elija una sección");
        }
        llenarSecciones();
        $sec.prop("disabled", false);
        $niv.on("change", function () {
            var v = $niv.val();
            opciones($gra, v ? unicos(secciones.filter(function (s) { return String(s.Niv_Id) === v; }), "Cun_Id", "Cun_Descripcion", "Cun_Orden") : [], "Todos los grados");
            $gra.prop("disabled", !v);
            llenarSecciones();
            alCambiar();
        });
        $gra.on("change", function () { llenarSecciones(); alCambiar(); });
        $sec.on("change", function () { if ($sec.val() !== "") alElegir(parseInt($sec.val(), 10)); });
    }
    function cargarSecciones() {
        return $.getJSON(url("secciones"), { id: ID }).then(function (r) { secciones = datos(r); return secciones; });
    }
    function nombreSeccion(seaId) {
        var s = secciones.filter(function (x) { return x.Sea_Id === seaId; })[0];
        return s ? s.Sea_Nombre : "Sección";
    }
    function estadoSeccion(s) {
        if (!s.ConDecision) return '<span class="badge badge-secondary">Sin resultados</span>';
        if (s.Confirmados >= s.ConDecision) return '<span class="badge badge-success">Confirmada</span>';
        if (s.Pendientes > 0) return '<span class="badge badge-warning">Con pendientes</span>';
        return '<span class="badge badge-info">Por confirmar</span>';
    }
    function marcarFila(seaId) {
        $("#ca-secciones tr").removeClass("table-active");
        $('#ca-secciones tr[data-sea="' + seaId + '"]').addClass("table-active");
        if (seaId > 0) $("#ca-f-seccion").val(String(seaId));
    }

    // ───────────────────────────── paso 2: reglas y resultados ─────────────────────────────
    var TXT_ORIGEN = { guardada: "Guardada para este año", anio_anterior: "Igual que el año anterior", escala: "Escala de notas (valor inicial)", honduras: "Valor inicial (70)" };
    function paso2() {
        var editarReglas = PUEDE && (ESTADO === 3 || ESTADO === 4);
        $.getJSON(url("reglas"), { id: ID }).done(function (r) {
            var lista = datos(r);
            if (!lista.length) { $("#ca-reglas").html(fila(5, "No hay niveles.")); return; }
            $("#ca-reglas").html(lista.map(function (g) {
                var nota = editarReglas
                    ? '<input type="number" class="form-control form-control-sm w-auto ca-r-nota" size="5" min="1" max="100" step="1" value="' + esc(Number(g.NotaMinima)) + '" aria-label="Nota mínima de ' + esc(g.Niv_Descripcion) + '">'
                    : esc(num(g.NotaMinima));
                var max = editarReglas
                    ? '<input type="number" class="form-control form-control-sm w-auto ca-r-max" size="5" min="0" max="20" step="1" value="' + esc(g.MaxRecuperacion) + '" aria-label="Máximo de materias en recuperación de ' + esc(g.Niv_Descripcion) + '">'
                    : esc(g.MaxRecuperacion);
                return '<tr data-niv="' + esc(g.Niv_Id) + '"><td class="align-middle">' + esc(g.Niv_Descripcion) + '</td><td class="align-middle">' + nota +
                    '</td><td class="align-middle">' + max + '</td><td class="align-middle text-muted small">' + esc(TXT_ORIGEN[g.Origen] || g.Origen) + "</td>" +
                    '<td class="align-middle text-right">' + (editarReglas ? '<button type="button" class="btn btn-sm btn-outline-primary ca-r-guardar">Guardar</button>' : "") + "</td></tr>";
            }).join(""));
        }).fail(function () { $("#ca-reglas").html(fila(5, "No se pudieron leer las reglas.")); });

        $("#ca-reglas").on("click", ".ca-r-guardar", function () {
            var $tr = $(this).closest("tr"), $b = $(this).prop("disabled", true);
            enviar("regla", {
                Niv_Id: parseInt($tr.data("niv"), 10),
                NotaMinima: parseFloat($tr.find(".ca-r-nota").val()),
                MaxRecuperacion: parseInt($tr.find(".ca-r-max").val(), 10)
            }).done(function (r) {
                avisar(r && r.success, r && r.message);
                if (r && r.success) setTimeout(function () { recargar(2); }, 700);
            }).fail(function () { avisar(false); }).always(function () { $b.prop("disabled", false); });
        });

        $("#ca-calcular").on("click", function () {
            var $b = $(this).prop("disabled", true).text("Calculando…");
            enviar("calcular", {}).done(function (r) {
                if (!r || !r.success) { avisar(false, r && r.message); return; }
                var d = r.datos || {};
                avisar(true, "Resultados calculados: " + num(d.Alumnos) + " alumnos, " + num(d.Materias) + " materias (" +
                    num(d.EnRecuperacion) + " en recuperación, " + num(d.Reprobadas) + " reprobadas).");
                setTimeout(function () { recargar(2); }, 900);
            }).fail(function () { avisar(false); }).always(function () { $b.prop("disabled", false).text("Volver a calcular"); });
        });

        cargarSecciones().done(function () {
            iniciarFiltros(pintarSecciones2, verResultados);
            pintarSecciones2();
        }).fail(function () { $("#ca-secciones").html(fila(6, "No se pudieron leer las secciones.")); });

        $("#ca-secciones").on("click", ".ca-ver", function () { verResultados(parseInt($(this).data("sea"), 10)); });
        $("#ca-resultados").on("click", ".ca-rec-guardar", function () { guardarRecuperacion($(this), false); });
        $("#ca-resultados").on("click", ".ca-rec-quitar", function () { guardarRecuperacion($(this), true); });
    }
    function pintarSecciones2() {
        var lista = filtradas();
        if (!lista.length) { $("#ca-secciones").html(fila(6, "El año no tiene secciones con estos filtros.")); return; }
        $("#ca-secciones").html(lista.map(function (s) {
            return '<tr data-sea="' + s.Sea_Id + '"><td>' + esc(s.Sea_Nombre) + '</td><td class="text-right">' + num(s.Alumnos) +
                '</td><td class="text-right">' + num(s.EnRecuperacion) + '</td><td class="text-right">' + num(s.Pendientes) +
                "</td><td>" + estadoSeccion(s) + '</td><td class="text-right"><button type="button" class="btn btn-sm btn-outline-primary text-nowrap ca-ver" data-sea="' + s.Sea_Id + '">Ver resultados</button></td></tr>';
        }).join(""));
        if (seaActual !== null) marcarFila(seaActual);
    }
    function verResultados(seaId) {
        seaActual = seaId;
        marcarFila(seaId);
        $("#ca-detalle").prop("hidden", false);
        $("#ca-detalle-titulo").text("Resultados · " + nombreSeccion(seaId));
        $("#ca-resultados").html(fila(6, "Cargando…"));
        $.getJSON(url("resultados"), { id: ID, seaId: seaId }).done(function (r) {
            var lista = datos(r);
            if (!lista.length) { $("#ca-resultados").html(fila(6, ESTADO >= 4 ? "Sin resultados: calcule los resultados del año." : "Inicie el cierre para calcular los resultados.")); return; }
            var html = "", alu = null;
            lista.forEach(function (x) {
                if (x.Alu_Id !== alu) {
                    alu = x.Alu_Id;
                    var suyas = lista.filter(function (y) { return y.Alu_Id === alu; });
                    var rec = suyas.filter(function (y) { return y.Resultado === "recuperacion"; }).length;
                    var rep = suyas.filter(function (y) { return y.Resultado === "reprobado"; }).length;
                    html += '<tr class="table-light"><td colspan="6"><strong>' + esc(x.Alumno) + "</strong>" +
                        (x.Identidad ? ' <span class="text-muted small ml-1">' + esc(x.Identidad) + "</span>" : "") +
                        '<span class="small text-muted ml-2">' + suyas.length + " materias" + (rec ? " · " + rec + " en recuperación" : "") + (rep ? " · " + rep + " reprobadas" : "") + "</span>" +
                        (x.Confirmada ? ' <span class="badge outline-badge-success ml-2">Decisión confirmada</span>' : "") + "</td></tr>";
                }
                var editarRec = EDITABLE && x.EnRecuperacion && !x.Confirmada;
                var recCelda;
                if (editarRec) {
                    recCelda = '<div class="input-group input-group-sm ca-rec" data-rea="' + x.Rea_Id + '">' +
                        '<input type="number" class="form-control" min="0" max="100" step="1" value="' + (x.NotaRecuperacion === null ? "" : esc(Number(x.NotaRecuperacion))) + '" aria-label="Nota de recuperación de ' + esc(x.Materia) + '">' +
                        '<div class="input-group-append"><button type="button" class="btn btn-outline-primary ca-rec-guardar">Guardar</button>' +
                        (x.NotaRecuperacion !== null ? '<button type="button" class="btn btn-outline-danger ca-rec-quitar" title="Quitar la nota de recuperación">Quitar</button>' : "") + "</div></div>";
                } else if (x.NotaRecuperacion !== null && x.EnRecuperacion) {
                    recCelda = esc(num(x.NotaRecuperacion)) + (x.AproboRecuperacion ? ' <span class="text-success small">aprobó</span>' : ' <span class="text-danger small">no aprobó</span>');
                } else {
                    recCelda = x.EnRecuperacion ? '<span class="text-muted small">Sin nota</span>' : '<span class="text-muted">—</span>';
                }
                var falta = x.PeriodosConNota < x.PeriodosEsperados;
                html += "<tr><td>" + esc(x.Materia) + '</td><td class="text-right' + (falta ? " text-warning" : "") + '">' + x.PeriodosConNota + " de " + x.PeriodosEsperados +
                    '</td><td class="text-right">' + num(x.Promedio, 2) + '</td><td class="text-right font-weight-bold">' + num(x.NotaFinal) +
                    (x.Concepto ? '<div class="small text-muted font-weight-normal">' + esc(x.Concepto) + "</div>" : "") +
                    '</td><td><span class="badge ' + (CLS_RES[x.Resultado] || "badge-secondary") + '">' + esc(TXT_RES[x.Resultado] || x.Resultado) + "</span>" +
                    (x.AproboRecuperacion === true && x.Resultado === "aprobado" ? '<div class="small text-muted">en recuperación</div>' : "") +
                    "</td><td>" + recCelda + "</td></tr>";
            });
            $("#ca-resultados").html(html);
        }).fail(function () { $("#ca-resultados").html(fila(6, "No se pudieron leer los resultados.")); });
    }
    function guardarRecuperacion($b, quitar) {
        var $g = $b.closest(".ca-rec"), valor = $.trim($g.find("input").val());
        if (!quitar && (valor === "" || isNaN(valor))) { avisar(false, "Escriba la nota de recuperación (0 a 100)."); return; }
        $b.prop("disabled", true);
        enviar("recuperacion", { Rea_Id: parseInt($g.data("rea"), 10), Nota: quitar ? null : parseFloat(valor) })
            .done(function (r) {
                avisar(r && r.success, r && r.message);
                if (r && r.success) cargarSecciones().done(function () { pintarSecciones2(); verResultados(seaActual); });
            })
            .fail(function () { avisar(false); })
            .always(function () { $b.prop("disabled", false); });
    }

    // ───────────────────────────── paso 3: decisiones ─────────────────────────────
    function paso3() {
        cargarSecciones().done(function () {
            iniciarFiltros(pintarSecciones3, verDecisiones);
            pintarSecciones3();
        }).fail(function () { $("#ca-secciones").html(fila(8, "No se pudieron leer las secciones.")); });
        $("#ca-secciones").on("click", ".ca-ver", function () { verDecisiones(parseInt($(this).data("sea"), 10)); });
        $("#ca-decisiones").on("click", ".ca-cambiar", function () { cambiarDecision($(this).data("fila")); });
        $("#ca-detalle-acciones").on("click", "#ca-confirmar", confirmarSeccion);
        $("#ca-detalle-acciones").on("click", "#ca-deshacer", deshacerSeccion);
    }
    function pintarSecciones3() {
        var lista = filtradas();
        if (!lista.length) { $("#ca-secciones").html(fila(8, "El año no tiene secciones con estos filtros.")); return; }
        $("#ca-secciones").html(lista.map(function (s) {
            return '<tr data-sea="' + s.Sea_Id + '"><td>' + esc(s.Sea_Nombre) + '</td><td class="text-right">' + num(s.Alumnos) +
                '</td><td class="text-right">' + num(s.Promovidos) + '</td><td class="text-right">' + num(s.Repiten) +
                '</td><td class="text-right">' + num(s.Graduan) + '</td><td class="text-right">' + num(s.Pendientes) +
                "</td><td>" + estadoSeccion(s) + '</td><td class="text-right"><button type="button" class="btn btn-sm btn-outline-primary text-nowrap ca-ver" data-sea="' + s.Sea_Id + '">Ver alumnos</button></td></tr>';
        }).join(""));
        if (seaActual !== null) marcarFila(seaActual);
    }
    var decisiones = [];
    function verDecisiones(seaId) {
        seaActual = seaId;
        marcarFila(seaId);
        $("#ca-detalle").prop("hidden", false);
        $("#ca-detalle-titulo").text("Decisiones · " + nombreSeccion(seaId));
        $("#ca-decisiones").html(fila(7, "Cargando…"));
        $("#ca-detalle-acciones").empty();
        $("#ca-detalle-texto").text("");
        $.getJSON(url("decisiones"), { id: ID, seaId: seaId }).done(function (r) {
            decisiones = datos(r);
            if (!decisiones.length) { $("#ca-decisiones").html(fila(7, ESTADO >= 4 ? "Sin decisiones: calcule los resultados (paso 2)." : "Inicie el cierre para ver las propuestas.")); return; }
            var conf = decisiones.filter(function (d) { return d.Confirmada; }).length;
            var pend = decisiones.filter(function (d) { return !d.Confirmada && d.Efectiva === "pendiente"; }).length;
            $("#ca-detalle-texto").text(conf === decisiones.length
                ? "Sección confirmada" + (decisiones[0].UsuarioConfirma ? " por " + decisiones[0].UsuarioConfirma : "") + (decisiones[0].FechaConfirmacion ? " el " + fechaHora(decisiones[0].FechaConfirmacion) : "") + "."
                : (pend ? pend + " alumno(s) pendientes: registre su recuperación en el paso 2 o decida con un motivo." : "Revise las propuestas y confirme la sección."));
            if (EDITABLE) {
                if (conf < decisiones.length)
                    $("#ca-detalle-acciones").html('<button type="button" class="btn btn-success" id="ca-confirmar"' + (pend ? " disabled" : "") + '><i class="bi bi-check2-all mr-1" aria-hidden="true"></i> Confirmar la sección</button>');
                else
                    $("#ca-detalle-acciones").html('<button type="button" class="btn btn-outline-danger" id="ca-deshacer"><i class="bi bi-arrow-counterclockwise mr-1" aria-hidden="true"></i> Deshacer la confirmación</button>');
            }
            $("#ca-decisiones").html(decisiones.map(function (d, i) {
                var resumen = d.Aprobadas + " de " + d.Materias + " aprobadas" +
                    (d.EnRecuperacion ? ' · <span class="text-warning">' + d.EnRecuperacion + " en recuperación</span>" : "") +
                    (d.Reprobadas ? ' · <span class="text-danger">' + d.Reprobadas + " reprobadas</span>" : "") +
                    (d.SinNotas ? ' · <span class="text-muted">' + d.SinNotas + " sin notas</span>" : "");
                var decision = d.Decision
                    ? badgeDec(d.Decision) + '<div class="small text-muted">Motivo: ' + esc(d.Motivo) + "</div>"
                    : '<span class="text-muted small">Igual a la propuesta</span>';
                var destino = d.Archivado ? '<span class="badge badge-dark">Graduado y archivado</span>'
                    : (d.Destino ? esc(d.AnioDestino) + " · " + esc(d.Destino) : (d.Confirmada ? '<span class="text-muted small">' + (d.Efectiva === "gradua" ? "Pasa a Graduación" : "Sin inscribir (no existe el año siguiente)") + "</span>" : '<span class="text-muted">—</span>'));
                return '<tr><td><strong>' + esc(d.Alumno) + "</strong>" + (d.Identidad ? '<div class="small text-muted">' + esc(d.Identidad) + "</div>" : "") +
                    '</td><td class="small text-wrap">' + resumen + '</td><td class="text-right">' + num(d.Promedio, 2) +
                    "</td><td>" + badgeDec(d.Propuesta) + '</td><td class="text-wrap">' + decision + '</td><td class="text-wrap">' + destino + '</td><td class="text-right">' +
                    (EDITABLE && !d.Confirmada ? '<button type="button" class="btn btn-sm btn-outline-primary ca-cambiar" data-fila="' + i + '">Cambiar</button>' :
                        (d.Confirmada ? '<span class="badge outline-badge-success">Confirmada</span>' : "")) + "</td></tr>";
            }).join(""));
        }).fail(function () { $("#ca-decisiones").html(fila(7, "No se pudieron leer las decisiones.")); });
    }
    function cambiarDecision(i) {
        var d = decisiones[i];
        if (!d) return;
        var lista = [];
        if (d.HaySiguienteGrado) lista.push("promovido");
        lista.push("repite");
        if (d.EsUltimoGrado) lista.push("gradua");
        abrirModal({
            titulo: "Decisión de " + d.Alumno,
            texto: "Propuesta según sus resultados: " + badgeDec(d.Propuesta) + ". " +
                (d.EsUltimoGrado && d.HaySiguienteGrado ? "Cursa el último grado de su nivel: «Promovido» lo inscribe en el primer grado del nivel siguiente. " : "") +
                "Si elige algo distinto a la propuesta, escriba el motivo.",
            decisiones: lista,
            decisionActual: d.Efectiva !== "pendiente" ? d.Efectiva : lista[0],
            motivo: true, motivoTexto: "Motivo del cambio", motivoInicial: d.Motivo || "",
            motivoAyuda: "Obligatorio si la decisión no es la propuesta. Queda guardado con la decisión.",
            boton: "Guardar decisión",
            alAceptar: function (motivo, decision) {
                return enviar("decision", { Pra_Id: d.Pra_Id, Decision: decision, Motivo: motivo }).done(function (r) {
                    if (!r || !r.success) { errorModal(r && r.message || "No se pudo guardar la decisión."); return; }
                    $("#ca-modal").modal("hide");
                    avisar(true, r.message);
                    cargarSecciones().done(function () { pintarSecciones3(); verDecisiones(seaActual); });
                }).fail(function () { errorModal("No se pudo conectar con el servidor."); });
            }
        });
    }
    function confirmarSeccion() {
        var n = decisiones.length;
        var prom = decisiones.filter(function (d) { return d.Efectiva === "promovido"; }).length;
        var rep = decisiones.filter(function (d) { return d.Efectiva === "repite"; }).length;
        var gra = decisiones.filter(function (d) { return d.Efectiva === "gradua"; }).length;
        abrirModal({
            titulo: "Confirmar " + nombreSeccion(seaActual),
            texto: "Se confirman " + n + " decisiones: <strong>" + prom + " promovidos, " + rep + " repiten y " + gra + " se gradúan</strong>. " +
                "La inscripción de " + ANIO + " queda con ese resultado" +
                (SIGUIENTE ? "; los promovidos y los que repiten quedan inscritos en " + esc(SIGUIENTE) + " (en la sección equivalente si existe, si no, sin sección)" : "; como no existe el año siguiente, nadie queda inscrito todavía") +
                (gra ? " y los que se gradúan aparecen en Graduación" : "") + ". Sus resultados quedan congelados.",
            boton: "Confirmar",
            alAceptar: function () {
                return enviar("confirmar", { Sea_Id: seaActual }).done(function (r) {
                    if (!r || !r.success) { errorModal(r && r.message || "No se pudieron confirmar las decisiones."); return; }
                    $("#ca-modal").modal("hide");
                    var d = r.datos || {};
                    avisar(true, "Decisiones confirmadas: " + num(d.Confirmados) + "." + (d.Inscritos ? " " + num(d.Inscritos) + " inscritos en " + esc(d.AnioDestino) + (d.SinSeccion ? " (" + num(d.SinSeccion) + " sin sección)" : "") + "." : ""));
                    cargarSecciones().done(function () { pintarSecciones3(); verDecisiones(seaActual); });
                }).fail(function () { errorModal("No se pudo conectar con el servidor."); });
            }
        });
    }
    function deshacerSeccion() {
        abrirModal({
            titulo: "Deshacer la confirmación de " + nombreSeccion(seaActual),
            texto: "La inscripción de " + ANIO + " vuelve a «Activa» y se quita la inscripción del año siguiente que hizo la confirmación (si nadie la cambió). Las decisiones vuelven a revisión.",
            motivo: true, motivoTexto: "Motivo", motivoObligatorio: true, peligro: true,
            boton: "Deshacer",
            alAceptar: function (motivo) {
                if (!motivo) { errorModal("Escriba el motivo para deshacer la confirmación."); return $.Deferred().resolve().promise(); }
                return enviar("deshacer", { Sea_Id: seaActual, Motivo: motivo }).done(function (r) {
                    if (!r || !r.success) { errorModal(r && r.message || "No se pudo deshacer la confirmación."); return; }
                    $("#ca-modal").modal("hide");
                    avisar(true, r.message);
                    cargarSecciones().done(function () { pintarSecciones3(); verDecisiones(seaActual); });
                }).fail(function () { errorModal("No se pudo conectar con el servidor."); });
            }
        });
    }

    // ───────────────────────────── paso 4: cerrar, reabrir, archivar, historial ─────────────────────────────
    var hayBloqueos = false;
    function paso4() {
        cargarRevision().done(function (lista) {
            var bloquea = lista.filter(function (p) { return p.Nivel === "bloquea"; });
            var aviso = lista.filter(function (p) { return p.Nivel === "aviso"; });
            hayBloqueos = bloquea.length > 0;
            var html = "";
            if (ESTADO === 4) {
                html = bloquea.length
                    ? '<div class="alert alert-light-danger" role="alert"><strong>Todavía no está listo para cerrar.</strong></div>' + pintarPuntos(bloquea.concat(aviso))
                    : (aviso.length ? '<div class="alert alert-light-success" role="alert">Nada impide cerrar. Revise estos avisos:</div>' + pintarPuntos(aviso)
                        : '<div class="alert alert-light-success mb-0" role="alert"><i class="bi bi-check-circle-fill mr-1" aria-hidden="true"></i>Todo está listo para cerrar el año.</div>');
            } else {
                html = '<p class="text-muted mb-0">' + (ESTADO === 5 ? "El año está cerrado." : "El año está archivado.") + " La revisión que se hizo al cerrarlo queda en el historial.</p>";
            }
            $("#ca-pendientes").html(html);
            $("#ca-forzar-grupo").prop("hidden", !hayBloqueos);
        }).fail(function () { $("#ca-pendientes").html('<p class="text-danger mb-0">No se pudo leer la revisión.</p>'); });

        $.getJSON(url("historial"), { id: ID }).done(function (r) {
            var lista = datos(r);
            $("#ca-historial").html(lista.length ? lista.map(function (h) {
                var estado = h.EstadoNuevo ? esc(h.EstadoAnteriorTexto) + ' <i class="bi bi-arrow-right" aria-hidden="true"></i> ' + esc(h.EstadoNuevoTexto) : '<span class="text-muted">—</span>';
                var detalle = [h.Detalle, h.Motivo ? "Motivo: " + h.Motivo : null].filter(Boolean).map(esc).join("<br>");
                return "<tr><td class=\"text-nowrap\">" + esc(fechaHora(h.Fecha)) + "</td><td>" + esc(h.AccionTexto) +
                    (h.Forzado ? ' <span class="badge badge-warning">con pendientes</span>' : "") + "</td><td class=\"text-nowrap\">" + estado +
                    '</td><td class="text-wrap">' + (detalle || '<span class="text-muted">—</span>') + '</td><td class="text-nowrap">' + esc(h.Usuario || "") + "</td></tr>";
            }).join("") : fila(5, "Sin movimientos todavía."));
        }).fail(function () { $("#ca-historial").html(fila(5, "No se pudo leer el historial.")); });

        $("#ca-cerrar").on("click", function () {
            var forzar = $("#ca-forzar").is(":checked"), motivo = $.trim($("#ca-motivo-cierre").val());
            if (hayBloqueos && !forzar) { avisar(false, "Hay puntos que impiden cerrar. Resuélvalos o marque «Cerrar de todos modos» y escriba el motivo."); return; }
            if (hayBloqueos && !motivo) { avisar(false, "Escriba el motivo para cerrar con puntos pendientes."); return; }
            var pasar = $("#ca-pasar-activo").is(":checked");
            abrirModal({
                titulo: "Cerrar el año " + ANIO,
                texto: "Desde ahora nadie podrá registrar ni cambiar notas, actividades, asistencia, pase de lista ni tareas de " + ANIO +
                    ". Los resultados y las decisiones quedan como están." + (pasar ? " El año de trabajo pasa a " + esc(SIGUIENTE) + "." : "") +
                    " Solo se podrá reabrir con el permiso especial y un motivo.",
                peligro: true, boton: "Cerrar el año",
                alAceptar: function () { return cambiarEstado("cerrar", { Forzar: forzar, Motivo: motivo, PasarActivo: pasar }, 4); }
            });
        });
        $("#ca-reabrir").on("click", function () {
            abrirModal({
                titulo: "Reabrir el año " + ANIO,
                texto: "El año vuelve a <strong>«Cerrando»</strong>: se podrán corregir notas (en los parciales que se reabran), resultados y decisiones. Diga qué corrección se autorizó.",
                motivo: true, motivoTexto: "Motivo", motivoObligatorio: true, peligro: true, boton: "Reabrir",
                alAceptar: function (motivo) {
                    if (!motivo) { errorModal("Escriba el motivo para reabrir el año."); return $.Deferred().resolve().promise(); }
                    return cambiarEstado("reabrir", { Motivo: motivo }, 4);
                }
            });
        });
        $("#ca-archivar").on("click", function () {
            abrirModal({
                titulo: "Archivar el año " + ANIO,
                texto: "El año queda como historia de solo lectura y deja de salir en los selectores de año. <strong>No se puede deshacer.</strong>",
                motivo: true, motivoTexto: "Motivo (opcional)", peligro: true, boton: "Archivar",
                alAceptar: function (motivo) { return cambiarEstado("archivar", { Motivo: motivo }, 4); }
            });
        });
    }

    if (PASO === 1) paso1();
    else if (PASO === 2) paso2();
    else if (PASO === 3) paso3();
    else paso4();
})(jQuery);
