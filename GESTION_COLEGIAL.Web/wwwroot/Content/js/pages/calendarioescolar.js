/* ============================================================
   Calendario escolar (espejo de Skuul "Calendar")
   - initMes: filtros del mes y carga de feriados de Honduras.
   - initDia: editor de un día (todo el día, tipo que cierra, para quién,
     buscar personas, guardar / publicar / volver a borrador / quitar).
   ============================================================ */
var CalendarioEscolar = (function () {
    var obj = {};
    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];

    function esc(v) {
        if (v === null || v === undefined) return "";
        return String(v)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function token() {
        return $('input[name="__RequestVerificationToken"]').first().val();
    }

    function postJson(url, datos) {
        return $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos),
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    function post(url, parametros) {
        return $.ajax({
            url: url + (url.indexOf("?") < 0 ? "?" : "&") + $.param(parametros),
            type: "POST",
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    /* toastr inserta HTML: siempre se le pasa texto escapado */
    function avisar(tipo, texto) {
        if (window.toastr) toastr[tipo](esc(texto));
        else window.alert(texto);
    }

    /* "2026-10-07T00:00:00" → {d:7, m:9, a:2026} */
    function partes(iso) {
        var s = String(iso || "").substr(0, 10).split("-");
        return { a: +s[0], m: +s[1] - 1, d: +s[2] };
    }

    function rango(ini, fin) {
        var i = partes(ini), f = partes(fin);
        if (i.a === f.a && i.m === f.m && i.d === f.d) return i.d + " " + MESES[i.m];
        if (i.m === f.m) return i.d + "–" + f.d + " " + MESES[f.m];
        return i.d + " " + MESES[i.m] + " – " + f.d + " " + MESES[f.m];
    }

    /* ───────────────────────── Vista de mes ───────────────────────── */
    obj.initMes = function (cfg) {
        $("#cal-filtro-tipo, #cal-filtro-borradores").on("change", function () {
            $("#cal-filtros").trigger("submit");
        });

        if (!cfg.puedeEditar) return;

        var tipos = {};
        $.each(cfg.tipos || [], function (_, t) { tipos[t.Codigo] = t; });

        var anio = parseInt($("#cal-btn-feriados").data("anio"), 10) || new Date().getFullYear();
        var $lista = $("#cal-feriados-lista");
        var $cargar = $("#cal-feriados-cargar");

        function actualizarBoton() {
            var n = $lista.find("input[type=checkbox]:checked:not(:disabled)").length;
            $cargar.prop("disabled", n === 0).text(n === 0 ? "Cargar" : "Cargar " + n + (n === 1 ? " día" : " días"));
        }

        function fila(f) {
            var t = tipos[f.Tipo] || { Etiqueta: f.Tipo, Clase: "cal-t-otro", Icono: "bi-calendar-event" };
            var marcado = f.Sugerido && !f.YaCargado;
            return '<label class="cal-feriado' + (f.YaCargado ? " cal-feriado--cargado" : "") + '">' +
                '<input type="checkbox" value="' + esc(f.Clave) + '" data-sugerido="' + (f.Sugerido ? 1 : 0) + '" data-verificar="' + (f.Verificar ? 1 : 0) + '"' +
                (marcado ? " checked" : "") + (f.YaCargado ? " disabled" : "") + ">" +
                '<span class="cal-feriado-fecha">' + esc(rango(f.Inicio, f.Fin)) + "</span>" +
                '<span class="cal-feriado-texto"><strong>' + esc(f.Titulo) + "</strong><small>" + esc(f.Descripcion || "") + "</small></span>" +
                '<span class="d-flex flex-column align-items-end" style="gap:4px">' +
                '<span class="cal-tipo ' + esc(t.Clase) + '"><i class="bi ' + esc(t.Icono) + '" aria-hidden="true"></i> ' + esc(t.Etiqueta) + "</span>" +
                (f.YaCargado
                    ? '<span class="cal-badge-borrador">' + (f.Publicado ? "Ya está publicado" : "Ya está (borrador)") + "</span>"
                    : (f.Verificar ? '<span class="cal-verificar"><i class="bi bi-exclamation-triangle" aria-hidden="true"></i> Confirmar fecha</span>' : "")) +
                "</span></label>";
        }

        function pintar(datos) {
            if (!datos || datos.length === 0) {
                $lista.html('<p class="cal-cargando">No hay feriados para este año.</p>');
                actualizarBoton();
                return;
            }
            var html = "", grupo = null;
            $.each(datos, function (_, f) { if (f.Grupo === "Feriado nacional") { if (grupo !== 1) { html += '<p class="cal-feriados-grupo">Feriados nacionales</p>'; grupo = 1; } html += fila(f); } });
            $.each(datos, function (_, f) { if (f.Grupo !== "Feriado nacional") { if (grupo !== 2) { html += '<p class="cal-feriados-grupo">Fechas escolares (opcionales)</p>'; grupo = 2; } html += fila(f); } });
            $lista.html(html);
            actualizarBoton();
        }

        function cargarAnio(a) {
            $("#cal-feriados-anio").text(a);
            $lista.html('<p class="cal-cargando">Cargando…</p>');
            $cargar.prop("disabled", true);
            $.getJSON(cfg.urlFeriados, { anio: a })
                .done(pintar)
                .fail(function () { $lista.html('<p class="cal-error">No se pudieron traer los feriados.</p>'); });
        }

        $("#cal-btn-feriados").on("click", function () {
            cargarAnio(anio);
            $("#cal-modal-feriados").modal("show");
        });
        $("#cal-feriados-prev").on("click", function () { anio--; cargarAnio(anio); });
        $("#cal-feriados-next").on("click", function () { anio++; cargarAnio(anio); });
        $("#cal-feriados-todos").on("click", function () {
            $lista.find("input[type=checkbox]:not(:disabled)").each(function () {
                $(this).prop("checked", $(this).data("sugerido") === 1);
            });
            actualizarBoton();
        });
        $lista.on("change", "input[type=checkbox]", actualizarBoton);

        $cargar.on("click", function () {
            var publicar = $("#cal-feriados-publicar").is(":checked");
            var items = $lista.find("input[type=checkbox]:checked:not(:disabled)").map(function () {
                return { Clave: this.value, Publicar: publicar && $(this).data("verificar") !== 1 };
            }).get();
            if (items.length === 0) return;

            $cargar.prop("disabled", true).text("Cargando…");
            postJson(cfg.urlCargarFeriados, { Anio: anio, Items: items })
                .done(function (r) {
                    if (!r || !r.success) {
                        avisar("error", (r && r.message) || "No se pudieron cargar los feriados.");
                        // Si alguno sí se cargó (falla a medias), se recarga para que se vea lo que quedó.
                        if (r && r.creados > 0) window.setTimeout(function () { window.location.reload(); }, 2500);
                        else actualizarBoton();
                        return;
                    }
                    avisar("success", r.creados + (r.creados === 1 ? " día agregado" : " días agregados") +
                        (r.yaEstaban > 0 ? " (" + r.yaEstaban + " ya estaban)" : "") + ".");
                    window.setTimeout(function () { window.location.reload(); }, 700);
                })
                .fail(function () {
                    avisar("error", "No se pudieron cargar los feriados.");
                    actualizarBoton();
                });
        });
    };

    /* ───────────────────────── Editor de un día ───────────────────────── */
    obj.initDia = function (cfg) {
        if (!cfg.puedeEditar) return;

        var $error = $("#eve-error");
        var anioSecciones = cfg.anioSecciones;
        var buscando = null, temporizador = null;

        function mostrarError(texto, $campo) {
            $error.text(texto).prop("hidden", false);
            if ($campo && $campo.length) $campo.attr({ "aria-invalid": "true", "aria-describedby": "eve-error" }).trigger("focus");
        }

        function limpiarError() {
            if ($error.prop("hidden")) return;
            $error.text("").prop("hidden", true);
            $("#eve-form [aria-invalid]").removeAttr("aria-invalid aria-describedby");
        }

        /* En cuanto el usuario escribe o cambia cualquier campo, el aviso anterior ya no aplica */
        $("#eve-form").on("input change", limpiarError);

        /* Feriado o cierre: aviso del pase de lista, siempre de día completo y solo para todo el
           colegio o secciones (roles y personas se ocultan y no se envían). */
        function tipoCierra() {
            return String($("#eve-tipo").find(":selected").data("cierra")) === "1";
        }

        $("#eve-tipo").on("change", function () {
            var cierra = tipoCierra(), $todo = $("#eve-todo-dia");
            $("#eve-nota-cierre, #eve-nota-dirigido").prop("hidden", !cierra);
            $("#eve-campo-roles, #eve-campo-personas").prop("hidden", cierra);
            if (cierra && !$todo.is(":checked")) $todo.prop("checked", true).trigger("change");
            $todo.prop("disabled", cierra);
        });

        /* Todo el día ↔ con hora (08:00 a 15:00 por defecto, como Skuul) */
        $("#eve-todo-dia").on("change", function () {
            var todo = this.checked;
            $.each(["#eve-inicio", "#eve-fin"], function (i, sel) {
                var $i = $(sel), fecha = String($i.val() || "").substr(0, 10);
                $i.attr("type", todo ? "date" : "datetime-local");
                $i.val(todo ? fecha : (fecha ? fecha + "T" + (i === 0 ? "08:00" : "15:00") : ""));
            });
        });

        /* Si el fin queda antes del inicio, lo acomoda; si cambia el año, recarga las secciones */
        $("#eve-inicio").on("change", function () {
            var ini = $(this).val(), $fin = $("#eve-fin");
            if (ini && (!$fin.val() || $fin.val() < ini)) {
                $fin.val($("#eve-todo-dia").is(":checked") ? ini.substr(0, 10) : ini.substr(0, 10) + "T" + ($fin.val().substr(11, 5) || "15:00"));
            }
            var anio = parseInt(String(ini).substr(0, 4), 10);
            if (anio && anio !== anioSecciones) recargarSecciones(ini.substr(0, 10));
        });

        function recargarSecciones(fecha) {
            var marcadas = {};
            $("input[name=eve-sec]:checked").each(function () { marcadas[this.value] = true; });
            $("#eve-secciones").html('<p class="cal-cargando">Cargando secciones…</p>');
            $.getJSON(cfg.urlSecciones, { fecha: fecha, eveId: cfg.id || null }).done(function (lista) {
                lista = lista || [];
                if (lista.length === 0) {
                    $("#eve-secciones").html('<p class="cal-cargando">No hay secciones en este año lectivo.</p>');
                    anioSecciones = parseInt(fecha.substr(0, 4), 10);
                    $("#eve-anio-secciones").text(anioSecciones);
                    return;
                }
                var anios = {}, maximo = 0;
                $.each(lista, function (_, s) { anios[s.Ani_Anio] = true; if (s.Ani_Anio > maximo) maximo = s.Ani_Anio; });
                anioSecciones = maximo;
                $("#eve-anio-secciones").text(anioSecciones);

                var html = "", nivel = null;
                $.each(lista, function (_, s) {
                    var n = s.Niv_Descripcion || "Sin nivel";
                    if (n !== nivel) {
                        if (nivel !== null) html += "</div></div>";
                        html += '<div class="cal-grupo-nivel"><div class="cal-grupo-nivel-cabeza"><span>' + esc(n) +
                            '</span><button type="button" class="cal-btn--link eve-todo-nivel">Todo el nivel</button></div>' +
                            '<div class="cal-chips" role="group" aria-label="' + esc(n) + '">';
                        nivel = n;
                    }
                    html += '<label class="cal-opcion"><input type="checkbox" name="eve-sec" value="' + s.Sea_Id + '"' +
                        (marcadas[s.Sea_Id] ? " checked" : "") + "> " + esc(s.Sea_Nombre) +
                        (s.Ani_Anio !== anioSecciones ? " (" + s.Ani_Anio + ")" : "") + "</label>";
                });
                html += "</div></div>";
                $("#eve-secciones").html(html);
            });
        }

        /* "Todo el nivel": marca todas sus secciones (o las desmarca si ya estaban todas) */
        $("#eve-secciones").on("click", ".eve-todo-nivel", function () {
            var $cajas = $(this).closest(".cal-grupo-nivel").find("input[name=eve-sec]");
            var todas = $cajas.length === $cajas.filter(":checked").length;
            $cajas.prop("checked", !todas);
            limpiarError();
        });

        /* Nombrar a una persona */
        function yaNombrada(tipo, id) {
            return $('#eve-personas .cal-persona[data-tipo="' + tipo + '"][data-id="' + id + '"]').length > 0;
        }

        function pintarResultados(lista) {
            var $r = $("#eve-resultados");
            lista = $.grep(lista || [], function (p) { return !yaNombrada(p.Tipo, p.Id); });
            if (lista.length === 0) {
                $r.html('<li><button type="button" disabled><small>Nadie en el colegio coincide</small></button></li>').prop("hidden", false);
                return;
            }
            $r.html($.map(lista, function (p) {
                return '<li><button type="button" class="eve-agregar-persona" data-tipo="' + esc(p.Tipo) + '" data-id="' + p.Id +
                    '" data-nombre="' + esc(p.Nombre) + '" data-detalle="' + esc(p.Detalle) + '">' +
                    '<i class="bi bi-plus-lg" aria-hidden="true"></i> ' + esc(p.Nombre) +
                    ' <small>· ' + esc(p.Detalle) + (p.Tipo === "alumno" ? " (alumno)" : "") + "</small></button></li>";
            }).join("")).prop("hidden", false);
        }

        $("#eve-persona").on("input", function () {
            var texto = $.trim($(this).val());
            window.clearTimeout(temporizador);
            if (texto.length < 2) { $("#eve-resultados").prop("hidden", true).empty(); return; }
            temporizador = window.setTimeout(function () {
                if (buscando) buscando.abort();
                buscando = $.getJSON(cfg.urlPersonas, { texto: texto }).done(pintarResultados);
            }, 300);
        }).on("keydown", function (ev) {
            if (ev.key === "Escape") { $(this).val(""); $("#eve-resultados").prop("hidden", true).empty(); }
            if (ev.key === "Enter") ev.preventDefault();
        });

        $("#eve-resultados").on("click", ".eve-agregar-persona", function () {
            var d = $(this).data();
            if (!yaNombrada(d.tipo, d.id)) {
                $("#eve-personas").append(
                    '<span class="cal-persona" data-tipo="' + esc(d.tipo) + '" data-id="' + d.id + '">' + esc(d.nombre) +
                    " <small>" + esc(d.detalle) + "</small>" +
                    '<button type="button" class="eve-quitar-persona" aria-label="Quitar a ' + esc(d.nombre) + '"><i class="bi bi-x"></i></button></span>');
            }
            $("#eve-persona").val("").trigger("focus");
            $("#eve-resultados").prop("hidden", true).empty();
            limpiarError();
        });

        $("#eve-personas").on("click", ".eve-quitar-persona", function () {
            $(this).closest(".cal-persona").remove();
            limpiarError();
        });

        /* Guardar */
        function ids(selector) {
            return $(selector).map(function () { return parseInt(this.value || $(this).data("id"), 10); }).get();
        }

        function datos() {
            var cierra = tipoCierra();  // feriado o cierre: día completo, sin roles ni personas
            return {
                Eve_Id: cfg.id || 0,
                Eve_Titulo: $.trim($("#eve-titulo").val()),
                Eve_Tipo: $("#eve-tipo").val(),
                Eve_Lugar: $.trim($("#eve-lugar").val()),
                Eve_Descripcion: $.trim($("#eve-descripcion").val()),
                Eve_TodoElDia: cierra || $("#eve-todo-dia").is(":checked"),
                Eve_Inicio: $("#eve-inicio").val(),
                Eve_Fin: $("#eve-fin").val(),
                Secciones: ids("input[name=eve-sec]:checked"),
                Roles: cierra ? [] : ids("input[name=eve-rol]:checked"),
                Empleados: cierra ? [] : $('#eve-personas .cal-persona[data-tipo="empleado"]').map(function () { return +$(this).data("id"); }).get(),
                Alumnos: cierra ? [] : $('#eve-personas .cal-persona[data-tipo="alumno"]').map(function () { return +$(this).data("id"); }).get()
            };
        }

        function validar(d) {
            if (!d.Eve_Titulo) return ["Escriba un título.", "#eve-titulo"];
            if (!d.Eve_Inicio) return ["Indique cuándo empieza.", "#eve-inicio"];
            if (!d.Eve_Fin) return ["Indique cuándo termina.", "#eve-fin"];
            var ini = d.Eve_TodoElDia ? d.Eve_Inicio.substr(0, 10) : d.Eve_Inicio;
            var fin = d.Eve_TodoElDia ? d.Eve_Fin.substr(0, 10) : d.Eve_Fin;
            if (fin < ini) return ["Un día del calendario no puede terminar antes de empezar.", "#eve-fin"];
            return null;
        }

        $("#eve-form").on("submit", function (ev) {
            ev.preventDefault();
            limpiarError();
            var d = datos(), problema = validar(d);
            if (problema) { mostrarError(problema[0], $(problema[1])); return; }

            var $btn = $("#eve-guardar").prop("disabled", true);
            postJson(cfg.urlGuardar, d)
                .done(function (r) {
                    if (!r || !r.success) {
                        mostrarError((r && r.message) || "No se pudo guardar.");
                        $btn.prop("disabled", false);
                        return;
                    }
                    if (r.redirect) { window.location.href = r.redirect; return; }
                    window.location.href = window.location.pathname + "?aviso=guardado";
                })
                .fail(function () {
                    mostrarError("No se pudo guardar. Revise la conexión e intente de nuevo.");
                    $btn.prop("disabled", false);
                });
        });

        /* Publicar / volver a borrador */
        function cambiarPublicacion(publicar) {
            post(cfg.urlPublicar, { id: cfg.id, publicar: publicar })
                .done(function (r) {
                    if (!r || !r.success) { avisar("error", (r && r.message) || "No se pudo cambiar la publicación."); return; }
                    window.location.href = window.location.pathname + "?aviso=" + (publicar ? "publicado" : "despublicado");
                })
                .fail(function () { avisar("error", "No se pudo cambiar la publicación."); });
        }

        $("#eve-publicar").on("click", function () { cambiarPublicacion(true); });
        $("#eve-despublicar").on("click", function () { cambiarPublicacion(false); });

        /* Quitar del calendario (con confirmación) */
        $("#eve-quitar").on("click", function () { $("#eve-modal-quitar").modal("show"); });
        $("#eve-confirmar-quitar").on("click", function () {
            var $b = $(this).prop("disabled", true);
            post(cfg.urlQuitar, { id: cfg.id })
                .done(function (r) {
                    if (!r || !r.success) { avisar("error", (r && r.message) || "No se pudo quitar."); $b.prop("disabled", false); return; }
                    window.location.href = r.redirect;
                })
                .fail(function () { avisar("error", "No se pudo quitar."); $b.prop("disabled", false); });
        });
    };

    return obj;
})();
