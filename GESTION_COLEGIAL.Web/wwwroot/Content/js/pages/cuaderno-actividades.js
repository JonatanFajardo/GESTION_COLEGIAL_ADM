/* Cuaderno de notas · actividades (notas ponderadas, espejo del "gradebook" de RosarioSIS y Skuul).
   Al cargar una materia, sección y parcial en el cuaderno (notasadm.js avisa con los eventos
   "cuaderno:cargar" / "cuaderno:limpiar" / "cuaderno:listo"):
   - sin actividades: sigue la nota directa de siempre; si hay plantilla de evaluación se ofrecen sus pesos;
   - con actividades: planilla alumno × actividad (puntos, E = excusado, NE = no entregó, vacío = sin
     calificar), nota calculada en vivo con los pesos de la plantilla y la nota guardada en el parcial.
   Guardar escribe los puntos y la API pasa la nota calculada a las notas del parcial. Un parcial cerrado
   es solo lectura. Un cambio de materia o parcial con puntos sin guardar pide confirmación.
   Todo texto que escribió una persona se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    var $raiz = $("#ev-raiz");
    if (!$raiz.length) return;

    var URL = {
        cuaderno: $raiz.data("url-cuaderno"),
        guardarActividad: $raiz.data("url-guardar-actividad"),
        quitarActividad: $raiz.data("url-quitar-actividad"),
        guardarPuntos: $raiz.data("url-guardar-puntos"),
        actualizar: $raiz.data("url-actualizar"),
        notaMinima: $raiz.data("url-nota-minima")
    };
    var COMBINACION = { suma: "suma de puntos", promedio: "promedio de las actividades", mejor: "la mejor actividad" };
    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];

    var notaMinima = 70;
    var ctx = null;          // { Hor_Id, Par_Id, Anio, Sem_Id } que mandó notasadm.js
    var C = null;            // cuaderno cargado (JSON de la API)
    var tipos = {};          // Cet_Id -> tipo del cuaderno
    var celdas = {};         // "Act_Id|Alu_Id" -> { e, p, v, oe, op, invalida }
    var pendiente = null;    // acción en espera mientras se confirma descartar cambios
    var editando = null;     // actividad que se edita (null = nueva)
    var quitando = null;
    var selPrevio = {};

    // ───────────────────────────── Utilidades ─────────────────────────────
    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }
    function enviar(url, datos) {
        return $.ajax({
            url: url, type: "POST", contentType: "application/json; charset=utf-8", dataType: "json",
            data: JSON.stringify(datos || {}), headers: { RequestVerificationToken: token() }
        });
    }
    function fmt(n) { return (Number(n) || 0).toLocaleString("es-HN", { maximumFractionDigits: 2 }); }
    function fecha(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] : "";
    }
    function clave(act, alu) { return act + "|" + alu; }
    function alerta(tipo, html) {
        return '<div class="alert alert-light-' + tipo + ' mb-4" role="alert">' + html + '</div>';
    }
    function cerrado() { return !!(C && C.Cerrado); }

    // ───────────────────────────── Celdas ─────────────────────────────
    // Texto de la celda -> estado. Vacío = sin calificar (no es cero).
    function leer(texto, max) {
        var t = String(texto || "").trim().toUpperCase();
        if (t === "") return { e: "vacio", p: null };
        if (t === "E") return { e: "excusado", p: null };
        if (t === "NE" || t === "N") return { e: "no_entrego", p: null };
        t = t.replace(",", ".");
        if (!/^\d+(\.\d{1,2})?$/.test(t)) return { error: "Escriba un número (hasta 2 decimales), E o NE." };
        var n = parseFloat(t);
        if (n > max) return { error: "El máximo de esta actividad es " + fmt(max) + " puntos." };
        return { e: "calificado", p: n };
    }
    function textoCelda(e, p) {
        if (e === "calificado") return String(Math.round(p * 100) / 100);
        if (e === "excusado") return "E";
        if (e === "no_entrego") return "NE";
        return "";
    }
    function sucia(c) { return c.e !== c.oe || c.p !== c.op; }
    function cambios() {
        var n = 0;
        $.each(celdas, function (_, c) { if (sucia(c) && !c.invalida) n++; });
        return n;
    }
    function invalidas() {
        var n = 0;
        $.each(celdas, function (_, c) { if (c.invalida) n++; });
        return n;
    }
    function hayCambios() { return cambios() > 0 || invalidas() > 0; }

    // ───────────────────────────── Cálculo (mismo que app.FN_Evaluacion_Calculo) ─────────────────────────────
    // Por tipo: suma = puntos/máximo sumados; promedio = promedio de cada actividad; mejor = la mejor.
    // Solo cuentan calificado y no entregó (0) de actividades que cuentan; los tipos sin nada no entran.
    function calcular(aluId) {
        var grupos = {};
        $.each(C.Actividades, function (_, a) {
            if (a.NoCuenta) return;
            var c = celdas[clave(a.Act_Id, aluId)];
            var t = tipos[a.Cet_Id];
            if (!c || !t || (c.e !== "calificado" && c.e !== "no_entrego")) return;
            var g = grupos[a.Cet_Id] || (grupos[a.Cet_Id] = { peso: Number(t.Peso), comb: t.Combinacion, partes: [], pts: 0, max: 0 });
            var p = c.e === "calificado" ? c.p : 0;
            g.partes.push(p / Number(a.PuntosMax));
            g.pts += p;
            g.max += Number(a.PuntosMax);
        });
        var suma = 0, pesos = 0;
        $.each(grupos, function (_, g) {
            var parte;
            if (g.comb === "promedio") parte = g.partes.reduce(function (s, x) { return s + x; }, 0) / g.partes.length;
            else if (g.comb === "mejor") parte = Math.max.apply(null, g.partes);
            else parte = g.max > 0 ? g.pts / g.max : 0;
            suma += parte * g.peso;
            pesos += g.peso;
        });
        if (!pesos) return null;
        var v = suma / pesos * 100;
        return { nota: Math.round(v * 100 + 1e-7) / 100, entera: Math.round(v + 1e-7) };
    }

    // ───────────────────────────── Nota directa vs. actividades ─────────────────────────────
    function mostrarDirecto(si) {
        // d-none en el botón (su columna lleva d-flex, que le gana a hidden y a d-none).
        $("#tabla-notas").closest(".statbox").toggleClass("d-none", !si);
        $("#btn-guardar-todo").toggleClass("d-none", !si);
    }

    function ocultarTodo() {
        C = null;
        celdas = {};
        tipos = {};
        $("#ev-resumen, #ev-error, #ev-panel").prop("hidden", true);
        $("#ev-resumen, #ev-error").empty();
        mostrarDirecto(true);
    }

    function barraPesos() {
        var html = '<div class="progress np-barra mt-3 mb-2" role="img" aria-label="Pesos de la plantilla">';
        $.each(C.Tipos, function (i, t) {
            html += '<div class="progress-bar np-c' + (i % 6) + '" style="width:' + Number(t.Peso) + '%"></div>';
        });
        html += '</div><ul class="list-unstyled np-tipos mb-0 row">';
        $.each(C.Tipos, function (i, t) {
            var n = C.Actividades.filter(function (a) { return t.Cet_Id && a.Cet_Id === t.Cet_Id && !a.NoCuenta; }).length;
            var uso = !C.UsaActividades ? "" : (n ? " · " + n + " actividad" + (n === 1 ? "" : "es")
                : ' · <span class="text-warning">sin actividades: todavía no cuenta</span>');
            html += '<li class="col-md-6 col-xl-4"><span class="np-punto np-c' + (i % 6) + '"></span>' + esc(t.Nombre) +
                ' <span class="np-peso">' + fmt(t.Peso) + ' %</span> <span class="text-muted small">(' +
                esc(COMBINACION[t.Combinacion] || t.Combinacion) + ')</span>' + uso + '</li>';
        });
        return html + '</ul>';
    }

    function renderResumen() {
        var $r = $("#ev-resumen");
        var bloqueado = cerrado();
        if (!C.Plantilla && !C.UsaActividades) {
            $r.html(bloqueado ? "" : alerta("secondary", '<i class="bi bi-info-circle mr-1" aria-hidden="true"></i>' +
                '<strong>Nota directa.</strong> Todavía no hay una plantilla de evaluación para este nivel en ' + esc(C.Anio) +
                '. Cuando Dirección la defina (Académico › Plantillas de evaluación) podrá calcular la nota con actividades.'))
              .prop("hidden", bloqueado);
            return;
        }
        var titulo, texto, botones = "";
        if (!C.UsaActividades) {
            titulo = "Nota directa en este parcial";
            texto = "Si prefiere que la nota salga de sus actividades (exámenes, tareas, trabajos…), agregue la primera: " +
                "la nota del parcial se calculará con estos pesos de la plantilla «" + esc(C.Plantilla.Nombre) + "».";
            if (!bloqueado)
                botones = '<button type="button" class="btn btn-primary" id="ev-agregar"><i class="bi bi-plus-lg mr-1" aria-hidden="true"></i> Agregar primera actividad</button>';
        } else {
            titulo = "Nota por actividades";
            texto = "La nota del parcial se calcula con los puntos y los pesos de la plantilla «" + esc(C.Plantilla ? C.Plantilla.Nombre : "") +
                "» y se guarda sola en las notas del parcial (boletín) cada vez que guarda los puntos.";
            if (!bloqueado) {
                botones = '<button type="button" class="btn btn-primary mb-1" id="ev-agregar"><i class="bi bi-plus-lg mr-1" aria-hidden="true"></i> Agregar actividad</button>';
                var desact = C.Alumnos.some(function (a) { return a.Entera !== null && a.NotaParcial !== a.Entera; });
                if (desact)
                    botones += ' <button type="button" class="btn btn-outline-primary mb-1 ml-1" id="ev-actualizar"><i class="bi bi-arrow-repeat mr-1" aria-hidden="true"></i> Actualizar notas del parcial</button>';
            }
        }
        var vigente = C.PlantillaVigente
            ? '<p class="small text-muted mb-0 mt-2"><i class="bi bi-info-circle mr-1" aria-hidden="true"></i>Este parcial empezó con la plantilla «' +
              esc(C.Plantilla.Nombre) + '». Dirección definió después «' + esc(C.PlantillaVigente.Nombre) +
              '» para esta materia o nivel: se aplica a los parciales que todavía no tienen actividades.</p>'
            : "";
        $r.html('<div class="statbox widget box box-shadow mb-4 np-tarjeta"><div class="widget-content widget-content-area">' +
            '<div class="d-flex flex-wrap justify-content-between align-items-start">' +
            '<div class="mr-3 mb-2"><h5 class="mb-1">' + titulo + '</h5><p class="text-muted mb-0">' + texto + '</p></div>' +
            '<div class="mb-2">' + botones + '</div></div>' + barraPesos() + vigente + '</div></div>').prop("hidden", false);
    }

    // ───────────────────────────── Planilla ─────────────────────────────
    function gruposActividades() {
        var grupos = [];
        $.each(C.Tipos, function (i, t) {
            var acts = C.Actividades.filter(function (a) { return a.Cet_Id === t.Cet_Id; });
            if (acts.length) grupos.push({ t: t, i: i, acts: acts });
        });
        return grupos;
    }

    function celdaNota(alu) {
        var r = calcular(alu.Alu_Id);
        if (!r) return '<span class="text-muted" title="Sin nada calificado todavía">—</span>';
        var rojo = r.entera < notaMinima ? " text-danger" : "";
        return '<strong class="' + rojo + '">' + r.entera + '</strong> <span class="text-muted small">(' + fmt(r.nota) + ')</span>';
    }

    function celdaParcial(alu) {
        var v = alu.NotaParcial;
        if (v === null || v === undefined) return '<span class="text-muted">—</span>';
        if (alu.Entera === null || alu.Entera === undefined)
            return '<span class="text-muted" title="Nota escrita antes de usar actividades: no cambia mientras el alumno no tenga nada calificado">' + esc(v) + '</span>';
        if (Number(v) !== Number(alu.Entera))
            return esc(v) + ' <span class="badge outline-badge-warning" title="Distinta a la calculada: guarde o pulse «Actualizar notas del parcial»">por actualizar</span>';
        return esc(v);
    }

    function renderPlanilla() {
        var grupos = gruposActividades();
        var bloqueado = cerrado();
        var h1 = '<tr><th rowspan="2" class="np-fija align-bottom">Alumno</th>';
        var h2 = "<tr>";
        $.each(grupos, function (_, g) {
            h1 += '<th colspan="' + g.acts.length + '" class="text-center np-b' + (g.i % 6) + '">' + esc(g.t.Nombre) + ' · ' + fmt(g.t.Peso) + ' %</th>';
            $.each(g.acts, function (_, a) {
                var sub = "de " + fmt(a.PuntosMax) + " pts" + (a.Fecha ? " · " + fecha(a.Fecha) : "");
                var acciones = bloqueado ? "" :
                    '<div class="mt-1"><button type="button" class="btn btn-link btn-sm btn-menu" data-editar="' + a.Act_Id + '" title="Editar" aria-label="Editar ' + esc(a.Nombre) + '"><i class="bi bi-pencil"></i></button>' +
                    '<button type="button" class="btn btn-link btn-sm btn-menu" data-quitar="' + a.Act_Id + '" title="Quitar" aria-label="Quitar ' + esc(a.Nombre) + '"><i class="bi bi-trash"></i></button></div>';
                h2 += '<th class="np-act' + (a.NoCuenta ? " np-no-cuenta" : "") + '"' + (a.Descripcion ? ' title="' + esc(a.Descripcion) + '"' : "") + '>' +
                    '<span class="np-act-nombre">' + esc(a.Nombre) + '</span><span class="np-act-sub">' + esc(sub) + '</span>' +
                    (a.NoCuenta ? '<span class="badge outline-badge-secondary mt-1">No cuenta</span>' : "") + acciones + '</th>';
            });
        });
        h1 += '<th rowspan="2" class="text-center align-bottom">Nota calculada</th><th rowspan="2" class="text-center align-bottom">En el parcial</th></tr>';
        h2 += "</tr>";
        $("#ev-tabla thead").html(h1 + h2);

        var cuerpo = "";
        if (!C.Alumnos.length) {
            cuerpo = '<tr><td colspan="' + (C.Actividades.length + 3) + '" class="text-center text-muted">No hay alumnos activos en esta sección.</td></tr>';
        }
        $.each(C.Alumnos, function (_, alu) {
            cuerpo += '<tr data-alu="' + alu.Alu_Id + '"><td class="np-fija">' + esc(alu.Nombre) + '</td>';
            $.each(grupos, function (_, g) {
                $.each(g.acts, function (_, a) {
                    var c = celdas[clave(a.Act_Id, alu.Alu_Id)];
                    cuerpo += '<td class="text-center' + (a.NoCuenta ? " np-no-cuenta" : "") + '">' +
                        '<input type="text" class="form-control np-celda" inputmode="decimal" maxlength="7" autocomplete="off"' +
                        ' data-act="' + a.Act_Id + '" data-alu="' + alu.Alu_Id + '" value="' + esc(textoCelda(c.e, c.p)) + '"' +
                        ' aria-label="' + esc(a.Nombre + " de " + alu.Nombre) + '"' + (bloqueado ? " disabled" : "") + '></td>';
                });
            });
            cuerpo += '<td class="text-center np-nota" data-calc="' + alu.Alu_Id + '">' + celdaNota(alu) + '</td>' +
                '<td class="text-center np-nota">' + celdaParcial(alu) + '</td></tr>';
        });
        $("#ev-tabla tbody").html(cuerpo);

        var pie = '<tr><td class="np-fija">Calificados</td>';
        $.each(grupos, function (_, g) {
            $.each(g.acts, function (_, a) { pie += '<td class="text-center" data-pie="' + a.Act_Id + '"></td>'; });
        });
        pie += '<td colspan="2"></td></tr>';
        $("#ev-tabla tfoot").html(pie);

        $("#ev-tabla .np-celda").each(function () { pintarCelda($(this)); });
        actualizarPie();
        actualizarBotones();
        $("#ev-guardar, #ev-deshacer").prop("hidden", bloqueado);
        $("#ev-panel").prop("hidden", false);
    }

    function pintarCelda($i) {
        var c = celdas[clave($i.data("act"), $i.data("alu"))];
        $i.toggleClass("np-excusado", c.e === "excusado" && !c.invalida)
          .toggleClass("np-noentrego", c.e === "no_entrego" && !c.invalida)
          .toggleClass("np-cambiada", sucia(c) || c.invalida)
          .toggleClass("is-invalid", !!c.invalida);
    }

    function actualizarPie() {
        var total = C.Alumnos.length;
        $.each(C.Actividades, function (_, a) {
            var n = 0;
            $.each(C.Alumnos, function (_, alu) { if (celdas[clave(a.Act_Id, alu.Alu_Id)].e !== "vacio") n++; });
            $('[data-pie="' + a.Act_Id + '"]').text(n + " de " + total);
        });
    }

    function actualizarBotones() {
        var n = cambios(), m = invalidas();
        $("#ev-cambios").html(m ? '<span class="text-danger">' + m + ' celda(s) con error</span>' + (n ? " · " : "") : "")
            .append(n ? document.createTextNode(n + " cambio(s) sin guardar") : "")
            .toggleClass("d-none", !n && !m);
        $("#ev-guardar").prop("disabled", cerrado() || n === 0 || m > 0);
        $("#ev-deshacer").prop("disabled", n + m === 0);
    }

    // ───────────────────────────── Cargar ─────────────────────────────
    function cargar() {
        if (!ctx) return;
        $("#ev-error").prop("hidden", true).empty();
        $.getJSON(URL.cuaderno, { horId: ctx.Hor_Id, pacId: ctx.Par_Id })
            .done(function (r) {
                if (!r || r.success === false) {
                    ocultarTodo();
                    $("#ev-error").html(alerta("danger", esc((r && r.message) || "No se pudo cargar el cuaderno de actividades."))).prop("hidden", false);
                    return;
                }
                C = r;
                tipos = {};
                $.each(C.Tipos, function (_, t) { if (t.Cet_Id) tipos[t.Cet_Id] = t; });
                celdas = {};
                $.each(C.Actividades, function (_, a) {
                    $.each(C.Alumnos, function (_, alu) {
                        celdas[clave(a.Act_Id, alu.Alu_Id)] = { e: "vacio", p: null, v: 0, oe: "vacio", op: null, invalida: false };
                    });
                });
                $.each(C.Puntos, function (_, p) {
                    var c = celdas[clave(p.Act_Id, p.Alu_Id)];
                    if (!c) return;
                    var pts = p.Puntos === null || p.Puntos === undefined ? null : Number(p.Puntos);
                    c.e = c.oe = p.Estado;
                    c.p = c.op = pts;
                    c.v = p.Version;
                });
                renderResumen();
                if (C.UsaActividades) {
                    mostrarDirecto(false);
                    renderPlanilla();
                } else {
                    mostrarDirecto(true);
                    $("#ev-panel").prop("hidden", true);
                }
            })
            .fail(function () {
                ocultarTodo();
                $("#ev-error").html(alerta("danger", "No se pudo cargar el cuaderno de actividades. Intente de nuevo.")).prop("hidden", false);
            });
    }

    function mostrarErrorGuardar(mensaje) {
        $("#ev-error").html('<div class="alert alert-light-danger mb-4 d-flex flex-wrap align-items-center justify-content-between" role="alert">' +
            '<span class="mr-3">' + esc(mensaje) + '</span>' +
            '<button type="button" class="btn btn-sm btn-outline-danger mt-1 mt-md-0" id="ev-recargar"><i class="bi bi-arrow-clockwise mr-1" aria-hidden="true"></i> Recargar</button></div>')
            .prop("hidden", false);
    }

    // ───────────────────────────── Editar celdas ─────────────────────────────
    $(document).on("input", "#ev-tabla .np-celda", function () {
        var $i = $(this);
        var act = C.Actividades.filter(function (a) { return a.Act_Id === $i.data("act"); })[0];
        var c = celdas[clave($i.data("act"), $i.data("alu"))];
        var r = leer($i.val(), Number(act.PuntosMax));
        if (r.error) {
            c.invalida = true;
            $i.attr("title", r.error);
        } else {
            c.invalida = false;
            c.e = r.e;
            c.p = r.p;
            $i.removeAttr("title");
        }
        pintarCelda($i);
        var alu = C.Alumnos.filter(function (a) { return a.Alu_Id === $i.data("alu"); })[0];
        $('[data-calc="' + alu.Alu_Id + '"]').html(celdaNota(alu));
        actualizarPie();
        actualizarBotones();
    });

    // Enter / flechas: misma actividad, alumno siguiente o anterior.
    $(document).on("keydown", "#ev-tabla .np-celda", function (e) {
        var abajo = (e.key === "Enter" && !e.shiftKey) || e.key === "ArrowDown";
        var arriba = (e.key === "Enter" && e.shiftKey) || e.key === "ArrowUp";
        if (!abajo && !arriba) return;
        e.preventDefault();
        var $td = $(this).closest("td"), col = $td.index();
        var $tr = abajo ? $td.closest("tr").next() : $td.closest("tr").prev();
        $tr.children().eq(col).find(".np-celda").trigger("focus").trigger("select");
    });

    function deshacer() {
        $.each(celdas, function (_, c) { c.e = c.oe; c.p = c.op; c.invalida = false; });
        if (C && C.UsaActividades) renderPlanilla();
    }
    $("#ev-deshacer").on("click", deshacer);

    $("#ev-guardar").on("click", function () {
        if (!C || cerrado()) return;
        if (invalidas()) { toastr.error("Corrija las celdas marcadas en rojo antes de guardar."); return; }
        var lista = [];
        $.each(celdas, function (k, c) {
            if (!sucia(c)) return;
            var partes = k.split("|");
            lista.push({ a: +partes[0], u: +partes[1], e: c.e, p: c.e === "calificado" ? c.p : null, v: c.v });
        });
        if (!lista.length) { toastr.info("No hay cambios para guardar."); return; }
        var $b = $(this).prop("disabled", true);
        var html = $b.html();
        $b.html('<span class="spinner-border spinner-border-sm mr-1" role="status" aria-hidden="true"></span> Guardando…');
        enviar(URL.guardarPuntos, { Cev_Id: C.Cev_Id, Puntos: JSON.stringify(lista) })
            .done(function (r) {
                if (!r || !r.success) { mostrarErrorGuardar((r && r.message) || "No se pudieron guardar los puntos."); return; }
                toastr.success(lista.length + " calificación(es) guardada(s)." +
                    (r.notasEscritas ? " Se actualizaron " + r.notasEscritas + " nota(s) del parcial." : ""));
                if (r.avisoNotas) toastr.warning(r.avisoNotas);
                cargar();
            })
            .fail(function () { mostrarErrorGuardar("No se pudo conectar con el servidor. Sus cambios siguen en pantalla: intente de nuevo."); })
            .always(function () { $b.html(html); actualizarBotones(); });
    });

    $(document).on("click", "#ev-recargar", function () { deshacer(); cargar(); });

    $(document).on("click", "#ev-actualizar", function () {
        var $b = $(this).prop("disabled", true);
        $.ajax({ url: URL.actualizar + "?cevId=" + encodeURIComponent(C.Cev_Id), type: "POST", dataType: "json",
                 headers: { RequestVerificationToken: token() } })
            .done(function (r) {
                if (!r || !r.success) { toastr.error((r && r.message) || "No se pudieron actualizar las notas del parcial."); return; }
                toastr.success(r.notasEscritas ? "Se actualizaron " + r.notasEscritas + " nota(s) del parcial." : "Las notas del parcial ya estaban al día.");
                if (r.avisoNotas) toastr.warning(r.avisoNotas);
                cargar();
            })
            .fail(function () { toastr.error("No se pudo conectar con el servidor."); })
            .always(function () { $b.prop("disabled", false); });
    });

    // ───────────────────────────── Actividad: crear / editar ─────────────────────────────
    function opcionesTipo(seleccion) {
        var $s = $("#ev-tipo").empty().append('<option value="">-- Elija --</option>');
        $.each(C.Tipos, function (_, t) {
            var v = t.Cet_Id ? "c:" + t.Cet_Id : "p:" + t.Plt_Id;
            $s.append($("<option>").val(v).text(t.Nombre + " (" + fmt(t.Peso) + " %)").prop("selected", v === seleccion));
        });
    }

    function abrirActividad(act) {
        if (!C || cerrado()) return;
        if (hayCambios()) { toastr.warning("Guarde o deshaga los cambios de puntos antes de agregar o editar actividades."); return; }
        editando = act || null;
        var $f = $("#ev-form");
        $f.find(".is-invalid").removeClass("is-invalid");
        $("#ev-form-error").prop("hidden", true).empty();
        $("#ev-modal-titulo").text(act ? "Editar actividad" : "Nueva actividad");
        $("#ev-form-grupo").text(C.Grado + " " + C.Seccion + " · " + C.Materia + " · " + C.Parcial + " (" + String(C.Semestre || "").toLowerCase() + " " + C.Anio + ")");
        opcionesTipo(act ? "c:" + act.Cet_Id : (C.Tipos.length === 1 ? (C.Tipos[0].Cet_Id ? "c:" + C.Tipos[0].Cet_Id : "p:" + C.Tipos[0].Plt_Id) : ""));
        $("#ev-nombre").val(act ? act.Nombre : "");
        $("#ev-max").val(act ? Number(act.PuntosMax) : "");
        $("#ev-fecha").val(act && act.Fecha ? String(act.Fecha).substring(0, 10) : "")
            .attr({ min: C.Anio + "-01-01", max: C.Anio + "-12-31" });
        $("#ev-descripcion").val(act ? (act.Descripcion || "") : "");
        $("#ev-nocuenta").prop("checked", !!(act && act.NoCuenta));
        var otras = act ? [] : (C.OtrasSecciones || []);
        $("#ev-otras").html(otras.map(function (o) {
            return '<div class="custom-control custom-checkbox custom-control-inline mb-1">' +
                '<input type="checkbox" class="custom-control-input" id="ev-otra-' + o.Hor_Id + '" value="' + o.Hor_Id + '">' +
                '<label class="custom-control-label" for="ev-otra-' + o.Hor_Id + '">' + esc(o.Texto) + '</label></div>';
        }).join(""));
        $("#ev-otras-grupo").prop("hidden", !otras.length);
        $("#ev-llenar-grupo").prop("hidden", !act);
        $("#ev-modal").modal("show");
    }

    function validarCampo(id) {
        var $c = $("#" + id), v = $.trim($c.val()), ok = true;
        if (id === "ev-nombre") ok = v.length > 0;
        if (id === "ev-tipo") ok = v.length > 0;
        if (id === "ev-max") {
            var n = parseFloat(String(v).replace(",", "."));
            ok = /^\d+([.,]\d{1,2})?$/.test(v) && n > 0 && n <= 1000;
        }
        if (id === "ev-fecha") ok = !v || v.substring(0, 4) === String(C.Anio);
        $c.toggleClass("is-invalid", !ok);
        return ok;
    }
    $("#ev-nombre, #ev-tipo, #ev-max, #ev-fecha").on("input change blur", function () {
        if ($(this).hasClass("is-invalid") || $.trim($(this).val())) validarCampo(this.id);
    });

    $("#ev-form").on("submit", function (e) {
        e.preventDefault();
        var ok = ["ev-nombre", "ev-tipo", "ev-max", "ev-fecha"].map(validarCampo).every(Boolean);
        if (!ok) return;
        var tipo = $("#ev-tipo").val().split(":");
        var datos = {
            Act_Id: editando ? editando.Act_Id : 0,
            Hor_Id: ctx.Hor_Id, Pac_Id: ctx.Par_Id,
            Cet_Id: tipo[0] === "c" ? +tipo[1] : null,
            Plt_Id: tipo[0] === "p" ? +tipo[1] : null,
            Nombre: $.trim($("#ev-nombre").val()),
            Descripcion: $.trim($("#ev-descripcion").val()) || null,
            PuntosMax: parseFloat(String($("#ev-max").val()).replace(",", ".")),
            Fecha: $("#ev-fecha").val() || null,
            NoCuenta: $("#ev-nocuenta").is(":checked"),
            Version: editando ? editando.Version : 0,
            Otras: JSON.stringify($("#ev-otras input:checked").map(function () { return +this.value; }).get())
        };
        var $b = $("#ev-form-guardar").prop("disabled", true);
        enviar(URL.guardarActividad, datos)
            .done(function (r) {
                if (!r || !r.success) {
                    $("#ev-form-error").html(alerta("danger", esc((r && r.message) || "No se pudo guardar la actividad."))).prop("hidden", false);
                    return;
                }
                $("#ev-modal").modal("hide");
                toastr.success(r.message || "Actividad guardada.");
                if (r.avisoNotas) toastr.warning(r.avisoNotas);
                cargar();
            })
            .fail(function () {
                $("#ev-form-error").html(alerta("danger", "No se pudo conectar con el servidor. Intente de nuevo.")).prop("hidden", false);
            })
            .always(function () { $b.prop("disabled", false); });
    });

    // Llenar las celdas vacías de la actividad que se edita (sin guardar).
    $("#ev-llenar-grupo [data-llenar]").on("click", function () {
        if (!editando) return;
        var modo = $(this).data("llenar"), n = 0, act = editando;
        $.each(C.Alumnos, function (_, alu) {
            var c = celdas[clave(act.Act_Id, alu.Alu_Id)];
            if (c.e !== "vacio" || c.invalida) return;
            if (modo === "maximo") { c.e = "calificado"; c.p = Number(act.PuntosMax); }
            else { c.e = "no_entrego"; c.p = null; }
            n++;
        });
        $("#ev-modal").modal("hide");
        renderPlanilla();
        if (n) toastr.info("Se llenaron " + n + " celda(s) de «" + act.Nombre + "». Revíselas y pulse «Guardar puntos».");
        else toastr.info("Esa actividad no tiene celdas vacías.");
    });

    $(document).on("click", "#ev-agregar", function () { abrirActividad(null); });
    $(document).on("click", "[data-editar]", function () {
        var id = +$(this).data("editar");
        abrirActividad(C.Actividades.filter(function (a) { return a.Act_Id === id; })[0]);
    });

    // ───────────────────────────── Quitar actividad ─────────────────────────────
    $(document).on("click", "[data-quitar]", function () {
        if (hayCambios()) { toastr.warning("Guarde o deshaga los cambios de puntos antes de quitar una actividad."); return; }
        var id = +$(this).data("quitar");
        quitando = C.Actividades.filter(function (a) { return a.Act_Id === id; })[0];
        if (!quitando) return;
        var n = quitando.Calificados || 0;
        $("#ev-quitar-texto").html("Se quitará <strong>" + esc(quitando.Nombre) + "</strong>." +
            (n ? " Tiene " + n + " calificación(es): dejarán de contar y la nota del parcial se vuelve a calcular." : " Todavía no tiene calificaciones."));
        $("#ev-modal-quitar").modal("show");
    });
    $("#ev-quitar-confirmar").on("click", function () {
        if (!quitando) return;
        var $b = $(this).prop("disabled", true);
        enviar(URL.quitarActividad, { Act_Id: quitando.Act_Id, Version: quitando.Version })
            .done(function (r) {
                $("#ev-modal-quitar").modal("hide");
                if (!r || !r.success) { mostrarErrorGuardar((r && r.message) || "No se pudo quitar la actividad."); return; }
                toastr.success("Actividad quitada.");
                if (r.avisoNotas) toastr.warning(r.avisoNotas);
                cargar();
            })
            .fail(function () { toastr.error("No se pudo conectar con el servidor."); })
            .always(function () { $b.prop("disabled", false); quitando = null; });
    });

    // ───────────────────────────── Cambios sin guardar ─────────────────────────────
    function pedirDescartar(accion) {
        pendiente = accion;
        $("#ev-descartar-texto").text("Tiene " + (cambios() + invalidas()) + " cambio(s) de puntos sin guardar. Si continúa, se pierden.");
        $("#ev-modal-descartar").modal("show");
    }
    $("#ev-descartar-confirmar").on("click", function () {
        $("#ev-modal-descartar").modal("hide");
        $.each(celdas, function (_, c) { c.e = c.oe; c.p = c.op; c.invalida = false; });
        var a = pendiente;
        pendiente = null;
        if (a) a();
    });

    // Escuchas en fase de captura (corren antes que los de notasadm.js, sin importar el orden de carga):
    // frenan el cambio de semestre, materia o parcial, y "Cargar", si hay puntos sin guardar.
    var FILTROS = ["f-sem", "f-hor", "f-par"];
    function recordar(e) { if (e.target && FILTROS.indexOf(e.target.id) >= 0) selPrevio[e.target.id] = e.target.value; }
    document.addEventListener("focusin", recordar, true);
    document.addEventListener("mousedown", recordar, true);
    document.addEventListener("change", function (e) {
        var t = e.target;
        if (!t || FILTROS.indexOf(t.id) < 0 || !hayCambios()) return;
        var id = t.id, nuevo = t.value;
        t.value = selPrevio[id] !== undefined ? selPrevio[id] : t.value;
        e.stopPropagation();
        pedirDescartar(function () { $("#" + id).val(nuevo).trigger("change"); });
    }, true);
    document.addEventListener("click", function (e) {
        var b = e.target && e.target.closest ? e.target.closest("#btn-cargar") : null;
        if (!b || !hayCambios()) return;
        e.stopPropagation();
        e.preventDefault();
        pedirDescartar(function () { $("#btn-cargar").trigger("click"); });
    }, true);
    window.addEventListener("beforeunload", function (e) {
        if (C && hayCambios()) { e.preventDefault(); e.returnValue = ""; }
    });

    // ───────────────────────────── Eventos de notasadm.js ─────────────────────────────
    $(document).on("cuaderno:limpiar", function () { ctx = null; ocultarTodo(); });
    $(document).on("cuaderno:cargar", function (_, contexto) {
        ctx = contexto ? $.extend({}, contexto) : null;
        ocultarTodo();
        cargar();
    });
    // Enlace directo desde "Uso en los cuadernos": /NotasAdm?sem=1&hor=123&par=2
    $(document).on("cuaderno:listo", function () {
        var q = new URLSearchParams(window.location.search);
        var sem = q.get("sem"), hor = q.get("hor"), par = q.get("par");
        if (!sem || !hor || !par) return;
        $("#f-sem").val(sem).trigger("change");
        if ($("#f-hor").val(hor).val() !== hor) return;
        $("#f-par").val(par);
        if ($("#f-par").val() === par) $("#btn-cargar").trigger("click");
    });

    if (URL.notaMinima) {
        $.getJSON(URL.notaMinima).done(function (r) {
            if (r && r.valor !== undefined && !isNaN(parseFloat(r.valor))) notaMinima = parseFloat(r.valor);
        });
    }
}(jQuery));
