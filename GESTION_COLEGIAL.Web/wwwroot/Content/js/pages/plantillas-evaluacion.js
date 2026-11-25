/* Plantillas de evaluación (notas ponderadas). Pestaña "Plantillas": qué plantilla se aplica a cada nivel
   (con sus excepciones por materia), una tarjeta por plantilla con sus pesos y su uso, y la ventana para
   crear, editar o copiar (tipos con peso que suman 100, cómo se juntan las actividades de cada tipo).
   Pestaña "Uso en los cuadernos": por materia y sección del parcial, si ya califica con actividades, cuánto
   se ha calificado y si la nota del parcial está al día (GcTabla, exportable); las notas faltantes se ven
   en Cierre de parciales. Todo texto que escribió una persona se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    var $raiz = $("#np-raiz");
    if (!$raiz.length) return;

    var URL = {
        lista: $raiz.data("url-lista"),
        opciones: $raiz.data("url-opciones"),
        guardar: $raiz.data("url-guardar"),
        quitar: $raiz.data("url-quitar"),
        avance: $raiz.data("url-avance"),
        cuaderno: $raiz.data("url-cuaderno")
    };
    var ANIO = parseInt($raiz.data("anio"), 10);
    var COMBINACION = { suma: "Suma de puntos", promedio: "Promedio de actividades", mejor: "La mejor actividad" };
    var SUGERIDOS = [
        { n: "Examen", p: 30, c: "suma" }, { n: "Tareas", p: 30, c: "suma" },
        { n: "Trabajos", p: 20, c: "suma" }, { n: "Participación", p: 20, c: "promedio" }
    ];
    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];

    var plantillas = [];
    var niveles = [];
    var editando = null;     // plantilla que se edita (null = nueva o copia)
    var quitando = null;
    var tabla = null;

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
    function datos(r) { return Array.isArray(r) ? r : ((r && r.data) || []); }
    function fmt(n) { return (Number(n) || 0).toLocaleString("es-HN", { maximumFractionDigits: 2 }); }
    function fechaHora(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1] + ", " + m[4] + ":" + m[5] : "";
    }
    function tiposDe(p) {
        try { return JSON.parse(p.TiposJson || "[]") || []; } catch (e) { return []; }
    }
    function alerta(tipo, html) { return '<div class="alert alert-light-' + tipo + ' mb-3" role="alert">' + html + '</div>'; }

    // ───────────────────────────── Lista ─────────────────────────────
    function cargar() {
        $.getJSON(URL.lista, { anio: ANIO })
            .done(function (r) {
                plantillas = datos(r);
                plantillas.forEach(function (p) { p.tipos = tiposDe(p); });
                pintarCobertura();
                pintarTarjetas();
            })
            .fail(function () {
                $("#np-lista").html('<div class="col-12">' + alerta("danger", "No se pudieron cargar las plantillas. Recargue la página.") + '</div>');
            });
    }

    function nombrePlantilla(p) {
        return p ? '<strong>' + esc(p.Nombre) + '</strong> <span class="text-muted small">(' +
            p.tipos.map(function (t) { return esc(t.Nombre) + " " + fmt(t.Peso) + " %"; }).join(" · ") + ')</span>' : "";
    }

    function pintarCobertura() {
        var porDefecto = plantillas.filter(function (p) { return p.PorDefecto; })[0] || null;
        if (!niveles.length) { $("#np-cobertura").html('<tr><td colspan="3" class="text-muted">Cargando…</td></tr>'); return; }
        var html = niveles.map(function (n) {
            var propia = plantillas.filter(function (p) { return p.Niv_Id === n.Id && !p.Mat_Id; })[0] || null;
            var aplica = propia || porDefecto;
            var excepciones = plantillas.filter(function (p) { return p.Mat_Id && (p.Niv_Id === n.Id || !p.Niv_Id); });
            var celda = aplica
                ? nombrePlantilla(aplica) + (propia ? ' <span class="badge outline-badge-info ml-1">del nivel</span>' : ' <span class="badge outline-badge-primary ml-1">por defecto</span>')
                : '<span class="text-warning"><i class="bi bi-exclamation-triangle mr-1" aria-hidden="true"></i>Sin plantilla: los docentes de este nivel solo pueden poner nota directa.</span>';
            var exc = excepciones.length
                ? excepciones.map(function (p) { return esc(p.Materia) + " → " + esc(p.Nombre); }).join("<br>")
                : '<span class="text-muted">Ninguna</span>';
            return "<tr><td>" + esc(n.Texto) + "</td><td>" + celda + "</td><td>" + exc + "</td></tr>";
        }).join("");
        $("#np-cobertura").html(html || '<tr><td colspan="3" class="text-muted">No hay niveles.</td></tr>');
    }

    function barra(tipos) {
        return '<div class="progress np-barra mb-3" role="img" aria-label="Pesos">' + tipos.map(function (t, i) {
            return '<div class="progress-bar np-c' + (i % 6) + '" style="width:' + Number(t.Peso) + '%"></div>';
        }).join("") + '</div>';
    }

    function pintarTarjetas() {
        if (!plantillas.length) {
            $("#np-lista").html('<div class="col-12"><div class="statbox widget box box-shadow mb-4"><div class="widget-content widget-content-area text-center py-5">' +
                '<i class="bi bi-sliders d-block mb-2 text-muted h2" aria-hidden="true"></i>' +
                '<h5 class="mb-1">Todavía no hay plantillas para ' + ANIO + '</h5>' +
                '<p class="text-muted mb-3">Sin plantilla, los docentes solo pueden escribir la nota del parcial directo. Empiece por la plantilla por defecto del año.</p>' +
                '<button type="button" class="btn btn-primary" data-nueva="1"><i class="bi bi-plus-lg mr-1" aria-hidden="true"></i> Crear la plantilla por defecto</button>' +
                '</div></div></div>');
            return;
        }
        $("#np-lista").html(plantillas.map(function (p) {
            var uso = p.Cuadernos
                ? "En uso en " + p.Cuadernos + " cuaderno(s)" + (p.CuadernosCerrados ? " (" + p.CuadernosCerrados + " de parciales cerrados)" : "") +
                  " · " + (p.Actividades || 0) + " actividad(es)"
                : "Todavía no se usa en ningún cuaderno";
            var lista = p.tipos.map(function (t, i) {
                return '<li><span class="np-punto np-c' + (i % 6) + '"></span>' + esc(t.Nombre) + ' <span class="np-peso">' + fmt(t.Peso) +
                    ' %</span> <span class="text-muted small">· ' + esc((COMBINACION[t.Combinacion] || t.Combinacion).toLowerCase()) + '</span></li>';
            }).join("");
            var quitar = p.Cuadernos
                ? '<button type="button" class="btn btn-sm btn-outline-danger mb-1" disabled title="Se usa en cuadernos: no se puede quitar">Quitar</button>'
                : '<button type="button" class="btn btn-sm btn-outline-danger mb-1" data-quitar="' + p.Ple_Id + '">Quitar</button>';
            return '<div class="col-xl-4 col-md-6 col-12 layout-spacing"><div class="statbox widget box box-shadow h-100 np-tarjeta">' +
                '<div class="widget-content widget-content-area h-100 d-flex flex-column">' +
                '<div class="d-flex justify-content-between align-items-start mb-2"><div class="mr-2"><h5 class="mb-1">' + esc(p.Nombre) + '</h5>' +
                '<div class="small text-muted">' + esc(p.Alcance) + '</div></div>' +
                (p.PorDefecto ? '<span class="badge badge-primary">Por defecto</span>' : (p.Mat_Id ? '<span class="badge outline-badge-info">Excepción</span>' : '')) + '</div>' +
                (p.Descripcion ? '<p class="small mb-2">' + esc(p.Descripcion) + '</p>' : "") +
                barra(p.tipos) + '<ul class="list-unstyled np-tipos mb-3">' + lista + '</ul>' +
                '<p class="small text-muted mt-auto mb-2">' + esc(uso) + (p.Modificado ? '<br>Último cambio: ' + esc(p.ModificadoPor || "") + ', ' + esc(fechaHora(p.Modificado)) : "") + '</p>' +
                '<div><button type="button" class="btn btn-sm btn-outline-primary mb-1 mr-1" data-editar="' + p.Ple_Id + '">Editar</button>' +
                '<button type="button" class="btn btn-sm btn-outline-primary mb-1 mr-1" data-copiar="' + p.Ple_Id + '">Copiar</button>' + quitar + '</div>' +
                '</div></div></div>';
        }).join(""));
    }

    // ───────────────────────────── Ventana: nivel → materia ─────────────────────────────
    function cargarMaterias(seleccion) {
        var $m = $("#np-materia");
        var bloqueado = $m.data("bloqueado");
        $m.prop("disabled", true).html('<option value="">Cargando…</option>');
        return $.getJSON(URL.opciones, { tipo: "materia", anio: $("#np-f-anio").val(), nivId: $("#np-nivel").val() || null })
            .done(function (r) {
                var lista = datos(r);
                $m.html('<option value="">Todas las materias</option>' + lista.map(function (o) {
                    return '<option value="' + o.Id + '">' + esc(o.Texto) + '</option>';
                }).join(""));
                if (seleccion && !lista.some(function (o) { return String(o.Id) === String(seleccion); }))
                    $m.append($("<option>").val(seleccion).text("(materia sin horario este año)"));
                $m.val(seleccion ? String(seleccion) : "");
            })
            .fail(function () { $m.html('<option value="">Todas las materias</option>'); })
            .always(function () { $m.prop("disabled", !!bloqueado); });
    }
    // Cascada: al cambiar el año o el nivel, la materia se limpia y se vuelve a pedir filtrada.
    $("#np-nivel, #np-f-anio").on("change", function () { cargarMaterias(null); });

    // ───────────────────────────── Ventana: tipos ─────────────────────────────
    function filaTipo(t) {
        var uso = t && t.Actividades ? t.Actividades : 0;
        var opciones = Object.keys(COMBINACION).map(function (k) {
            return '<option value="' + k + '"' + (t && t.c === k ? " selected" : "") + '>' + COMBINACION[k] + '</option>';
        }).join("");
        return '<tr data-id="' + (t && t.id ? t.id : 0) + '">' +
            '<td><input type="text" class="form-control form-control-sm np-t-nombre" maxlength="60" value="' + esc(t ? t.n : "") + '" aria-label="Nombre del tipo">' +
            '<div class="invalid-feedback">Escriba un nombre que no se repita.</div></td>' +
            '<td><input type="number" class="form-control form-control-sm np-peso-input np-t-peso" min="0.01" max="100" step="0.01" inputmode="decimal" value="' +
            (t && t.p !== undefined && t.p !== null ? Number(t.p) : "") + '" aria-label="Peso en por ciento"><div class="invalid-feedback">Mayor que 0 y hasta 100.</div></td>' +
            '<td><select class="custom-select custom-select-sm np-t-comb" aria-label="Cómo se juntan las actividades">' + opciones + '</select></td>' +
            '<td class="text-right">' + (uso
                ? '<button type="button" class="btn btn-sm btn-outline-danger" disabled title="Tiene ' + uso + ' actividad(es): no se puede quitar"><i class="bi bi-trash" aria-hidden="true"></i></button>'
                : '<button type="button" class="btn btn-sm btn-outline-danger np-t-quitar" aria-label="Quitar tipo"><i class="bi bi-trash" aria-hidden="true"></i></button>') +
            '</td></tr>';
    }

    function total() {
        var s = 0;
        $("#np-tipos .np-t-peso").each(function () { s += parseFloat($(this).val()) || 0; });
        s = Math.round(s * 100) / 100;
        var ok = s === 100;
        $("#np-total").attr("class", "mb-1 " + (ok ? "np-total-ok" : "np-total-mal"))
            .text("Total: " + fmt(s) + " %" + (ok ? "" : " (debe ser 100 %)"));
        $("#np-agregar-tipo").prop("disabled", $("#np-tipos tr").length >= 10);
        return ok;
    }
    $("#np-tipos").on("input change", ".np-t-peso, .np-t-nombre", function () {
        if ($(this).hasClass("is-invalid")) validarTipos();
        total();
    });
    $("#np-tipos").on("click", ".np-t-quitar", function () { $(this).closest("tr").remove(); total(); });
    $("#np-agregar-tipo").on("click", function () {
        if ($("#np-tipos tr").length >= 10) return;
        $("#np-tipos").append(filaTipo(null));
        $("#np-tipos tr:last .np-t-nombre").trigger("focus");
        total();
    });

    function validarTipos() {
        var ok = true, vistos = {};
        $("#np-tipos tr").each(function () {
            var $n = $(this).find(".np-t-nombre"), $p = $(this).find(".np-t-peso");
            var n = $.trim($n.val()).toLowerCase();
            var p = String($p.val()).trim();
            var okN = n.length > 0 && !vistos[n];
            vistos[n] = true;
            var v = parseFloat(p);
            var okP = /^\d+(\.\d{1,2})?$/.test(p) && v > 0 && v <= 100;
            $n.toggleClass("is-invalid", !okN);
            $p.toggleClass("is-invalid", !okP);
            ok = ok && okN && okP;
        });
        return ok && $("#np-tipos tr").length > 0;
    }

    // ───────────────────────────── Ventana: abrir ─────────────────────────────
    function abrir(p, copia) {
        editando = p && !copia ? p : null;
        var enUso = editando && editando.Cuadernos > 0;
        $("#np-form .is-invalid").removeClass("is-invalid");
        $("#np-form-error").prop("hidden", true).empty();
        $("#np-modal-titulo").text(editando ? "Editar plantilla" : (copia ? "Copiar plantilla" : "Nueva plantilla"));
        $("#np-nombre").val(p ? (copia ? "Copia de " + p.Nombre : p.Nombre) : (plantillas.some(function (x) { return x.PorDefecto; }) ? "" : "Evaluación general " + ANIO));
        $("#np-descripcion").val(p ? (p.Descripcion || "") : "");
        $("#np-f-anio").val(String(p && !copia ? p.Anio : ANIO)).prop("disabled", !!enUso);
        $("#np-nivel").val(p && p.Niv_Id ? String(p.Niv_Id) : "").prop("disabled", !!enUso);
        $("#np-materia").data("bloqueado", !!enUso);
        cargarMaterias(p && p.Mat_Id ? p.Mat_Id : null);

        var tipos = p ? p.tipos.map(function (t) {
            return { id: copia ? 0 : t.Plt_Id, n: t.Nombre, p: t.Peso, c: t.Combinacion, Actividades: copia ? 0 : t.Actividades };
        }) : SUGERIDOS;
        $("#np-tipos").html(tipos.map(filaTipo).join(""));
        total();

        $("#np-form-uso").html(enUso ? alerta("warning", '<i class="bi bi-exclamation-triangle mr-1" aria-hidden="true"></i>' +
            "Esta plantilla se usa en " + editando.Cuadernos + " cuaderno(s)" +
            (editando.CuadernosCerrados ? " (" + editando.CuadernosCerrados + " de parciales cerrados, que conservan sus pesos)" : "") +
            ". Los cambios de nombres y pesos se aplican a los parciales abiertos y sus notas se recalculan. No se puede cambiar el año, el nivel ni la materia.") : "")
            .prop("hidden", !enUso);
        $("#np-modal").modal("show");
    }

    $("#np-nueva").on("click", function () { abrir(null, false); });
    $(document).on("click", "[data-nueva]", function () { abrir(null, false); });
    $(document).on("click", "[data-editar]", function () {
        var id = +$(this).data("editar");
        abrir(plantillas.filter(function (p) { return p.Ple_Id === id; })[0], false);
    });
    $(document).on("click", "[data-copiar]", function () {
        var id = +$(this).data("copiar");
        abrir(plantillas.filter(function (p) { return p.Ple_Id === id; })[0], true);
    });
    $("#np-nombre").on("input blur", function () { $(this).toggleClass("is-invalid", !$.trim($(this).val())); });

    $("#np-form").on("submit", function (e) {
        e.preventDefault();
        var okNombre = !!$.trim($("#np-nombre").val());
        $("#np-nombre").toggleClass("is-invalid", !okNombre);
        var okTipos = validarTipos();
        var okTotal = total();
        if (!okNombre || !okTipos || !okTotal) {
            $("#np-form-error").html(alerta("danger", !okTotal && okTipos ? "Los pesos deben sumar 100 %." : "Revise los campos marcados en rojo."))
                .prop("hidden", false);
            return;
        }
        var tipos = $("#np-tipos tr").map(function () {
            return {
                id: +$(this).data("id") || 0,
                n: $.trim($(this).find(".np-t-nombre").val()),
                p: parseFloat($(this).find(".np-t-peso").val()),
                c: $(this).find(".np-t-comb").val()
            };
        }).get();
        var payload = {
            Ple_Id: editando ? editando.Ple_Id : 0,
            Anio: parseInt($("#np-f-anio").val(), 10),
            Niv_Id: $("#np-nivel").val() ? parseInt($("#np-nivel").val(), 10) : null,
            Mat_Id: $("#np-materia").val() ? parseInt($("#np-materia").val(), 10) : null,
            Nombre: $.trim($("#np-nombre").val()),
            Descripcion: $.trim($("#np-descripcion").val()) || null,
            Tipos: JSON.stringify(tipos),
            Version: editando ? editando.Version : 0
        };
        var $b = $("#np-form-guardar").prop("disabled", true);
        enviar(URL.guardar, payload)
            .done(function (r) {
                if (!r || !r.success) {
                    $("#np-form-error").html(alerta("danger", esc((r && r.message) || "No se pudo guardar la plantilla."))).prop("hidden", false);
                    return;
                }
                $("#np-modal").modal("hide");
                toastr.success(r.message || "Plantilla guardada.");
                if (r.aviso) toastr.warning(r.aviso);
                if (payload.Anio !== ANIO) { window.location.href = window.location.pathname + "?anio=" + payload.Anio; return; }
                cargar();
                if (tabla) tabla.recargar();
            })
            .fail(function () { $("#np-form-error").html(alerta("danger", "No se pudo conectar con el servidor. Intente de nuevo.")).prop("hidden", false); })
            .always(function () { $b.prop("disabled", false); });
    });

    // ───────────────────────────── Quitar ─────────────────────────────
    $(document).on("click", "[data-quitar]", function () {
        var id = +$(this).data("quitar");
        quitando = plantillas.filter(function (p) { return p.Ple_Id === id; })[0];
        if (!quitando) return;
        $("#np-quitar-texto").html("Se quitará la plantilla <strong>" + esc(quitando.Nombre) + "</strong> (" + esc(quitando.Alcance) + "). " +
            "Todavía no se usa en ningún cuaderno.");
        $("#np-modal-quitar").modal("show");
    });
    $("#np-quitar-confirmar").on("click", function () {
        if (!quitando) return;
        var $b = $(this).prop("disabled", true);
        enviar(URL.quitar, { Ple_Id: quitando.Ple_Id, Version: quitando.Version })
            .done(function (r) {
                $("#np-modal-quitar").modal("hide");
                if (!r || !r.success) { toastr.error((r && r.message) || "No se pudo quitar la plantilla."); return; }
                toastr.success("Plantilla quitada.");
                cargar();
            })
            .fail(function () { toastr.error("No se pudo conectar con el servidor."); })
            .always(function () { $b.prop("disabled", false); quitando = null; });
    });

    // ───────────────────────────── Uso en los cuadernos ─────────────────────────────
    var ESTADO = {
        directa: '<span class="badge outline-badge-secondary">Nota directa</span>',
        en_curso: '<span class="badge outline-badge-warning">En curso</span>',
        completo: '<span class="badge outline-badge-success">Completo</span>'
    };
    function urlUso() {
        return URL.avance + "?anio=" + ANIO + "&semId=" + encodeURIComponent($("#np-sem").val()) + "&pacId=" + encodeURIComponent($("#np-par").val());
    }
    function indicadores(filas) {
        var conAct = 0, cap = 0, esp = 0, dia = 0, calc = 0;
        filas.forEach(function (f) {
            if (f.Actividades > 0) conAct++;
            cap += f.Capturadas; esp += f.Esperadas; dia += f.NotasAlDia; calc += f.ConCalculo;
        });
        $("#np-kpi-act").text(conAct + " de " + filas.length);
        $("#np-kpi-act-texto").text("materias y secciones del parcial; las demás siguen con nota directa");
        $("#np-kpi-cal").text(esp ? Math.floor(1000 * cap / esp) / 10 + " %" : "—");
        $("#np-kpi-cal-texto").text(esp ? fmt(cap) + " de " + fmt(esp) + " calificaciones esperadas donde hay actividades" : "Todavía no hay actividades en este parcial");
        $("#np-kpi-dia").text(calc ? Math.floor(1000 * dia / calc) / 10 + " %" : "—");
        $("#np-kpi-dia-texto").text(calc ? fmt(dia) + " de " + fmt(calc) + " notas calculadas ya están en las notas del parcial" : "Sin notas calculadas todavía");
    }
    function crearTabla() {
        var archivo = "uso-actividades-" + ANIO + "-s" + $("#np-sem").val() + "-p" + $("#np-par").val();
        tabla = GcTabla.crear({
            elemento: "#np-tabla-uso", nombre: "uso-actividades", url: urlUso(),
            archivo: archivo, tituloPdf: "Uso de actividades · " + ANIO,
            buscarEn: ["Grupo", "Curso", "Materia", "Docente", "Plantilla"],
            agrupar: [{ campos: ["Docente"], texto: "Docente" }, { campos: ["Grado", "Seccion"], texto: "Grado → Sección" }],
            alCargar: indicadores,
            columnas: [
                { title: "Sección", field: "Grupo", width: 120, mutator: function (v, d) { return d.Grado + " " + d.Seccion; } },
                { title: "Curso", field: "Curso", width: 170, formatter: "textarea" },
                { title: "Materia", field: "Materia", width: 180, formatter: "textarea" },
                { title: "Docente", field: "Docente", width: 170, formatter: "textarea" },
                { title: "Plantilla", field: "Plantilla", width: 150, formatter: "textarea",
                  mutator: function (v) { return v || "Sin plantilla"; } },
                { title: "Actividades", field: "Actividades", hozAlign: "right", width: 105 },
                { title: "Calificado", field: "Porcentaje", hozAlign: "right", width: 115,
                  mutator: function (v) { return v === null || v === undefined ? null : Number(v); },
                  formatter: function (c) {
                      var d = c.getData();
                      return d.Esperadas ? fmt(c.getValue()) + ' % <span class="text-muted small">(' + d.Capturadas + "/" + d.Esperadas + ")</span>" : '<span class="text-muted">—</span>';
                  } },
                { title: "Notas al día", field: "NotasAlDia", hozAlign: "right", width: 115,
                  formatter: function (c) { var d = c.getData(); return d.ConCalculo ? d.NotasAlDia + " de " + d.ConCalculo : '<span class="text-muted">—</span>'; } },
                { title: "Estado", field: "Estado", width: 115, formatter: function (c) { return ESTADO[c.getValue()] || esc(c.getValue()); },
                  accessorDownload: function (v) { return { directa: "Nota directa", en_curso: "En curso", completo: "Completo" }[v] || v; } },
                { title: "Cuaderno", field: "Hor_Id", headerSort: false, download: false, width: 105,
                  formatter: function (c) {
                      var d = c.getData();
                      var href = URL.cuaderno + "?sem=" + encodeURIComponent($("#np-sem").val()) + "&hor=" + d.Hor_Id + "&par=" + d.Pac_Id;
                      return '<a class="btn btn-sm btn-outline-primary" href="' + esc(href) + '">Abrir</a>';
                  } }
            ]
        });
    }
    $("#np-tab-uso").on("shown.bs.tab", function () { if (!tabla) crearTabla(); });
    $("#np-sem, #np-par").on("change", function () { if (tabla) tabla.tabla.setData(urlUso()); });

    $("#np-anio").on("change", function () { this.form.submit(); });

    // ───────────────────────────── Inicio ─────────────────────────────
    $.getJSON(URL.opciones, { tipo: "nivel" }).done(function (r) {
        niveles = datos(r);
        $("#np-nivel").html('<option value="">Todos los niveles</option>' + niveles.map(function (n) {
            return '<option value="' + n.Id + '">' + esc(n.Texto) + '</option>';
        }).join(""));
        pintarCobertura();
    });
    cargar();
}(jQuery));
