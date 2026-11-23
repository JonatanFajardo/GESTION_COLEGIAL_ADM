/* Sustituciones de docentes — día a día (selector de fecha, indicadores, una tarjeta por docente
   ausente con sus clases y quién las cubre, asignar suplente con sugerencias, cancelar, marcar y
   quitar ausencias) e historial (rango, horas por suplente, tabla GcTabla, Excel).
   Quien solo ve lo suyo (docente) recibe del servidor solo sus filas y no tiene acciones.
   Todo texto que escribió una persona se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    var $app = $("#sus-app");
    if (!$app.length) return;

    var URL = {
        dia: $app.data("url-dia"), sugerencias: $app.data("url-sugerencias"), docentes: $app.data("url-docentes"),
        historial: $app.data("url-historial"), asignar: $app.data("url-asignar"), cancelar: $app.data("url-cancelar"),
        marcar: $app.data("url-marcar"), quitar: $app.data("url-quitar")
    };
    var ASIGNA = String($app.data("asigna")) === "1";
    var TODAS = String($app.data("todas")) === "1";
    var HOY = String($app.data("hoy"));
    var INICIO_MES = String($app.data("inicio-mes"));

    /* ───────────────────────────── utilidades ───────────────────────────── */

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function normalizar(s) {
        return String(s || "").normalize("NFD").replace(/[̀-ͯ]/g, "").toLowerCase();
    }

    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    var DIAS = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];
    function iso(v) { var m = /^(\d{4}-\d{2}-\d{2})/.exec(v || ""); return m ? m[1] : ""; }
    function fechaCorta(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        if (!m) return "";
        var d = new Date(+m[1], +m[2] - 1, +m[3]);
        return DIAS[d.getDay()] + " " + (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1];
    }
    function sumarDias(v, n) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v);
        var d = new Date(+m[1], +m[2] - 1, +m[3] + n);
        return d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2);
    }

    /* Minutos del día → "7:00–7:40 a. m." */
    function reloj(min) {
        var h = Math.floor(min / 60), m = min % 60, h12 = h % 12 === 0 ? 12 : h % 12;
        return { t: h12 + ":" + ("0" + m).slice(-2), s: h >= 12 ? "p. m." : "a. m." };
    }
    function rango(ini, fin) {
        if (ini === null || ini === undefined || fin === null || fin === undefined) return "—";
        var a = reloj(ini), b = reloj(fin);
        return a.s === b.s ? a.t + "–" + b.t + " " + b.s : a.t + " " + a.s + " – " + b.t + " " + b.s;
    }
    function horas(min) {
        var h = (min || 0) / 60;
        return h.toLocaleString("es-HN", { minimumFractionDigits: 0, maximumFractionDigits: 1 });
    }
    function grupo(f) {
        var g = [f.Grado, f.Seccion].filter(Boolean).join(" ");
        return f.Modalidad ? (g ? g + " · " + f.Modalidad : f.Modalidad) : g;
    }
    function iniciales(nombre) {
        return String(nombre || "").split(" ").filter(Boolean).slice(0, 2)
            .map(function (p) { return p.charAt(0); }).join("").toUpperCase();
    }

    var ORIGENES = {
        vacaciones: { texto: "Vacaciones", clase: "badge-info" },
        permiso: { texto: "Permiso", clase: "badge-primary" },
        incapacidad: { texto: "Incapacidad", clase: "badge-danger" },
        asistencia: { texto: "Ausencia registrada", clase: "badge-warning" },
        manual: { texto: "Marcada a mano", clase: "badge-secondary" }
    };
    function origenTexto(o) { return (ORIGENES[o] || { texto: "Ausencia" }).texto; }
    function origenBadge(o) {
        var x = ORIGENES[o] || { texto: "Ausencia", clase: "badge-secondary" };
        return '<span class="badge ' + x.clase + '">' + esc(x.texto) + "</span>";
    }

    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }
    function enviar(url, datos) {
        return $.ajax({
            url: url, type: "POST", contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos), headers: { RequestVerificationToken: token() }, dataType: "json"
        });
    }
    function avisar(tipo, texto) {
        if (window.toastr) window.toastr[tipo](texto); else window.alert(texto);
    }
    function error($p, texto) {
        if (!texto) { $p.attr("hidden", true).text(""); return; }
        $p.text(texto).removeAttr("hidden");
    }
    function vacio(icono, texto) {
        return '<div class="text-center text-muted py-4"><i class="bi ' + icono + ' d-block mb-2 sus-vacio-icono" aria-hidden="true"></i>' + esc(texto) + "</div>";
    }

    /* ───────────────────────────── día a día ───────────────────────────── */

    var estado = { fecha: String($app.data("fecha")), datos: null, clases: {} };
    var $fecha = $("#sus-fecha");

    function cargarDia() {
        var f = estado.fecha;
        $fecha.val(f);
        $("#sus-ausentes").html('<div class="col-12">' + vacio("bi-hourglass-split", "Cargando…") + "</div>");
        $.getJSON(URL.dia, { fecha: f }).done(function (r) {
            if (!r || !r.success) { avisar("error", (r && r.message) || "No se pudo cargar el día."); return; }
            if (f !== estado.fecha) return;   // llegó tarde: ya se pidió otro día
            estado.datos = r;
            estado.clases = {};
            (r.filas || []).forEach(function (x) { if (x.Hor_Id) estado.clases[x.Hor_Id] = x; });
            try { history.replaceState(null, "", location.pathname + "?fecha=" + f); } catch (e) { /* sin historial */ }
            pintarDia();
        }).fail(function () { avisar("error", "No se pudo cargar el día. Revise la conexión."); });
    }

    function irA(f) { if (/^\d{4}-\d{2}-\d{2}$/.test(f || "")) { estado.fecha = f; cargarDia(); } }

    function agrupar(filas) {
        var orden = [], por = {};
        filas.forEach(function (x) {
            var k = x.Emp_IdAusente;
            if (!por[k]) {
                por[k] = { id: k, nombre: x.Ausente, origen: x.Origen, origenId: x.OrigenId, detalle: x.Detalle, clases: [] };
                orden.push(por[k]);
            }
            if (x.Hor_Id) por[k].clases.push(x);
        });
        orden.forEach(function (d) {
            d.cubiertas = d.clases.filter(function (c) { return c.Sus_Id; }).length;
            d.cerradas = d.clases.filter(function (c) { return !c.Sus_Id && c.Cierre; }).length;
            d.pendientes = d.clases.length - d.cubiertas - d.cerradas;
            d.texto = normalizar([d.nombre].concat(d.clases.map(function (c) { return c.Materia + " " + grupo(c) + " " + (c.Suplente || ""); })).join(" "));
        });
        return orden;
    }

    function pintarDia() {
        var r = estado.datos, filas = r.filas || [], mes = r.mes || {};
        var docentes = agrupar(filas);
        var clases = filas.filter(function (x) { return x.Hor_Id; });
        var sinCubrir = clases.filter(function (x) { return !x.Sus_Id && !x.Cierre; }).length;
        var cubiertas = clases.filter(function (x) { return x.Sus_Id; }).length;

        $("#sus-titulo").text(r.titulo || "");
        $("#kpi-1").text(TODAS ? docentes.length : (r.cubro || []).length);
        $("#kpi-2").text(sinCubrir);
        $("#kpi-3").text(cubiertas);
        $("#kpi-4").text(horas(mes.MinutosMes));
        $("#kpi-4-titulo").text((TODAS ? "Horas cubiertas en " : "Horas que cubrí en ") + (r.mesNombre || "el mes"));
        $("#kpi-4-texto").text((mes.ClasesMes || 0) + (mes.ClasesMes === 1 ? " clase" : " clases")
            + (TODAS ? " · " + (mes.SuplentesMes || 0) + " suplentes distintos" : " cubiertas este mes"));

        var avisos = [];
        if (r.esFinDeSemana && !clases.length) avisos.push("Es fin de semana: no hay clases en el horario.");
        var cierres = {};
        clases.forEach(function (x) { if (x.Cierre) cierres[x.Cierre] = 1; });
        Object.keys(cierres).forEach(function (c) { avisos.push("El colegio cierra este día por " + c + ": esas clases no necesitan suplente."); });
        if (r.esPasado && ASIGNA) avisos.push("Día pasado: se registran sustituciones de hasta 7 días atrás, y una sustitución de una clase que ya pasó no se cancela (queda para el pago de horas).");
        var $av = $("#sus-aviso");
        if (avisos.length) $av.html(avisos.map(function (a) { return "<div>" + esc(a) + "</div>"; }).join("")).removeAttr("hidden");
        else $av.attr("hidden", true).empty();

        if (!TODAS) pintarPropias(r);
        estado.docentes = docentes;
        pintarTarjetas();
    }

    function pintarTarjetas() {
        var docentes = estado.docentes || [];
        var q = normalizar($("#sus-buscar").val());
        var solo = $("#sus-solo-pendientes").is(":checked");
        var lista = docentes.filter(function (d) {
            return (!q || d.texto.indexOf(q) >= 0) && (!solo || d.pendientes > 0);
        });
        $("#sus-total").text(docentes.length);
        var $c = $("#sus-ausentes");
        if (!docentes.length) {
            $c.html('<div class="col-12"><div class="statbox widget box box-shadow"><div class="widget-content widget-content-area">'
                + vacio("bi-emoji-smile", TODAS ? "Nadie falta este día." : "No tiene ausencias registradas este día.") + "</div></div></div>");
            return;
        }
        if (!lista.length) {
            $c.html('<div class="col-12"><div class="statbox widget box box-shadow"><div class="widget-content widget-content-area">'
                + vacio("bi-search", "Ningún docente coincide con el filtro.") + "</div></div></div>");
            return;
        }
        $c.html(lista.map(tarjeta).join(""));
    }

    function tarjeta(d) {
        var resumen = d.clases.length
            ? d.clases.length + (d.clases.length === 1 ? " clase" : " clases") + " · " + d.cubiertas + " cubierta" + (d.cubiertas === 1 ? "" : "s")
              + (d.pendientes ? " · " + d.pendientes + " sin cubrir" : "")
            : "No tiene clases este día";
        var filas = d.clases.map(function (c) {
            var estadoHtml, accion = "";
            if (c.Sus_Id) {
                estadoHtml = '<span class="badge badge-success">Cubierta</span><span class="sus-sub">por ' + esc(c.Suplente) + "</span>";
                if (ASIGNA && c.PuedeCancelar)
                    accion = '<button type="button" class="btn btn-sm btn-outline-danger" data-accion="cancelar" data-hor="' + c.Hor_Id + '">Cancelar</button>';
            } else if (c.Cierre) {
                estadoHtml = '<span class="badge badge-secondary">Sin clase</span><span class="sus-sub">' + esc(c.Cierre) + "</span>";
            } else {
                estadoHtml = '<span class="badge badge-warning">Sin cubrir</span>';
                if (ASIGNA)
                    accion = '<button type="button" class="btn btn-sm btn-primary" data-accion="asignar" data-hor="' + c.Hor_Id + '"><i class="bi bi-person-plus mr-1" aria-hidden="true"></i>Asignar</button>';
            }
            return "<tr>"
                + '<td class="text-nowrap sus-hora">' + esc(rango(c.MinInicio, c.MinFin)) + "</td>"
                + '<td><span class="font-weight-bold">' + esc(c.Materia) + '</span><span class="sus-sub">' + esc(grupo(c))
                + (c.Aula ? " · Aula " + esc(c.Aula) : "") + "</span></td>"
                + "<td>" + estadoHtml + "</td>"
                + (ASIGNA ? '<td class="text-right text-nowrap">' + accion + "</td>" : "")
                + "</tr>";
        }).join("");
        var quitar = ASIGNA && d.origen === "manual" && d.origenId && d.cubiertas === 0
            ? '<button type="button" class="btn btn-link btn-sm text-danger px-0 mt-2" data-accion="quitar" data-aud="' + d.origenId + '" data-emp="' + d.id + '">'
              + '<i class="bi bi-x-circle mr-1" aria-hidden="true"></i>Quitar ausencia (marcada por error)</button>'
            : "";
        return '<div class="col-12 layout-spacing">'
            + '<article class="statbox widget box box-shadow h-100 sus-docente" aria-label="' + esc(d.nombre) + '">'
            + '<div class="widget-header"><div class="d-flex align-items-center flex-wrap px-2 py-3">'
            + '<span class="sus-avatar mr-3" aria-hidden="true">' + esc(iniciales(d.nombre)) + "</span>"
            + '<div class="mr-auto sus-min0"><h5 class="mb-0 sus-nombre">' + esc(d.nombre) + '</h5><span class="small text-muted">' + esc(resumen) + "</span></div>"
            + '<div class="ml-2 my-1">' + origenBadge(d.origen) + "</div>"
            + "</div></div>"
            + '<div class="widget-content widget-content-area pt-0">'
            + (d.detalle ? '<p class="small text-muted mb-2"><i class="bi bi-chat-left-text mr-1" aria-hidden="true"></i>' + esc(d.detalle) + "</p>" : "")
            + (d.clases.length
                ? '<div class="table-responsive"><table class="table table-sm mb-0 sus-clases"><caption class="sr-only">Clases de ' + esc(d.nombre) + "</caption>"
                  + '<thead><tr><th scope="col" class="sus-col-hora">Hora</th><th scope="col">Clase</th><th scope="col" class="sus-col-estado">Estado</th>' + (ASIGNA ? '<th scope="col" class="sus-col-accion"><span class="sr-only">Acción</span></th>' : "") + "</tr></thead>"
                  + "<tbody>" + filas + "</tbody></table></div>"
                : "")
            + quitar
            + "</div></article></div>";
    }

    function tablaPropias(lista, conFecha) {
        if (!lista.length) return "";
        return '<div class="table-responsive"><table class="table table-sm table-hover mb-0"><thead><tr>'
            + (conFecha ? '<th scope="col">Fecha</th>' : "")
            + '<th scope="col">Hora</th><th scope="col">Clase</th><th scope="col">En lugar de</th><th scope="col">Notas</th></tr></thead><tbody>'
            + lista.map(function (s) {
                return "<tr>" + (conFecha ? '<td class="text-nowrap">' + esc(fechaCorta(s.Sus_Fecha)) + "</td>" : "")
                    + '<td class="text-nowrap">' + esc(rango(s.MinInicio, s.MinFin)) + "</td>"
                    + '<td><span class="font-weight-bold">' + esc(s.Materia) + '</span><span class="sus-sub">' + esc(grupo(s)) + (s.Aula ? " · Aula " + esc(s.Aula) : "") + "</span></td>"
                    + "<td>" + esc(s.Ausente) + "</td>"
                    + '<td class="small">' + esc(s.Sus_Notas || "") + "</td></tr>";
            }).join("") + "</tbody></table></div>";
    }

    function pintarPropias(r) {
        var cubro = r.cubro || [], prox = r.proximas || [];
        $("#sus-cubro").html(cubro.length ? tablaPropias(cubro, false) : vacio("bi-cup-hot", "Este día no le toca cubrir ninguna clase."));
        $("#sus-proximas").html(prox.length ? tablaPropias(prox, true) : vacio("bi-calendar2-check", "No tiene sustituciones en las próximas dos semanas."));
    }

    $("#sus-anterior").on("click", function () { irA(sumarDias(estado.fecha, -1)); });
    $("#sus-siguiente").on("click", function () { irA(sumarDias(estado.fecha, 1)); });
    $("#sus-hoy").on("click", function () { irA(HOY); });
    $fecha.on("change", function () { irA($fecha.val()); });
    $("#sus-buscar").on("input", pintarTarjetas);
    $("#sus-solo-pendientes").on("change", pintarTarjetas);

    /* ───────────────────────────── asignar suplente ───────────────────────────── */

    var asig = { clase: null, lista: [], elegido: null, todos: false };
    var VISIBLES = 6;

    function abrirAsignar(horId) {
        var c = estado.clases[horId];
        if (!c) return;
        asig = { clase: c, lista: [], elegido: null, todos: false };
        $("#sus-asignar-clase").html(
            '<div class="d-flex flex-wrap align-items-center">'
            + '<div class="mr-auto"><div class="font-weight-bold">' + esc(c.Materia) + " · " + esc(grupo(c)) + "</div>"
            + '<div class="small text-muted">' + esc(fechaCorta(estado.fecha)) + " · " + esc(rango(c.MinInicio, c.MinFin))
            + (c.Aula ? " · Aula " + esc(c.Aula) : "") + "</div></div>"
            + '<div class="small text-right my-1">En lugar de <strong>' + esc(c.Ausente) + "</strong><br>" + origenBadge(c.Origen) + "</div></div>");
        $("#sus-notas").val("");
        $("#sus-buscar-suplente").val("");
        error($("#sus-asignar-error"), "");
        $("#sus-asignar-guardar").prop("disabled", true);
        $("#sus-sugerencias").html(vacio("bi-hourglass-split", "Buscando docentes libres…"));
        $("#sus-ver-todos").attr("hidden", true);
        $("#sus-modal-asignar").modal("show");
        cargarSugerencias();
    }

    function cargarSugerencias() {
        var c = asig.clase;
        $.getJSON(URL.sugerencias, { horId: c.Hor_Id, fecha: estado.fecha }).done(function (r) {
            asig.lista = (r && r.data) || [];
            if (asig.elegido && !asig.lista.some(function (x) { return x.Emp_Id === asig.elegido; })) asig.elegido = null;
            $("#sus-libres").text(asig.lista.length);
            pintarSugerencias();
        }).fail(function () { $("#sus-sugerencias").html(vacio("bi-wifi-off", "No se pudieron cargar los docentes libres.")); });
    }

    function razones(x, c) {
        var b = [];
        if (x.SabeMateria) b.push('<span class="badge outline-badge-success">Sabe dar ' + esc(c.Materia) + "</span>");
        if (x.MismoGrupo) b.push('<span class="badge outline-badge-info">Da clases a ' + esc([c.Grado, c.Seccion].filter(Boolean).join(" ")) + "</span>");
        else if (x.MismoGrado) b.push('<span class="badge outline-badge-info">Da clases en ' + esc(c.Grado) + "</span>");
        if (!b.length) b.push('<span class="badge outline-badge-secondary">Libre a esa hora</span>');
        return b.join(" ");
    }

    function pintarSugerencias() {
        var c = asig.clase, q = normalizar($("#sus-buscar-suplente").val());
        var lista = asig.lista.filter(function (x) { return !q || normalizar(x.Nombre).indexOf(q) >= 0; });
        var mostrar = q || asig.todos ? lista : lista.slice(0, VISIBLES);
        var $l = $("#sus-sugerencias");
        if (!asig.lista.length) {
            $l.html(vacio("bi-person-slash", "No hay docentes libres y presentes a esa hora."));
        } else if (!lista.length) {
            $l.html(vacio("bi-search", "Ningún docente libre coincide con la búsqueda."));
        } else {
            $l.html(mostrar.map(function (x) {
                var activo = x.Emp_Id === asig.elegido;
                return '<button type="button" class="list-group-item list-group-item-action sus-opcion' + (activo ? " active" : "") + '" role="radio" aria-checked="' + activo + '" data-emp="' + x.Emp_Id + '">'
                    + '<div class="d-flex align-items-center flex-wrap">'
                    + '<span class="sus-avatar sus-avatar-sm mr-2" aria-hidden="true">' + esc(iniciales(x.Nombre)) + "</span>"
                    + '<div class="mr-auto sus-min0"><div class="font-weight-bold">' + esc(x.Nombre)
                    + (x.Orden === 1 ? ' <span class="badge badge-primary ml-1">Sugerido</span>' : "") + "</div>"
                    + '<div class="small sus-sub-opcion">' + x.ClasesDia + (x.ClasesDia === 1 ? " clase" : " clases") + " ese día · "
                    + x.SustitucionesMes + (x.SustitucionesMes === 1 ? " sustitución" : " sustituciones") + " este mes</div></div>"
                    + '<div class="my-1 ml-2 text-right">' + razones(x, c) + "</div>"
                    + "</div></button>";
            }).join(""));
        }
        var $todos = $("#sus-ver-todos");
        if (!q && !asig.todos && lista.length > VISIBLES) $todos.text("Mostrar todos los libres (" + lista.length + ")").removeAttr("hidden");
        else $todos.attr("hidden", true);
        $("#sus-asignar-guardar").prop("disabled", !asig.elegido);
    }

    $("#sus-sugerencias").on("click", ".sus-opcion", function () {
        asig.elegido = +$(this).data("emp");
        pintarSugerencias();
    });
    $("#sus-buscar-suplente").on("input", pintarSugerencias);
    $("#sus-ver-todos").on("click", function () { asig.todos = true; pintarSugerencias(); });

    $("#sus-asignar-guardar").on("click", function () {
        var $b = $(this);
        if (!asig.elegido) return;
        $b.prop("disabled", true);
        enviar(URL.asignar, {
            Hor_Id: asig.clase.Hor_Id, Sus_Fecha: estado.fecha, Emp_IdSuplente: asig.elegido,
            Sus_Notas: $("#sus-notas").val()
        }).done(function (r) {
            if (r && r.success) {
                $("#sus-modal-asignar").modal("hide");
                avisar("success", r.message || "Se asignó el suplente.");
                cargarDia();
            } else {
                error($("#sus-asignar-error"), (r && r.message) || "No se pudo asignar el suplente.");
                cargarSugerencias();   // pudo cambiar la disponibilidad mientras tanto
            }
        }).fail(function () {
            error($("#sus-asignar-error"), "No se pudo conectar con el servidor. Intente de nuevo.");
            $b.prop("disabled", false);
        });
    });

    /* ───────────────────────────── confirmar: cancelar / quitar ───────────────────────────── */

    var confirmar = null;
    function abrirConfirmar(op) {
        confirmar = op;
        $("#sus-confirmar-titulo").text(op.titulo);
        $("#sus-confirmar-texto").text(op.texto);
        $("#sus-confirmar-motivo").val("");
        $("#sus-confirmar-motivo-grupo").toggle(!!op.conMotivo);
        $("#sus-confirmar-ok").text(op.boton).prop("disabled", false);
        error($("#sus-confirmar-error"), "");
        $("#sus-modal-confirmar").modal("show");
    }
    $("#sus-confirmar-ok").on("click", function () {
        if (!confirmar) return;
        var $b = $(this).prop("disabled", true);
        var datos = confirmar.datos();
        enviar(confirmar.url, datos).done(function (r) {
            if (r && r.success) {
                $("#sus-modal-confirmar").modal("hide");
                avisar("success", r.message || "Listo.");
                cargarDia();
            } else {
                error($("#sus-confirmar-error"), (r && r.message) || "No se pudo completar.");
                $b.prop("disabled", false);
            }
        }).fail(function () {
            error($("#sus-confirmar-error"), "No se pudo conectar con el servidor. Intente de nuevo.");
            $b.prop("disabled", false);
        });
    });

    $("#sus-ausentes").on("click", "[data-accion]", function () {
        var $b = $(this), accion = $b.data("accion");
        if (accion === "asignar") { abrirAsignar(+$b.data("hor")); return; }
        if (accion === "cancelar") {
            var c = estado.clases[+$b.data("hor")];
            if (!c) return;
            abrirConfirmar({
                titulo: "Cancelar sustitución", boton: "Cancelar sustitución", conMotivo: true, url: URL.cancelar,
                texto: "¿Cancelar la sustitución de " + c.Materia + " (" + grupo(c) + ") de " + rango(c.MinInicio, c.MinFin)
                    + "? " + c.Suplente + " ya no la cubrirá y la clase vuelve a quedar sin cubrir.",
                datos: function () { return { Sus_Id: c.Sus_Id, Motivo: $("#sus-confirmar-motivo").val() }; }
            });
            return;
        }
        if (accion === "quitar") {
            var aud = +$b.data("aud"), emp = +$b.data("emp");
            var d = (estado.docentes || []).filter(function (x) { return x.id === emp; })[0];
            abrirConfirmar({
                titulo: "Quitar ausencia", boton: "Quitar ausencia", conMotivo: false, url: URL.quitar,
                texto: "¿Quitar la ausencia de " + (d ? d.nombre : "este docente") + " del " + fechaCorta(estado.fecha)
                    + "? Úselo solo si se marcó por error.",
                datos: function () { return { Aud_Id: aud }; }
            });
        }
    });

    /* ───────────────────────────── marcar docente ausente ───────────────────────────── */

    var aus = { lista: [], elegido: null };
    function cargarDocentes() {
        var f = $("#sus-aus-fecha").val();
        aus.elegido = null;
        $("#sus-aus-lista").html(vacio("bi-hourglass-split", "Cargando docentes…"));
        $.getJSON(URL.docentes, { fecha: f }).done(function (r) {
            aus.lista = (r && r.data) || [];
            pintarDocentes();
        }).fail(function () { $("#sus-aus-lista").html(vacio("bi-wifi-off", "No se pudieron cargar los docentes.")); });
    }
    function pintarDocentes() {
        var q = normalizar($("#sus-aus-buscar").val());
        var lista = aus.lista.filter(function (x) { return !q || normalizar(x.Nombre).indexOf(q) >= 0; });
        var total = lista.length;
        lista = lista.slice(0, 40);
        $("#sus-aus-lista").html(lista.length ? lista.map(function (x) {
            var ya = !!x.AusenteOrigen, activo = x.Emp_Id === aus.elegido;
            return '<button type="button" class="list-group-item list-group-item-action sus-opcion' + (activo ? " active" : "") + '" role="radio" aria-checked="' + activo + '"'
                + (ya ? ' disabled aria-disabled="true"' : "") + ' data-emp="' + x.Emp_Id + '">'
                + '<div class="d-flex align-items-center"><span class="mr-auto">' + esc(x.Nombre) + "</span>"
                + '<span class="small sus-sub-opcion text-nowrap ml-2">'
                + (ya ? "Ya figura ausente (" + esc(origenTexto(x.AusenteOrigen).toLowerCase()) + ")"
                      : (x.ClasesDia ? x.ClasesDia + (x.ClasesDia === 1 ? " clase" : " clases") + " ese día" : "Sin clases ese día"))
                + "</span></div></button>";
        }).join("") + (total > lista.length ? '<div class="list-group-item small text-muted">Escriba para ver los demás (' + (total - lista.length) + " más).</div>" : "")
            : vacio("bi-search", "Ningún docente coincide."));
    }
    $("#sus-marcar").on("click", function () {
        $("#sus-aus-fecha").val(estado.fecha);
        $("#sus-aus-buscar").val("");
        $("#sus-aus-motivo").val("");
        error($("#sus-ausente-error"), "");
        $("#sus-ausente-guardar").prop("disabled", false);
        $("#sus-modal-ausente").modal("show");
        cargarDocentes();
    });
    $("#sus-aus-fecha").on("change", cargarDocentes);
    $("#sus-aus-buscar").on("input", pintarDocentes);
    $("#sus-aus-lista").on("click", ".sus-opcion:not([disabled])", function () {
        aus.elegido = +$(this).data("emp");
        pintarDocentes();
    });
    $("#sus-ausente-guardar").on("click", function () {
        var f = $("#sus-aus-fecha").val(), motivo = $.trim($("#sus-aus-motivo").val());
        if (!aus.elegido) { error($("#sus-ausente-error"), "Elija al docente."); return; }
        if (!motivo) { error($("#sus-ausente-error"), "Escriba el motivo de la ausencia."); $("#sus-aus-motivo").trigger("focus"); return; }
        var $b = $(this).prop("disabled", true);
        enviar(URL.marcar, { Emp_Id: aus.elegido, Aud_Fecha: f, Aud_Motivo: motivo }).done(function (r) {
            if (r && r.success) {
                $("#sus-modal-ausente").modal("hide");
                avisar("success", r.message || "Se marcó al docente ausente.");
                irA(f);
            } else {
                error($("#sus-ausente-error"), (r && r.message) || "No se pudo marcar la ausencia.");
                $b.prop("disabled", false);
            }
        }).fail(function () {
            error($("#sus-ausente-error"), "No se pudo conectar con el servidor. Intente de nuevo.");
            $b.prop("disabled", false);
        });
    });

    /* ───────────────────────────── historial ───────────────────────────── */

    var tabla = null, filasHist = [];
    var ESTADOS = { asignada: { texto: "Vigente", clase: "badge-success" }, cancelada: { texto: "Cancelada", clase: "badge-dark" } };

    function urlHistorial() {
        return URL.historial + "?desde=" + encodeURIComponent($("#sus-desde").val()) + "&hasta=" + encodeURIComponent($("#sus-hasta").val());
    }
    function rangoValido() {
        var d = $("#sus-desde").val(), h = $("#sus-hasta").val();
        if (!d || !h) return "Indique las dos fechas.";
        if (h < d) return "La fecha final no puede ser antes de la inicial.";
        if ((new Date(h) - new Date(d)) / 86400000 > 400) return "El rango admite hasta 400 días.";
        return null;
    }
    function suplenteElegido() { return $("#sus-suplente").val(); }
    function incluirCanceladas() { return $("#sus-canceladas").is(":checked"); }

    function pintarTotales() {
        var sup = suplenteElegido(), por = {}, orden = [];
        filasHist.forEach(function (s) {
            if (s.Sus_Estado !== "asignada") return;
            if (sup && String(s.Emp_IdSuplente) !== sup) return;
            if (!por[s.Emp_IdSuplente]) { por[s.Emp_IdSuplente] = { nombre: s.Suplente, clases: 0, min: 0 }; orden.push(por[s.Emp_IdSuplente]); }
            por[s.Emp_IdSuplente].clases++;
            por[s.Emp_IdSuplente].min += s.Sus_Minutos || 0;
        });
        orden.sort(function (a, b) { return b.min - a.min || a.nombre.localeCompare(b.nombre); });
        var tc = 0, tm = 0;
        $("#sus-totales").html(orden.length ? orden.map(function (x) {
            tc += x.clases; tm += x.min;
            return "<tr><td>" + esc(x.nombre) + '</td><td class="text-right">' + x.clases + '</td><td class="text-right">' + horas(x.min) + "</td></tr>";
        }).join("") : '<tr><td colspan="3" class="text-center text-muted py-3">Sin sustituciones en el rango.</td></tr>');
        $("#sus-totales-pie").html(orden.length
            ? '<tr class="font-weight-bold"><td>Total</td><td class="text-right">' + tc + '</td><td class="text-right">' + horas(tm) + "</td></tr>" : "");
    }

    function llenarSuplentes() {
        var $s = $("#sus-suplente"), actual = $s.val(), vistos = {}, ops = [];
        filasHist.forEach(function (s) { if (!vistos[s.Emp_IdSuplente]) { vistos[s.Emp_IdSuplente] = 1; ops.push({ id: s.Emp_IdSuplente, nombre: s.Suplente }); } });
        ops.sort(function (a, b) { return a.nombre.localeCompare(b.nombre); });
        $s.html('<option value="">Todos</option>' + ops.map(function (o) { return '<option value="' + o.id + '">' + esc(o.nombre) + "</option>"; }).join(""));
        if (actual && vistos[actual]) $s.val(actual);
    }

    function crearTabla() {
        tabla = GcTabla.crear({
            elemento: "#tabla-sustituciones", nombre: "sustituciones", url: urlHistorial(),
            archivo: "sustituciones", tituloPdf: "Sustituciones de docentes",
            vacio: "No hay sustituciones en el rango.",
            buscarEn: ["Materia", "Grupo", "Ausente", "Suplente", "OrigenTexto", "Sus_Notas"],
            ocultas: ["Aula", "Sus_Notas", "RegistradoPor", "Sus_MotivoCancelacion"],
            filtroExtra: function (s) {
                var sup = suplenteElegido();
                return (incluirCanceladas() || s.Sus_Estado === "asignada") && (!sup || String(s.Emp_IdSuplente) === sup);
            },
            alCargar: function (filas) {
                filasHist = filas || [];
                llenarSuplentes();
                pintarTotales();
                tabla && tabla.filtrar();
                contar();
            },
            formatoFila: function (fila) { if (fila.getData().Sus_Estado === "cancelada") fila.getElement().classList.add("sus-cancelada"); },
            columnas: [
                { title: "Fecha", field: "Sus_Fecha", mutator: function (v) { return iso(v); }, formatter: function (c) { return esc(fechaCorta(c.getValue()).replace(/ \d{4}$/, "")); }, minWidth: 115 },
                { title: "Hora", field: "Hora", mutator: function (v, d) { return rango(d.MinInicio, d.MinFin); }, headerSort: false, minWidth: 165 },
                { title: "Materia", field: "Materia", minWidth: 130 },
                { title: "Grado y sección", field: "Grupo", mutator: function (v, d) { return grupo(d); }, minWidth: 150 },
                { title: "Aula", field: "Aula" },
                { title: "Docente ausente", field: "Ausente", minWidth: 150 },
                { title: "Suplente", field: "Suplente", minWidth: 150 },
                { title: "Origen", field: "OrigenTexto", mutator: function (v, d) { return origenTexto(d.Sus_Origen); },
                  formatter: function (c) { return origenBadge(c.getData().Sus_Origen); }, minWidth: 140 },
                { title: "Horas", field: "Horas", mutator: function (v, d) { return Math.round((d.Sus_Minutos || 0) / 6) / 10; }, hozAlign: "right", width: 90 },
                { title: "Estado", field: "EstadoTexto", mutator: function (v, d) { return (ESTADOS[d.Sus_Estado] || { texto: d.Sus_Estado }).texto; },
                  formatter: function (c) { var e = ESTADOS[c.getData().Sus_Estado] || { texto: "", clase: "badge-secondary" }; return '<span class="badge ' + e.clase + '">' + esc(e.texto) + "</span>"; }, width: 110 },
                { title: "Notas", field: "Sus_Notas", formatter: function (c) { return esc(c.getValue() || ""); } },
                { title: "Motivo de cancelación", field: "Sus_MotivoCancelacion", formatter: function (c) { return esc(c.getValue() || ""); } },
                { title: "Registró", field: "RegistradoPor" }
            ]
        });
        tabla.tabla.on("dataFiltered", function (filtros, filas) { $("#sus-hist-total").text(filas.length); });
    }
    function contar() { if (tabla) setTimeout(function () { $("#sus-hist-total").text(tabla.tabla.getData("active").length); }, 0); }

    function recargarHistorial() {
        var problema = rangoValido();
        error($("#sus-rango-error"), problema);
        if (problema || !tabla) return;
        tabla.tabla.setData(urlHistorial());
    }

    $("#sus-tab-historial").on("shown.bs.tab", function () {
        if (!tabla) crearTabla(); else tabla.tabla.redraw(true);
    });
    $("#sus-desde, #sus-hasta").on("change", recargarHistorial);
    $("#sus-suplente").on("change", function () { pintarTotales(); tabla && tabla.filtrar(); });
    $("#sus-canceladas").on("change", function () { tabla && tabla.filtrar(); });
    $("[data-rango]").on("click", function () {
        var m = /^(\d{4})-(\d{2})/.exec(HOY), y = +m[1], mes = +m[2];
        if ($(this).data("rango") === "anterior") { mes -= 1; if (mes === 0) { mes = 12; y -= 1; } }
        var desde = y + "-" + ("0" + mes).slice(-2) + "-01";
        var fin = new Date(y, mes, 0).getDate();
        var hasta = $(this).data("rango") === "mes" ? HOY : y + "-" + ("0" + mes).slice(-2) + "-" + ("0" + fin).slice(-2);
        $("#sus-desde").val(desde); $("#sus-hasta").val(hasta);
        recargarHistorial();
    });
    $("#sus-exportar").on("click", function () {
        if (!tabla) crearTabla();
        tabla.tabla.download("xlsx", "sustituciones-" + $("#sus-desde").val() + "-a-" + $("#sus-hasta").val() + ".xlsx", { sheetName: "Sustituciones" }, "active");
    });

    if (!$("#sus-desde").val()) $("#sus-desde").val(INICIO_MES);
    cargarDia();
})(jQuery);
