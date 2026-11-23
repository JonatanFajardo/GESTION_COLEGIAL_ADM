/* Temario y avance — biblioteca (indicadores, filtros en cascada Nivel → Grado → Curso → Materia,
   "Nuevo temario", "Copiar temario"), resumen de Dirección (semáforo por sección, materias sin temario)
   y el temario (temas por semana, matriz de avance temas × secciones, imprimir). Todo texto que
   escribió una persona se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    function iso(v) { var m = /^(\d{4}-\d{2}-\d{2})/.exec(v || ""); return m ? m[1] : ""; }
    function fecha(v, conAnio) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + (conAnio === false ? "" : " " + m[1]) : "";
    }
    function aFecha(v) { var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || ""); return m ? new Date(+m[1], +m[2] - 1, +m[3]) : null; }
    function aIso(d) { return d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2); }
    function sumarDias(v, n) { var d = aFecha(v); if (!d) return ""; d.setDate(d.getDate() + n); return aIso(d); }
    function num(v) { var n = parseInt(v, 10); return isNaN(n) ? null : n; }

    /* Inicio de clases sugerido: primer lunes de febrero (I semestre) o de julio (II semestre). */
    function lunesSugerido(anio, sem) {
        anio = num(anio); if (!anio) return "";
        var d = new Date(anio, num(sem) === 2 ? 6 : 1, 1);
        while (d.getDay() !== 1) d.setDate(d.getDate() + 1);
        return aIso(d);
    }

    /* Lunes a viernes de la semana n contada desde el inicio de clases: "16 – 20 feb". */
    function rangoSemana(inicio, n) {
        var a = sumarDias(inicio, (n - 1) * 7), b = sumarDias(a, 4);
        if (!a) return "";
        return a.slice(5, 7) === b.slice(5, 7) ? (+a.slice(8)) + " – " + fecha(b, false) : fecha(a, false) + " – " + fecha(b, false);
    }

    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }
    function enviar(url, datos) {
        return $.ajax({
            url: url, type: "POST", contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos), headers: { RequestVerificationToken: token() }, dataType: "json"
        });
    }

    function llenar($select, items, vacio) {
        $select.empty();
        if (vacio !== null) $select.append($("<option>", { value: "", text: vacio }));
        (items || []).forEach(function (it) { $select.append($("<option>", { value: it.Value, text: it.Text })); });
    }
    function sinPadre($select, texto) { llenar($select, [], texto); $select.prop("disabled", true); }
    function opciones(url, params) {
        return $.getJSON(url, params).then(function (r) { return (r && r.data) || []; }, function () { return []; });
    }

    /* Errores: <p data-error="campo"> junto al control con ese id, o el aviso general del formulario. */
    function limpiarErrores($form, $alerta) {
        $form.find(".is-invalid").removeClass("is-invalid").removeAttr("aria-invalid");
        $form.find("[data-error]").text("");
        if ($alerta) $alerta.attr("hidden", true).text("");
    }
    function mostrarError($form, $alerta, r, general) {
        var campo = r && r.field, mensaje = (r && r.message) || general || "No se pudo guardar. Intente de nuevo.";
        if (campo && $form.find("#" + campo).length) {
            $form.find("#" + campo).addClass("is-invalid").attr("aria-invalid", "true").trigger("focus");
            $form.find('[data-error="' + campo + '"]').text(mensaje);
        } else if ($alerta) {
            $alerta.text(mensaje).removeAttr("hidden");
        }
    }
    function ocupado($form, si) { $form.find("[type=submit]").prop("disabled", si); }
    function fallaRed($form, $alerta) { mostrarError($form, $alerta, null, "No se pudo conectar con el servidor. Intente de nuevo."); ocupado($form, false); }

    var SEMAFORO = {
        al_dia: { texto: "Al día", clase: "badge-success" },
        leve: { texto: "Leve atraso", clase: "badge-warning" },
        atrasada: { texto: "Atrasada", clase: "badge-danger" },
        sin_inicio: { texto: "Sin iniciar", clase: "badge-secondary" },
        sin_temas: { texto: "Sin temas", clase: "badge-secondary" }
    };
    function semaforo(s) { var x = SEMAFORO[s] || { texto: "—", clase: "badge-secondary" }; return '<span class="badge ' + x.clase + '">' + x.texto + "</span>"; }
    function barra(pct) {
        pct = Math.max(0, Math.min(100, pct || 0));
        return '<div class="d-flex align-items-center"><strong class="mr-2 text-nowrap">' + pct + ' %</strong><div class="tm-barra flex-grow-1" role="img" aria-label="' + pct +
            ' por ciento"><span style="width:' + pct + '%"></span></div></div>';
    }
    function docentesCortos(texto) {
        var lista = String(texto || "").split(", ").filter(Boolean);
        if (!lista.length) return "Sin docente en el horario";
        return lista.slice(0, 3).join(", ") + (lista.length > 3 ? " y " + (lista.length - 3) + " más" : "");
    }
    var n = function (v) { return (v || 0).toLocaleString("es-HN"); };
    /* Tabulator alinea la celda con flex: todo el contenido va en un solo bloque para que no se parta en columnas. */
    function caja(html) { return '<div class="tm-caja">' + html + "</div>"; }

    /* ═══════════════════════ Copiar temario (biblioteca y ficha) ═══════════════════════ */
    var $formCopiar = $("#tm-form-copiar");
    var abrirCopiar = null;
    if ($formCopiar.length) {
        var $copiaError = $("#tm-copiar-error");
        var sugerirCopia = function () { $("#tm-copia-inicio").val(lunesSugerido($("#tm-copia-anio").val(), $("#tm-copia-semestre").val())); };
        $("#tm-copia-anio, #tm-copia-semestre").on("change", sugerirCopia);
        abrirCopiar = function (origenId) {
            limpiarErrores($formCopiar, $copiaError);
            if (origenId && $("#tm-origen").is("select")) $("#tm-origen").val(String(origenId));
            sugerirCopia();
            $("#tm-modal-copiar").modal("show");
        };
        $("#tm-btn-copiar").on("click", function () { abrirCopiar(null); });
        $formCopiar.on("submit", function (e) {
            e.preventDefault();
            limpiarErrores($formCopiar, $copiaError);
            if (!num($("#tm-origen").val())) { mostrarError($formCopiar, $copiaError, { field: "tm-origen", message: "Elija el temario que quiere copiar." }); return; }
            ocupado($formCopiar, true);
            enviar($formCopiar.data("url"), {
                Tem_IdOrigen: num($("#tm-origen").val()), Tem_Anio: num($("#tm-copia-anio").val()), Sem_Id: num($("#tm-copia-semestre").val()),
                Tem_FechaInicio: $("#tm-copia-inicio").val() || null, Tem_Nombre: $("#tm-copia-nombre").val()
            }).done(function (r) {
                if (r && r.success) { location.href = r.redirect; return; }
                mostrarError($formCopiar, $copiaError, r); ocupado($formCopiar, false);
            }).fail(function () { fallaRed($formCopiar, $copiaError); });
        });
    }

    /* ═══════════════════════ Biblioteca ═══════════════════════ */
    var $tabla = $("#tabla-temarios");
    if ($tabla.length) {
        var urlTemario = String($tabla.data("url-temario")), urlOpciones = String($tabla.data("url-opciones"));
        var edita = String($tabla.data("edita")) === "1";
        var $anio = $("#tm-f-anio"), $sem = $("#tm-f-semestre"), $nivel = $("#tm-f-nivel"), $grado = $("#tm-f-grado"),
            $curso = $("#tm-f-curso"), $materia = $("#tm-f-materia"), $atrasados = $("#tm-solo-atrasados");
        var t;

        // Cascada de la biblioteca: opciones de todos los años (y de los temarios que ya ve).
        $nivel.on("change", function () {
            sinPadre($grado, "Elija un nivel primero"); sinPadre($curso, "Elija un grado"); sinPadre($materia, "Elija un grado");
            var v = num($nivel.val());
            if (v) opciones(urlOpciones, { tipo: "grado", nivId: v }).then(function (it) { llenar($grado, it, "Todos"); $grado.prop("disabled", false); });
            refiltrar();
        });
        $grado.on("change", function () {
            sinPadre($curso, "Elija un grado"); sinPadre($materia, "Elija un grado");
            var v = num($grado.val());
            if (v) {
                opciones(urlOpciones, { tipo: "curso", cunId: v }).then(function (it) { llenar($curso, it, "Todos"); $curso.prop("disabled", false); });
                opciones(urlOpciones, { tipo: "materia", cunId: v }).then(function (it) { llenar($materia, it, "Todas"); $materia.prop("disabled", false); });
            }
            refiltrar();
        });
        $curso.on("change", function () {
            var g = num($grado.val());
            sinPadre($materia, "Elija un grado");
            if (g) opciones(urlOpciones, { tipo: "materia", cunId: g, curId: num($curso.val()) || undefined })
                .then(function (it) { llenar($materia, it, "Todas"); $materia.prop("disabled", false); });
            refiltrar();
        });
        $anio.add($sem).add($materia).add($atrasados).on("change", refiltrar);

        function filtroExtra(d) {
            var a = num($anio.val()), s = num($sem.val()), nv = num($nivel.val()), g = num($grado.val()), c = num($curso.val()), m = num($materia.val());
            if (a && d.Tem_Anio !== a) return false;
            if (s && d.Sem_Id !== s) return false;
            if (nv && d.Niv_Id !== nv) return false;
            if (g && d.Cun_Id !== g) return false;
            if (c && d.Cur_Id !== null && d.Cur_Id !== c) return false;
            if (m && d.Mat_Id !== m) return false;
            if ($atrasados.is(":checked") && !(d.SeccionesAtrasadas > 0)) return false;
            return true;
        }

        function indicadores() {
            if (!t) return;
            var filas = t.tabla.getData("active");
            $("#tm-total").text(filas.length);
            var temas = 0, secc = 0, atras = 0, sumaPct = 0, conSecc = 0;
            filas.forEach(function (d) {
                temas += d.Temas; secc += d.Secciones; atras += d.SeccionesAtrasadas;
                if (d.Secciones > 0) { sumaPct += d.AvancePct * d.Secciones; conSecc += d.Secciones; }
            });
            $("#kpi-temarios").text(n(filas.length));
            $("#kpi-temarios-texto").text(n(temas) + " temas en total");
            $("#kpi-avance").text(conSecc ? Math.round(sumaPct / conSecc) + " %" : "—");
            $("#kpi-atrasadas").text(n(atras));
            $("#kpi-secciones").text(n(secc));
            var hay = $anio.val() !== String($tabla.data("anio-actual")) || $sem.val() || $nivel.val() || $("#tm-buscar").val() || $atrasados.is(":checked");
            $("#tm-limpiar").prop("hidden", !hay);
        }
        function refiltrar() { if (t) { t.filtrar(); indicadores(); } }

        $("#tm-limpiar").on("click", function () {
            $anio.val(String($tabla.data("anio-actual"))); $sem.val(""); $nivel.val("").trigger("change");
            $("#tm-buscar").val(""); $atrasados.prop("checked", false); refiltrar();
        });
        $("#tm-buscar").on("input", function () { setTimeout(indicadores, 0); });

        t = GcTabla.crear({
            elemento: "#tabla-temarios", nombre: "temarios", url: $tabla.data("url"),
            archivo: "temarios", tituloPdf: "Temarios", buscador: "#tm-buscar",
            buscarEn: ["Materia", "Grado", "Curso", "Tem_Nombre", "Semestre"],
            vacio: "No hay temarios con estos filtros.",
            filtroExtra: filtroExtra,
            alCargar: function (filas) {
                var $origen = $("#tm-origen");
                if ($origen.is("select")) {
                    var con = filas.filter(function (d) { return d.Temas > 0; });
                    llenar($origen, con.map(function (d) {
                        return { Value: d.Tem_Id, Text: d.Materia + " · " + d.Grado + (d.Curso ? " (" + d.Curso + ")" : "") + " · " + d.Semestre + " " + d.Tem_Anio + " — " + d.Temas + " temas" };
                    }), con.length ? "Elija un temario" : "Todavía no hay temarios con temas");
                }
                setTimeout(function () { t.filtrar(); indicadores(); }, 0);
            },
            columnas: [
                { title: "Materia", field: "Materia", minWidth: 170, formatter: function (c) {
                    var d = c.getData();
                    return caja('<a class="tm-enlace" href="' + esc(urlTemario + "/" + d.Tem_Id) + '">' + esc(d.Materia) + "</a>" +
                        '<span class="tm-sub">' + esc(d.Tem_Nombre) + "</span>");
                } },
                { title: "Grado", field: "Grado", minWidth: 150, formatter: function (c) {
                    var d = c.getData();
                    return caja(esc(d.Grado + (d.Curso ? " · " + d.Curso : "")) + '<span class="tm-sub">' + esc(d.Semestre + " " + d.Tem_Anio) + (d.SemanaActual ? " · semana " + d.SemanaActual : "") + "</span>");
                } },
                { title: "Temas", field: "Temas", hozAlign: "center", width: 90, formatter: function (c) {
                    var d = c.getData(); return caja("<strong>" + d.Temas + '</strong><span class="tm-sub">' + d.Semanas + (d.Semanas === 1 ? " semana" : " semanas") + "</span>");
                } },
                { title: "Avance", field: "AvancePct", minWidth: 120, formatter: function (c) { return caja(barra(c.getValue())); } },
                { title: "Secciones", field: "Secciones", minWidth: 110, formatter: function (c) {
                    var d = c.getData();
                    if (!d.Secciones) return caja('<span class="text-muted">Ninguna en el horario</span>');
                    var b = d.SeccionesAtrasadas > 0 ? '<span class="badge badge-warning">' + d.SeccionesAtrasadas + (d.SeccionesAtrasadas === 1 ? " atrasada" : " atrasadas") + "</span>"
                        : (d.SemanaActual && d.Temas ? '<span class="badge badge-success">Al día</span>' : "");
                    return caja('<a class="tm-enlace" href="' + esc(urlTemario + "/" + d.Tem_Id + "?tab=avance") + '" title="Ver el avance por sección">' + d.Secciones + "</a> " + b);
                } },
                { title: "Acciones", field: "Acciones", headerSort: false, download: false, minWidth: 140, formatter: function (c) {
                    var d = c.getData(), url = urlTemario + "/" + d.Tem_Id;
                    var h = '<a class="btn btn-sm btn-outline-primary mr-1" href="' + esc(url) + '">Abrir</a>';
                    if (edita && d.Temas > 0) h += '<button type="button" class="btn btn-sm btn-outline-secondary" data-copiar="' + d.Tem_Id + '" title="Copiar a otro semestre o año" aria-label="Copiar temario"><i class="bi bi-files"></i></button>';
                    return caja(h);
                } }
            ]
        });

        $tabla.on("click", "[data-copiar]", function () { if (abrirCopiar) abrirCopiar(num($(this).data("copiar"))); });

        var urlSin = String($tabla.data("url-sin-temario") || "");
        if (urlSin) {
            $.getJSON(urlSin, { anio: $tabla.data("anio-actual") }).done(function (r) {
                var filas = (r && r.data) || [], claves = {};
                filas.forEach(function (d) { claves[d.Mat_Id + "-" + d.Cun_Id] = 1; });
                $("#kpi-sin-temario").text(n(Object.keys(claves).length));
            });
        }
    }

    /* ═══════════════════════ Nuevo temario ═══════════════════════ */
    var $formNuevo = $("#tm-form-nuevo");
    if ($formNuevo.length) {
        var urlOpc = String($("#tabla-temarios").data("url-opciones"));
        var $nError = $("#tm-nuevo-error"), $nAnio = $("#tm-anio"), $nSem = $("#tm-semestre"), $nNivel = $("#tm-nivel"),
            $nGrado = $("#tm-grado"), $nCurso = $("#tm-curso"), $nMat = $("#tm-materia"), $nNombre = $("#tm-nombre"), $nInicio = $("#tm-inicio");
        var prefill = null;

        var sugerirNombre = function () {
            if ($nNombre.data("tocado")) return;
            var m = $nMat.find("option:selected"), g = $nGrado.find("option:selected"), s = $nSem.find("option:selected");
            if (!num($nMat.val()) || !num($nGrado.val())) { $nNombre.val(""); return; }
            $nNombre.val(m.text() + " · " + g.text() + " · " + s.text() + " " + $nAnio.val());
        };
        var sugerirInicio = function () { if (!$nInicio.data("tocado")) $nInicio.val(lunesSugerido($nAnio.val(), $nSem.val())); };
        $nNombre.on("input", function () { $nNombre.data("tocado", $nNombre.val().trim() !== ""); });
        $nInicio.on("input change", function (e) { if (e.originalEvent) $nInicio.data("tocado", true); });

        var cargarNiveles = function () {
            sinPadre($nGrado, "Elija un nivel primero"); sinPadre($nCurso, "Elija un grado"); sinPadre($nMat, "Elija un grado");
            return opciones(urlOpc, { tipo: "nivel", anio: $nAnio.val() }).then(function (it) {
                llenar($nNivel, it, it.length ? "Elija el nivel" : "No imparte materias en el horario de ese año");
                $nNivel.prop("disabled", !it.length);
                if (it.length === 1) $nNivel.val(String(it[0].Value)).trigger("change");
            });
        };
        $nAnio.on("change", function () { cargarNiveles(); sugerirInicio(); sugerirNombre(); });
        $nSem.on("change", function () { sugerirInicio(); sugerirNombre(); });
        $nNivel.on("change", function () {
            sinPadre($nGrado, "Elija un nivel primero"); sinPadre($nCurso, "Elija un grado"); sinPadre($nMat, "Elija un grado"); sugerirNombre();
            var v = num($nNivel.val());
            if (v) opciones(urlOpc, { tipo: "grado", nivId: v, anio: $nAnio.val() }).then(function (it) {
                llenar($nGrado, it, "Elija el grado"); $nGrado.prop("disabled", false);
                if (prefill && prefill.cun) { $nGrado.val(String(prefill.cun)).trigger("change"); }
            });
        });
        var cargarMaterias = function () {
            var g = num($nGrado.val());
            sinPadre($nMat, "Elija un grado"); sugerirNombre();
            if (!g) return;
            opciones(urlOpc, { tipo: "materia", cunId: g, curId: num($nCurso.val()) || undefined, anio: $nAnio.val() }).then(function (it) {
                llenar($nMat, it, it.length ? "Elija la materia" : "Sin materias en el horario"); $nMat.prop("disabled", !it.length);
                if (prefill && prefill.mat) { $nMat.val(String(prefill.mat)); prefill = null; sugerirNombre(); }
            });
        };
        $nGrado.on("change", function () {
            sinPadre($nCurso, "Elija un grado");
            var g = num($nGrado.val());
            if (g) opciones(urlOpc, { tipo: "curso", cunId: g, anio: $nAnio.val() }).then(function (it) { llenar($nCurso, it, "Todos los del grado"); $nCurso.prop("disabled", false); });
            cargarMaterias();
        });
        $nCurso.on("change", cargarMaterias);
        $nMat.on("change", sugerirNombre);

        var abrirNuevo = function () {
            limpiarErrores($formNuevo, $nError);
            $formNuevo[0].reset();
            $nNombre.data("tocado", false); $nInicio.data("tocado", false);
            if (prefill) { if (prefill.anio) $nAnio.val(String(prefill.anio)); if (prefill.sem) $nSem.val(String(prefill.sem)); }
            sugerirInicio();
            cargarNiveles().then(function () { if (prefill && prefill.niv) $nNivel.val(String(prefill.niv)).trigger("change"); });
            $("#tm-modal-nuevo").modal("show");
        };
        $("#tm-btn-nuevo").on("click", function () { prefill = null; abrirNuevo(); });

        // Llega desde "Materias sin temario" del resumen: ?nuevo=1&anio=&sem=&niv=&cun=&mat=
        var q = new URLSearchParams(location.search);
        if (q.get("nuevo") === "1") {
            prefill = { anio: num(q.get("anio")), sem: num(q.get("sem")), niv: num(q.get("niv")), cun: num(q.get("cun")), mat: num(q.get("mat")) };
            abrirNuevo();
        }

        $formNuevo.on("submit", function (e) {
            e.preventDefault();
            limpiarErrores($formNuevo, $nError);
            ocupado($formNuevo, true);
            enviar($formNuevo.data("url"), {
                Tem_Anio: num($nAnio.val()) || 0, Sem_Id: num($nSem.val()) || 0, Cun_Id: num($nGrado.val()) || 0,
                Cur_Id: num($nCurso.val()), Mat_Id: num($nMat.val()) || 0, Tem_Nombre: $nNombre.val(),
                Tem_Descripcion: $("#tm-descripcion").val(), Tem_FechaInicio: $nInicio.val() || null
            }).done(function (r) {
                if (r && r.success) { location.href = r.redirect; return; }
                mostrarError($formNuevo, $nError, r); ocupado($formNuevo, false);
            }).fail(function () { fallaRed($formNuevo, $nError); });
        });
    }

    /* ═══════════════════════ Resumen de Dirección ═══════════════════════ */
    var $res = $("#tabla-temario-resumen");
    if ($res.length) {
        var urlT = String($res.data("url-temario")), urlNuevo = String($res.data("url-nuevo") || ""), urlOp = String($res.data("url-opciones"));
        var $rAnio = $("#tm-r-anio"), $rSem = $("#tm-r-semestre"), $rNivel = $("#tm-r-nivel"), $rGrado = $("#tm-r-grado"), $rSemaforo = $("#tm-r-semaforo");
        var urlRes = function () { return $res.data("url") + "?anio=" + encodeURIComponent($rAnio.val() || "") + ($rSem.val() ? "&semId=" + encodeURIComponent($rSem.val()) : ""); };
        var r;

        function kpisResumen() {
            if (!r) return;
            var filas = r.tabla.getData("active"), c = { al_dia: 0, leve: 0, atrasada: 0 };
            filas.forEach(function (d) { if (c[d.Semaforo] !== undefined) c[d.Semaforo]++; });
            $("#tm-r-total").text(filas.length);
            $("#kpi-secciones").text(n(filas.length));
            var temarios = {}; filas.forEach(function (d) { temarios[d.Tem_Id] = 1; });
            $("#kpi-secciones-texto").text(n(Object.keys(temarios).length) + " temarios");
            $("#kpi-al-dia").text(n(c.al_dia));
            $("#kpi-atraso").text(n(c.leve + c.atrasada));
            $("#kpi-atraso-texto").text(n(c.atrasada) + " atrasadas (3 o más temas) · " + n(c.leve) + " con leve atraso");
        }
        function filtroResumen(d) {
            var nv = num($rNivel.val()), g = num($rGrado.val()), s = $rSemaforo.val();
            if (nv && d.Niv_Id !== nv) return false;
            if (g && d.Cun_Id !== g) return false;
            if (s === "con_atraso") return d.Semaforo === "leve" || d.Semaforo === "atrasada";
            if (s && d.Semaforo !== s) return false;
            return true;
        }
        function sinTemario() {
            var $c = $("#tm-sin-cuerpo");
            $.getJSON($res.data("url-sin-temario"), { anio: $rAnio.val(), semId: $rSem.val() || undefined }).done(function (resp) {
                var filas = (resp && resp.data) || [], claves = {};
                filas.forEach(function (d) { claves[d.Mat_Id + "-" + d.Cun_Id] = 1; });
                $("#kpi-sin-temario").text(n(Object.keys(claves).length));
                $("#tm-sin-total").text(filas.length);
                if (!filas.length) { $c.html('<tr><td colspan="5" class="text-muted">Todas las materias del horario tienen temario.</td></tr>'); return; }
                $c.html(filas.map(function (d) {
                    var link = urlNuevo ? '<td class="text-right"><a class="btn btn-sm btn-outline-primary" href="' + esc(urlNuevo + "?nuevo=1&anio=" + $rAnio.val() + "&sem=" + d.Sem_Id +
                        "&niv=" + (d.Niv_Id || "") + "&cun=" + d.Cun_Id + "&mat=" + d.Mat_Id) + '">Crear temario</a></td>' : "";
                    return "<tr><td>" + esc(d.Semestre) + "</td><td>" + esc(d.Materia) + "</td><td>" + esc(d.Grado) + '<span class="tm-sub">' + esc(d.Nivel) +
                        '</span></td><td class="text-right">' + d.Secciones + "</td>" + link + "</tr>";
                }).join(""));
            }).fail(function () { $c.html('<tr><td colspan="5" class="text-danger">No se pudo cargar la lista.</td></tr>'); });
        }

        $rNivel.on("change", function () {
            sinPadre($rGrado, "Elija un nivel primero");
            var v = num($rNivel.val());
            if (v) opciones(urlOp, { tipo: "grado", nivId: v }).then(function (it) { llenar($rGrado, it, "Todos los grados"); $rGrado.prop("disabled", false); });
            if (r) { r.filtrar(); kpisResumen(); }
        });
        $rGrado.add($rSemaforo).on("change", function () { if (r) { r.filtrar(); kpisResumen(); } });
        $rAnio.add($rSem).on("change", function () { if (r) r.tabla.setData(urlRes()); sinTemario(); });
        $("#tm-r-buscar").on("input", function () { setTimeout(kpisResumen, 0); });

        r = GcTabla.crear({
            elemento: "#tabla-temario-resumen", nombre: "temario-resumen", url: urlRes(),
            archivo: "avance-temarios", tituloPdf: "Avance del programa por sección", buscador: "#tm-r-buscar",
            buscarEn: ["Materia", "Docentes", "Grado", "SeccionTexto"],
            vacio: "No hay secciones con temario para este año y semestre.",
            filtroExtra: filtroResumen,
            alCargar: function () { setTimeout(function () { r.filtrar(); kpisResumen(); }, 0); },
            columnas: [
                { title: "Materia", field: "Materia", minWidth: 160, formatter: function (c) {
                    var d = c.getData();
                    return caja('<a class="tm-enlace" href="' + esc(urlT + "/" + d.Tem_Id + "?tab=avance") + '">' + esc(d.Materia) + "</a>");
                } },
                { title: "Sección", field: "SeccionTexto", minWidth: 130, mutator: function (v, d) { return d.Grado + " " + d.Seccion + " · " + d.Jornada; },
                  formatter: function (c) { var d = c.getData(); return caja(esc(d.Grado + " " + d.Seccion) + '<span class="tm-sub">' + esc(d.Jornada + (d.Curso ? " · " + d.Curso : "")) + "</span>"); } },
                { title: "Docentes", field: "Docentes", minWidth: 150, formatter: function (c) { return caja('<span class="small" title="' + esc(c.getValue()) + '">' + esc(docentesCortos(c.getValue())) + "</span>"); } },
                { title: "Avance", field: "Pct", minWidth: 150, formatter: function (c) {
                    var d = c.getData();
                    return caja(barra(d.Pct) + '<span class="tm-sub">' + d.Dados + " de " + d.Total + " temas" + (d.UltimaFecha ? " · última clase " + fecha(d.UltimaFecha, false) : "") + "</span>");
                } },
                { title: "Atraso", field: "SemaforoTexto", minWidth: 150, mutator: function (v, d) { return (SEMAFORO[d.Semaforo] || {}).texto || ""; },
                  formatter: function (c) {
                      var d = c.getData();
                      return caja(semaforo(d.Semaforo) + '<span class="tm-sub">' + (d.Atrasados > 0 ? '<strong class="text-danger">' + d.Atrasados + "</strong>" : "0") +
                          " de " + d.Esperados + " esperados</span>");
                  } }
            ]
        });
        sinTemario();
    }

    /* ═══════════════════════ El temario ═══════════════════════ */
    var $t = $("#tm-temario");
    if ($t.length) {
        var temId = num($t.data("id")), inicio = String($t.data("inicio")), hoy = String($t.data("hoy"));
        var editaT = String($t.data("edita")) === "1", supervisa = String($t.data("supervisa")) === "1";
        var datos = { temas: [], grupos: [], avance: [], temario: {} }, marcas = {};
        var clave = function (g) { return g.Cur_Id + "-" + g.Sec_Id + "-" + g.Mda_Id; };
        var $semanas = $("#tm-semanas");

        var cargar = function () {
            return $.getJSON($t.data("url-datos")).done(function (r) {
                if (!r || !r.ok) { $semanas.html('<div class="alert alert-light-warning">No se pudo leer el temario. Recargue la página.</div>'); return; }
                datos = r; marcas = {};
                (r.avance || []).forEach(function (a) { marcas[a.Tet_Id + "|" + clave(a)] = a; });
                $("#tm-n-temas").text(r.temas.length);
                $("#tm-n-secciones").text(r.grupos.length);
                $("#tm-dato-conteo").text(r.temas.length + " temas · " + r.grupos.length + " secciones");
                pintarSemanas(); pintarGrupos(); pintarMatriz();
            }).fail(function () { $semanas.html('<div class="alert alert-light-danger">No se pudo conectar con el servidor.</div>'); });
        };

        var semanaActual = function () { return datos.temario && datos.temario.SemanaActual; };

        function pintarSemanas() {
            var temas = datos.temas || [];
            if (!temas.length) {
                $semanas.html('<div class="alert alert-light-info mb-0">Este temario todavía no tiene temas.' +
                    (editaT ? " Agregue el primero o traiga los temas de otro temario de la misma materia." : "") + "</div>");
                return;
            }
            var actual = semanaActual(), grupos = (datos.grupos || []).length, porSemana = {};
            temas.forEach(function (x) { (porSemana[x.Tet_Semana] = porSemana[x.Tet_Semana] || []).push(x); });
            var html = Object.keys(porSemana).map(Number).sort(function (a, b) { return a - b; }).map(function (s) {
                var lista = porSemana[s], esActual = actual === s, pasada = actual && s < actual;
                var cab = '<div class="tm-semana-cab"><h5>Semana ' + s + '</h5><span class="tm-fechas">' + esc(rangoSemana(inicio, s)) + "</span>" +
                    (esActual ? '<span class="badge badge-info">Semana actual</span>' : "") +
                    (pasada ? '<span class="text-muted small"><i class="bi bi-clock-history mr-1" aria-hidden="true"></i>Ya debió darse</span>' : "") +
                    (editaT ? '<button type="button" class="btn btn-sm btn-link ml-auto p-0" data-tema-semana="' + s + '"><i class="bi bi-plus-lg mr-1" aria-hidden="true"></i>Tema en esta semana</button>' : "") +
                    "</div>";
                var items = lista.map(function (x, i) {
                    var det = "";
                    if (x.Tet_Objetivos) det += "<dt>Objetivos</dt><dd>" + esc(x.Tet_Objetivos) + "</dd>";
                    if (x.Tet_Contenido) det += "<dt>Contenido</dt><dd>" + esc(x.Tet_Contenido) + "</dd>";
                    if (x.Tet_Recursos) det += "<dt>Recursos</dt><dd>" + esc(x.Tet_Recursos) + "</dd>";
                    var acc = editaT ? '<div class="tm-tema-acciones">' +
                        '<button type="button" class="btn btn-sm btn-outline-secondary" data-mover="arriba" data-tet="' + x.Tet_Id + '" title="Subir" aria-label="Subir tema"' + (i === 0 ? " disabled" : "") + '><i class="bi bi-arrow-up"></i></button>' +
                        '<button type="button" class="btn btn-sm btn-outline-secondary" data-mover="abajo" data-tet="' + x.Tet_Id + '" title="Bajar" aria-label="Bajar tema"' + (i === lista.length - 1 ? " disabled" : "") + '><i class="bi bi-arrow-down"></i></button>' +
                        '<button type="button" class="btn btn-sm btn-outline-primary" data-editar-tema="' + x.Tet_Id + '" title="Editar" aria-label="Editar tema"><i class="bi bi-pencil"></i></button>' +
                        '<button type="button" class="btn btn-sm btn-outline-danger" data-quitar-tema="' + x.Tet_Id + '" title="Quitar" aria-label="Quitar tema"><i class="bi bi-trash"></i></button></div>' : "";
                    return '<div class="tm-tema"><span class="tm-tema-num">' + (i + 1) + '</span><div class="tm-tema-cuerpo"><div class="tm-tema-titulo">' + esc(x.Tet_Titulo) + "</div>" +
                        (grupos ? '<span class="tm-sub">Dado en ' + x.SeccionesDado + " de " + grupos + (grupos === 1 ? " sección" : " secciones") + "</span>" : "") +
                        (det ? '<dl class="tm-tema-detalle">' + det + "</dl>" : "") + "</div>" + acc + "</div>";
                }).join("");
                return '<section class="tm-semana' + (esActual ? " tm-actual" : "") + '" aria-label="Semana ' + s + '">' + cab + items + "</section>";
            }).join("");
            $semanas.html(html);
        }

        function nombreGrupo(g, corto) {
            var j = corto ? String(g.Jornada || "").slice(0, 4) + "." : g.Jornada;
            return (corto ? "" : "Sección ") + g.Seccion + " · " + j;
        }
        var variosCursos = function () { var c = {}; (datos.grupos || []).forEach(function (g) { c[g.Cur_Id] = 1; }); return Object.keys(c).length > 1; };

        function pintarGrupos() {
            var gs = datos.grupos || [], varios = variosCursos();
            if (!gs.length) { $("#tm-grupos").html('<div class="col-12"><div class="alert alert-light-warning">Ninguna sección lleva esta materia en el horario del año del temario.</div></div>'); return; }
            $("#tm-grupos").html(gs.map(function (g) {
                return '<div class="col-md-6 col-xl-4 mb-3"><div class="tm-grupo"><div class="d-flex justify-content-between align-items-start mb-1"><div><h6>' + esc(nombreGrupo(g)) + "</h6>" +
                    (varios ? '<span class="tm-sub">' + esc(g.Curso) + "</span>" : "") + "</div>" + semaforo(g.Semaforo) + "</div>" + barra(g.Pct) +
                    '<p class="small mb-1 mt-1">' + g.Dados + " de " + g.Total + " dados" + (g.Parciales ? " · " + g.Parciales + " en parte" : "") + (g.Omitidos ? " · " + g.Omitidos + (g.Omitidos === 1 ? " omitido" : " omitidos") : "") +
                    (g.Atrasados ? ' · <strong class="text-danger">' + g.Atrasados + (g.Atrasados === 1 ? " atrasado" : " atrasados") + "</strong>" : "") + "</p>" +
                    '<p class="tm-sub mb-0" title="' + esc(g.Docentes) + '"><i class="bi bi-person mr-1" aria-hidden="true"></i>' + esc(docentesCortos(g.Docentes)) + "</p>" +
                    (g.PuedeMarcar && !supervisa ? '<span class="badge outline-badge-primary mt-2">Usted da esta sección</span>' : "") + "</div></div>";
            }).join(""));
        }

        var ICONO = { dado: "bi-check-lg", parcial: "bi-circle-half", omitido: "bi-skip-forward" };
        var ESTADO = { dado: "Dado", parcial: "Dado en parte", omitido: "Omitido" };

        function pintarMatriz() {
            var gs = datos.grupos || [], temas = datos.temas || [], actual = semanaActual(), varios = variosCursos();
            var algunoMio = gs.some(function (g) { return g.PuedeMarcar; });
            $("#tm-matriz-ayuda").text(!gs.length || !temas.length ? "" :
                supervisa ? "Haga clic en una casilla para marcar si el tema se dio en esa sección." :
                algunoMio ? "Puede marcar las casillas de las secciones a las que usted da la materia; las demás son solo de consulta." :
                "Solo consulta: usted no da esta materia a ninguna de estas secciones.");
            if (!gs.length || !temas.length) { $("#tm-matriz").html('<p class="text-muted mb-0">' + (temas.length ? "No hay secciones para mostrar." : "Agregue temas para registrar el avance.") + "</p>"); return; }
            var cab = '<thead><tr><th class="tm-col-semana" scope="col">Sem.</th><th class="tm-col-tema" scope="col">Tema</th>' + gs.map(function (g) {
                return '<th scope="col" title="' + esc((varios ? g.Curso + " · " : "") + nombreGrupo(g) + " — " + g.Docentes) + '">' + esc(g.Seccion) + '<br><span class="font-weight-normal text-muted">' +
                    esc(String(g.Jornada || "").slice(0, 4) + ".") + (varios ? " " + esc(String(g.Curso || "").replace(/^BTP\s*/i, "").slice(0, 10)) : "") + "</span></th>";
            }).join("") + "</tr></thead>";
            var cuerpo = temas.map(function (x) {
                return '<tr><td class="tm-col-semana">' + x.Tet_Semana + '</td><th class="tm-col-tema font-weight-normal" scope="row">' + esc(x.Tet_Titulo) + "</th>" + gs.map(function (g) {
                    var m = marcas[x.Tet_Id + "|" + clave(g)], est = m ? m.Tav_Estado : "";
                    var atrasado = !m && actual && x.Tet_Semana < actual || (m && m.Tav_Estado === "parcial" && actual && x.Tet_Semana < actual);
                    var cls = "tm-celda" + (est ? " tm-" + est : "") + (atrasado && est !== "parcial" ? " tm-atrasado" : "");
                    var icono = est ? ICONO[est] : (atrasado ? "bi-exclamation" : "bi-circle");
                    var texto = est ? ESTADO[est] + (m.Tav_Fecha ? " el " + fecha(m.Tav_Fecha) : "") + (m.Tav_Nota ? " — " + m.Tav_Nota : "") : (atrasado ? "Atrasado" : "Sin marcar");
                    return '<td class="text-center p-1"><button type="button" class="' + cls + '" data-celda="1" data-tet="' + x.Tet_Id + '" data-clave="' + clave(g) + '"' +
                        (g.PuedeMarcar ? "" : " disabled") + ' title="' + esc(texto) + '" aria-label="' + esc(x.Tet_Titulo + ", " + nombreGrupo(g) + ": " + texto) + '"><i class="bi ' + icono + '" aria-hidden="true"></i></button></td>';
                }).join("") + "</tr>";
            }).join("");
            $("#tm-matriz").html('<table class="table table-bordered table-sm tm-matriz">' + cab + "<tbody>" + cuerpo + "</tbody></table>");
        }

        // ── Marcar avance ──
        var $fAv = $("#tm-form-avance"), $avError = $("#tm-av-error"), celda = null;
        var estadoAv = function () { return $fAv.find("input[name=tm-av-estado]:checked").val(); };
        $fAv.find("input[name=tm-av-estado]").on("change", function () { $("#tm-av-fecha-grupo").prop("hidden", estadoAv() === "omitido"); });
        $("#tm-matriz").on("click", "[data-celda]", function () {
            var tet = num($(this).data("tet")), k = String($(this).data("clave"));
            var tema = (datos.temas || []).filter(function (x) { return x.Tet_Id === tet; })[0];
            var g = (datos.grupos || []).filter(function (y) { return clave(y) === k; })[0];
            if (!tema || !g || !g.PuedeMarcar) return;
            celda = { tema: tema, grupo: g };
            var m = marcas[tet + "|" + k];
            limpiarErrores($fAv, $avError);
            $("#tm-av-tema").text("Semana " + tema.Tet_Semana + " · " + tema.Tet_Titulo);
            $("#tm-av-seccion").text((variosCursos() ? g.Curso + " · " : "") + nombreGrupo(g));
            $fAv.find("input[name=tm-av-estado][value=" + (m ? m.Tav_Estado : "dado") + "]").prop("checked", true).trigger("change");
            $("#tm-av-fecha").val(m && m.Tav_Fecha ? m.Tav_Fecha : hoy);
            $("#tm-av-nota").val(m ? m.Tav_Nota || "" : "");
            $("#tm-av-quien").text(m && m.Usuario ? "Última marca: " + m.Usuario : "");
            $("#tm-av-quitar").prop("hidden", !m);
            $("#tm-modal-avance").modal("show");
        });
        var datosCelda = function () { return { Tet_Id: celda.tema.Tet_Id, Cur_Id: celda.grupo.Cur_Id, Sec_Id: celda.grupo.Sec_Id, Mda_Id: celda.grupo.Mda_Id }; };
        $fAv.on("submit", function (e) {
            e.preventDefault();
            if (!celda) return;
            limpiarErrores($fAv, $avError); ocupado($fAv, true);
            var d = datosCelda(); d.Tav_Estado = estadoAv(); d.Tav_Fecha = d.Tav_Estado === "omitido" ? null : ($("#tm-av-fecha").val() || null); d.Tav_Nota = $("#tm-av-nota").val();
            enviar($t.data("url-marcar"), d).done(function (r) {
                ocupado($fAv, false);
                if (r && r.success) { $("#tm-modal-avance").modal("hide"); cargar(); return; }
                mostrarError($fAv, $avError, r);
            }).fail(function () { fallaRed($fAv, $avError); });
        });
        $("#tm-av-quitar").on("click", function () {
            if (!celda) return;
            enviar($t.data("url-quitar-marca"), datosCelda()).done(function (r) {
                if (r && r.success) { $("#tm-modal-avance").modal("hide"); cargar(); return; }
                mostrarError($fAv, $avError, r);
            }).fail(function () { fallaRed($fAv, $avError); });
        });

        // ── Confirmar ──
        var confirmar = function (texto, boton, accion) {
            $("#tm-confirmar-texto").text(texto);
            $("#tm-confirmar-si").text(boton).off("click").on("click", function () { $("#tm-modal-confirmar").modal("hide"); accion(); });
            $("#tm-modal-confirmar").modal("show");
        };
        var avisoError = function (r) {
            $semanas.prepend('<div class="alert alert-light-danger" role="alert">' +
                esc((r && r.message) || "No se pudo completar la acción.") + "</div>");
        };

        if (editaT) {
            // ── Tema ──
            var $fTema = $("#tm-form-tema"), $temaError = $("#tm-tema-error");
            var fechasSemana = function () { var s = num($("#tm-tema-semana").val()); $("#tm-tema-fechas").text(s >= 1 && s <= 30 ? rangoSemana(inicio, s) : " "); };
            $("#tm-tema-semana").on("input change", fechasSemana);
            var abrirTema = function (tema, semana) {
                limpiarErrores($fTema, $temaError);
                $fTema[0].reset();
                $("#tm-tema-titulo-modal").text(tema ? "Editar tema" : "Agregar tema");
                $("#tm-tema-id").val(tema ? tema.Tet_Id : 0);
                var ultima = (datos.temas || []).reduce(function (m, x) { return Math.max(m, x.Tet_Semana); }, 0);
                $("#tm-tema-semana").val(tema ? tema.Tet_Semana : (semana || ultima || semanaActual() || 1));
                if (tema) {
                    $("#tm-tema-titulo").val(tema.Tet_Titulo); $("#tm-tema-objetivos").val(tema.Tet_Objetivos || "");
                    $("#tm-tema-contenido").val(tema.Tet_Contenido || ""); $("#tm-tema-recursos").val(tema.Tet_Recursos || "");
                }
                fechasSemana();
                $("#tm-modal-tema").modal("show");
            };
            $("#tm-btn-tema").on("click", function () { abrirTema(null); });
            $semanas.on("click", "[data-tema-semana]", function () { abrirTema(null, num($(this).data("tema-semana"))); });
            $semanas.on("click", "[data-editar-tema]", function () {
                var id = num($(this).data("editar-tema"));
                abrirTema((datos.temas || []).filter(function (x) { return x.Tet_Id === id; })[0]);
            });
            $fTema.on("submit", function (e) {
                e.preventDefault();
                limpiarErrores($fTema, $temaError); ocupado($fTema, true);
                enviar($t.data("url-tema"), {
                    Tet_Id: num($("#tm-tema-id").val()) || 0, Tem_Id: temId, Tet_Semana: num($("#tm-tema-semana").val()) || 0,
                    Tet_Titulo: $("#tm-tema-titulo").val(), Tet_Objetivos: $("#tm-tema-objetivos").val(),
                    Tet_Contenido: $("#tm-tema-contenido").val(), Tet_Recursos: $("#tm-tema-recursos").val()
                }).done(function (r) {
                    ocupado($fTema, false);
                    if (r && r.success) { $("#tm-modal-tema").modal("hide"); cargar(); return; }
                    mostrarError($fTema, $temaError, r);
                }).fail(function () { fallaRed($fTema, $temaError); });
            });
            $semanas.on("click", "[data-mover]", function () {
                var $b = $(this).prop("disabled", true);
                enviar($t.data("url-mover"), { Tet_Id: num($b.data("tet")), Direccion: $b.data("mover") })
                    .done(function (r) { if (r && r.success) cargar(); else { avisoError(r); $b.prop("disabled", false); } })
                    .fail(function () { avisoError(null); $b.prop("disabled", false); });
            });
            $semanas.on("click", "[data-quitar-tema]", function () {
                var id = num($(this).data("quitar-tema")), tema = (datos.temas || []).filter(function (x) { return x.Tet_Id === id; })[0];
                if (!tema) return;
                confirmar("¿Quitar el tema «" + tema.Tet_Titulo + "» de la semana " + tema.Tet_Semana + "?" +
                    (tema.Marcas ? " Tiene avance marcado en " + tema.Marcas + (tema.Marcas === 1 ? " sección" : " secciones") + ", que deja de contarse." : ""), "Quitar tema", function () {
                    enviar($t.data("url-quitar-tema"), { Tet_Id: id }).done(function (r) { if (r && r.success) cargar(); else avisoError(r); }).fail(function () { avisoError(null); });
                });
            });

            // ── Datos del temario ──
            var $fEd = $("#tm-form-editar"), $edError = $("#tm-editar-error");
            $("#tm-btn-editar").on("click", function () { limpiarErrores($fEd, $edError); $("#tm-modal-editar").modal("show"); });
            $fEd.on("submit", function (e) {
                e.preventDefault();
                limpiarErrores($fEd, $edError); ocupado($fEd, true);
                enviar($t.data("url-editar"), { Tem_Id: temId, Tem_Nombre: $("#tm-nombre").val(), Tem_Descripcion: $("#tm-descripcion").val(), Tem_FechaInicio: $("#tm-inicio").val() || null })
                    .done(function (r) { if (r && r.success) { location.href = r.redirect; return; } mostrarError($fEd, $edError, r); ocupado($fEd, false); })
                    .fail(function () { fallaRed($fEd, $edError); });
            });

            // ── Quitar el temario ──
            $("#tm-btn-eliminar").on("click", function () {
                var marcasTotal = (datos.avance || []).length;
                confirmar("¿Quitar este temario? Deja de aparecer en la biblioteca" + (marcasTotal ? " y se pierde de vista el avance de " + marcasTotal + " marcas" : "") + ".", "Quitar temario", function () {
                    enviar($t.data("url-eliminar"), { Tem_Id: temId }).done(function (r) { if (r && r.success) location.href = r.redirect; else avisoError(r); }).fail(function () { avisoError(null); });
                });
            });

            // ── Traer temas de otro temario (misma materia) ──
            var $fTr = $("#tm-form-traer"), $trError = $("#tm-traer-error");
            $("#tm-btn-traer").on("click", function () {
                limpiarErrores($fTr, $trError);
                var $sel = $("#tm-traer-origen");
                llenar($sel, [], "Cargando…");
                $.getJSON($t.data("url-lista")).done(function (r) {
                    var mat = num($t.data("mat"));
                    var lista = ((r && r.data) || []).filter(function (d) { return d.Mat_Id === mat && d.Tem_Id !== temId && d.Temas > 0; });
                    llenar($sel, lista.map(function (d) { return { Value: d.Tem_Id, Text: d.Grado + " · " + d.Semestre + " " + d.Tem_Anio + " — " + d.Temas + " temas" }; }),
                        lista.length ? "Elija un temario" : "No hay otros temarios de esta materia con temas");
                });
                $("#tm-modal-traer").modal("show");
            });
            $fTr.on("submit", function (e) {
                e.preventDefault();
                limpiarErrores($fTr, $trError);
                var origen = num($("#tm-traer-origen").val());
                if (!origen) { $trError.text("Elija el temario del que quiere traer los temas.").removeAttr("hidden"); return; }
                ocupado($fTr, true);
                enviar($fTr.data("url"), { Tem_IdOrigen: origen, Tem_IdDestino: temId })
                    .done(function (r) { if (r && r.success) { location.href = r.redirect; return; } mostrarError($fTr, $trError, r); ocupado($fTr, false); })
                    .fail(function () { fallaRed($fTr, $trError); });
            });
        }

        $("#tm-ver-detalle").on("change", function () { $semanas.toggleClass("tm-oculto-detalle", !this.checked); });
        $("#tm-tabs a[data-toggle=tab]").on("shown.bs.tab", function () {
            var q = new URLSearchParams(location.search);
            if (this.id === "tm-tab-avance") q.set("tab", "avance"); else q.delete("tab");
            q.delete("aviso"); q.delete("n");
            history.replaceState(null, "", location.pathname + (q.toString() ? "?" + q.toString() : ""));
        });
        cargar();
    }

    $("[data-imprimir]").on("click", function () { window.print(); });
}(jQuery));
