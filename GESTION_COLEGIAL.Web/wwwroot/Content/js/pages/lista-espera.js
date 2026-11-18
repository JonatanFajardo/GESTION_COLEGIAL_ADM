/* Lista de espera de admisión — la lista (resumen por grado + cola), el registro de un aspirante
   y su detalle. Cada parte se activa si su contenedor existe en la página. */
(function ($) {
    "use strict";

    var ESTADOS = {
        EE: { texto: "En espera", insignia: "badge-warning" },
        OF: { texto: "Ofrecido", insignia: "badge-info" },
        MA: { texto: "Matriculado", insignia: "badge-success" },
        RE: { texto: "Rechazado", insignia: "badge-danger" },
        DE: { texto: "Desistió", insignia: "badge-secondary" }
    };
    var PRIORIDADES = {
        0: { texto: "Normal", insignia: "outline-badge-secondary" },
        1: { texto: "Media", insignia: "outline-badge-warning" },
        2: { texto: "Alta", insignia: "outline-badge-danger" }
    };

    function esc(t) { return $("<div>").text(t == null ? "" : String(t)).html(); }

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

    function aviso(texto, tipo) {
        if (window.toastr) toastr[tipo || "success"](texto);
    }

    function errorGeneral(texto) {
        $("#lde-error-general").text(texto).removeAttr("hidden");
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    /* Combo hijo de la cascada: se vacía, se deshabilita y (si hay padre) se llena con lo que pide al servidor. */
    function limpiarCombo($sel, texto) {
        $sel.empty().append('<option value="">' + esc(texto) + "</option>").prop("disabled", true).val("");
    }

    function llenarCombo($sel, url, id, textoVacio) {
        return $.getJSON(url, { id: id }).then(function (r) {
            var lista = (r && r.data) || [];
            $sel.empty().append('<option value="">' + esc(textoVacio) + "</option>");
            lista.forEach(function (x) { $sel.append('<option value="' + esc(x.Value) + '">' + esc(x.Text) + "</option>"); });
            $sel.prop("disabled", lista.length === 0);
            return lista;
        }, function () {
            limpiarCombo($sel, "No se pudo cargar");
        });
    }

    /* ───────────── Ventana para mover de estado (lista y detalle) ───────────── */
    var $modal = $("#lde-modal-estado");
    var alTerminar = null;

    var ACCIONES = {
        ofrecer: {
            titulo: "Ofrecer cupo", boton: "Ofrecer cupo", clase: "btn-primary", motivo: false, etiqueta: "Nota (opcional)",
            texto: function (o) { return "¿Ofrecer el cupo de " + o.grado + " a " + o.nombre + "? La oferta reserva un asiento hasta que la familia acepte (se matricula) o desista."; }
        },
        rechazar: {
            titulo: "Rechazar aspirante", boton: "Rechazar", clase: "btn-danger", motivo: true, etiqueta: "Motivo",
            texto: function (o) { return o.nombre + " sale de la cola. Se puede reactivar después."; }
        },
        desistio: {
            titulo: "La familia desistió", boton: "Marcar que desistió", clase: "btn-primary", motivo: true, etiqueta: "Motivo",
            texto: function (o) { return o.nombre + " sale de la cola porque la familia ya no quiere el cupo. Se puede reactivar después."; }
        },
        reactivar: {
            titulo: "Reactivar aspirante", boton: "Reactivar", clase: "btn-primary", motivo: true, etiqueta: "Motivo",
            texto: function (o) { return o.nombre + " vuelve a la lista en espera, al final de la cola de " + o.grado + "."; }
        }
    };

    function abrirModal(o, terminar) {
        var a = ACCIONES[o.accion];
        if (!a || !$modal.length) return;
        alTerminar = terminar;
        $("#lde-modal-id").val(o.id);
        $("#lde-modal-accion").val(o.accion);
        $("#lde-modal-titulo").text(a.titulo);
        $("#lde-modal-texto").text(a.texto(o));
        $("#lde-modal-motivo-etiqueta").text(a.etiqueta);
        $("#lde-modal-motivo").val("").removeClass("is-invalid").attr("placeholder", a.motivo ? "Por qué (obligatorio)" : "Opcional");
        $("#lde-modal-error").text("");
        var $adv = $("#lde-modal-advertencia");
        if (o.accion === "ofrecer" && !o.cupoDefinido) {
            $adv.html('<i class="fas fa-exclamation-triangle mr-1" aria-hidden="true"></i> El cupo de ' + esc(o.grado) +
                " no está definido en las secciones del año: se ofrecerá sin control de asientos.").removeAttr("hidden");
        } else {
            $adv.attr("hidden", true).empty();
        }
        $("#lde-modal-confirmar").attr("class", "btn " + a.clase).text(a.boton).prop("disabled", false);
        $modal.modal("show");
    }

    $modal.on("shown.bs.modal", function () { $("#lde-modal-motivo").trigger("focus"); });

    $("#lde-modal-confirmar").on("click", function () {
        var accion = $("#lde-modal-accion").val();
        var a = ACCIONES[accion];
        var motivo = $.trim($("#lde-modal-motivo").val());
        if (a.motivo && !motivo) {
            $("#lde-modal-motivo").addClass("is-invalid").trigger("focus");
            $("#lde-modal-error").text("Escriba el motivo.");
            return;
        }
        var $btn = $(this).prop("disabled", true);
        postJson($("[data-url-estado]").first().data("url-estado"), {
            Asp_Id: parseInt($("#lde-modal-id").val(), 10),
            Accion: accion,
            Motivo: motivo
        }).done(function (r) {
            if (!r || !r.success) {
                $("#lde-modal-error").text((r && r.message) || "No se pudo cambiar el estado.");
                $btn.prop("disabled", false);
                return;
            }
            $modal.modal("hide");
            aviso(r.message || "Listo.");
            if (r.aviso) aviso(r.aviso, "warning");
            if (alTerminar) alTerminar(r);
        }).fail(function () {
            $("#lde-modal-error").text("No se pudo conectar. Intente de nuevo.");
            $btn.prop("disabled", false);
        });
    });

    /* ───────────── La lista ───────────── */
    var $lista = $("#lde-lista");
    if ($lista.length) {
        var gestionar = $lista.data("gestionar") === 1 || $lista.data("gestionar") === "1";
        var matricular = $lista.data("matricular") === 1 || $lista.data("matricular") === "1";
        var urlAspirante = $lista.data("url-aspirante");
        var urlMatricula = $lista.data("url-matricula");

        $("#lde-anio").on("change", function () {
            window.location = window.location.pathname + "?anio=" + encodeURIComponent($(this).val());
        });

        // ── Indicadores y resumen por grado ──
        function numero(n) { return (n == null ? 0 : n).toLocaleString("es-HN"); }

        function cargarResumen() {
            $.getJSON($lista.data("url-resumen")).done(function (r) {
                var filas = (r && r.data) || [];
                var espera = 0, ofertas = 0, matric = 0, libres = 0, sinCupo = 0, grados = 0;
                var html = "";
                filas.forEach(function (g) {
                    espera += g.EnEspera; ofertas += g.Ofertas; matric += g.MatriculadosLista;
                    if (g.CupoDefinido) libres += Math.max(0, g.Libres || 0); else sinCupo++;
                    if (g.EnEspera > 0) grados++;
                    var demanda = g.CupoDefinido && g.EnEspera > Math.max(0, g.Libres || 0)
                        ? ' <span class="badge outline-badge-danger ml-1" title="Hay más aspirantes en espera que asientos libres">Demanda mayor que el cupo</span>' : "";
                    html += "<tr>" +
                        '<td><strong>' + esc(g.Cun_Descripcion) + '</strong><span class="d-block small text-muted">' + esc(g.Niv_Descripcion) + "</span></td>" +
                        '<td class="text-right">' + (g.CupoDefinido ? numero(g.Cupo) : '<span class="badge outline-badge-warning" title="' +
                            esc(g.SeccionesConCupo + " de " + g.Secciones + " secciones con cupo") + '">Sin definir</span>') + "</td>" +
                        '<td class="text-right">' + numero(g.Matriculados) + "</td>" +
                        '<td class="text-right">' + numero(g.Ofertas) + "</td>" +
                        '<td class="text-right">' + numero(g.EnEspera) + demanda + "</td>" +
                        '<td class="text-right font-weight-bold">' + (g.CupoDefinido ? numero(g.Libres) : "—") + "</td>" +
                        "<td>" + (g.SiguienteId ? '<a href="' + urlAspirante + "/" + g.SiguienteId + '">' + esc(g.SiguienteNombre) + "</a>" : '<span class="text-muted">Nadie en espera</span>') + "</td>" +
                        "</tr>";
                });
                $("#lde-resumen-cuerpo").html(html || '<tr><td colspan="7" class="text-center text-muted py-4">No hay grados configurados.</td></tr>');
                $("#kpi-espera").text(numero(espera));
                $("#kpi-espera-nota").text(grados ? "En " + grados + (grados === 1 ? " grado" : " grados") : "Nadie en la cola");
                $("#kpi-ofertas").text(numero(ofertas));
                $("#kpi-matriculados").text(numero(matric));
                $("#kpi-matriculados-nota").text("Ya son alumnos de este año");
                $("#kpi-libres").text(sinCupo === filas.length && filas.length ? "—" : numero(libres));
                $("#kpi-libres-nota").text(sinCupo ? sinCupo + (sinCupo === 1 ? " grado con cupo sin definir" : " grados con cupo sin definir") : "En los grados con cupo definido");
            }).fail(function () {
                $("#lde-resumen-cuerpo").html('<tr><td colspan="7" class="text-center text-danger py-4">No se pudo cargar el resumen.</td></tr>');
            });
        }
        cargarResumen();

        // ── La cola ──
        function insigniaEstado(e) {
            var x = ESTADOS[e] || { texto: e, insignia: "badge-secondary" };
            return '<span class="badge ' + x.insignia + '">' + esc(x.texto) + "</span>";
        }
        function boton(accion, clase, icono, titulo) {
            return '<button type="button" class="btn btn-sm ' + clase + '" data-accion="' + accion + '" title="' + titulo + '" aria-label="' + titulo + '">' +
                '<i class="bi ' + icono + '" aria-hidden="true"></i></button>';
        }
        // Solo íconos (con su título) para que la tabla quepa en 1366 px; la acción principal va en color.
        function acciones(d) {
            var b = '<div class="lde-acciones text-nowrap" data-id="' + d.Asp_Id + '">';
            if (gestionar) {
                if (d.Asp_Estado === "EE" && d.PuedeOfrecer) b += boton("ofrecer", "btn-primary", "bi-envelope-paper", "Ofrecer cupo");
                if (d.Asp_Estado === "OF" && matricular) {
                    b += '<a class="btn btn-sm btn-success" href="' + urlMatricula + "?aspiranteId=" + d.Asp_Id + '" title="Matricular con el wizard" aria-label="Matricular">' +
                        '<i class="bi bi-person-check" aria-hidden="true"></i></a>';
                }
                if (d.Asp_Estado === "EE" || d.Asp_Estado === "OF") {
                    b += boton("rechazar", "btn-outline-danger", "bi-x-circle", "Rechazar");
                    b += boton("desistio", "btn-outline-secondary", "bi-box-arrow-left", "Desistió");
                }
                if (d.Asp_Estado === "RE" || d.Asp_Estado === "DE") b += boton("reactivar", "btn-outline-primary", "bi-arrow-counterclockwise", "Reactivar");
            }
            b += '<a class="btn btn-sm btn-outline-primary" href="' + urlAspirante + "/" + d.Asp_Id + '" title="Ver aspirante" aria-label="Ver aspirante"><i class="bi bi-eye" aria-hidden="true"></i></a>';
            return b + "</div>";
        }

        // Primera vez de este usuario: la cola se muestra agrupada por grado (luego manda lo que él elija).
        var claveTabla = "gc-tabla-lista-espera-" + ($("#tabla-espera").data("usuario") || "anonimo");
        try {
            if (!localStorage.getItem(claveTabla)) localStorage.setItem(claveTabla, JSON.stringify({ orden: [], agrupar: "Grado" }));
        } catch (x) { }

        var tabla = GcTabla.crear({
            elemento: "#tabla-espera",
            nombre: "lista-espera",
            url: $lista.data("url-lista"),
            archivo: "lista-de-espera",
            tituloPdf: "Lista de espera",
            vacio: "No hay aspirantes con estos filtros.",
            buscarEn: ["Aspirante", "Asp_Identidad", "Encargado", "Grado", "EstadoTexto"],
            agrupar: [{ campos: ["Grado"], texto: "Grado" }],
            columnas: [
                {
                    title: "Lugar", field: "Lugar", hozAlign: "center", headerHozAlign: "center", headerSort: false, width: 95,
                    formatter: function (c) {
                        var d = c.getData();
                        if (d.Asp_Estado === "EE") return '<span class="badge ' + (d.EsSiguiente ? "badge-primary" : "outline-badge-primary") + '" title="' +
                            (d.EsSiguiente ? "Es el siguiente de la cola" : "Lugar en la cola") + '">' + esc(d.Lugar) + "</span>";
                        if (d.Asp_Estado === "OF") return '<span class="badge outline-badge-info">Oferta</span>';
                        return '<span class="text-muted">—</span>';
                    }
                },
                {
                    title: "Aspirante", field: "Aspirante", headerSort: false, widthGrow: 3,
                    formatter: function (c) {
                        var d = c.getData();
                        var sub = [d.Mda_Descripcion, d.Asp_Identidad].filter(Boolean).join(" · ");
                        return '<div class="lde-celda"><a class="lde-nombre" href="' + urlAspirante + "/" + d.Asp_Id + '" title="' + esc(d.Aspirante) + '">' +
                            esc(d.Aspirante) + "</a>" + (sub ? '<span class="d-block small text-muted">' + esc(sub) + "</span>" : "") + "</div>";
                    }
                },
                { title: "Edad", field: "Edad", hozAlign: "center", headerHozAlign: "center", headerSort: false, width: 90 },
                {
                    title: "Encargado", field: "Encargado", headerSort: false, widthGrow: 2,
                    formatter: function (c) {
                        var d = c.getData();
                        return '<div class="lde-celda"><span class="d-block" title="' + esc(d.Asp_EncargadoNombre) + '">' + esc(d.Asp_EncargadoNombre) + "</span>" +
                            '<span class="d-block small text-muted"><i class="bi bi-telephone mr-1" aria-hidden="true"></i>' + esc(d.Asp_EncargadoTelefono) + "</span></div>";
                    }
                },
                {
                    title: "Prioridad", field: "PrioridadTexto", headerSort: false, width: 131,
                    formatter: function (c) {
                        var d = c.getData(), p = PRIORIDADES[d.Asp_Prioridad] || PRIORIDADES[0];
                        return '<span class="badge ' + p.insignia + '"' + (d.Asp_MotivoPrioridad ? ' title="' + esc(d.Asp_MotivoPrioridad) + '"' : "") + ">" + p.texto + "</span>";
                    }
                },
                { title: "Fecha", field: "Solicitud", headerSort: false, headerTooltip: "Fecha de solicitud", width: 95 },
                { title: "Estado", field: "EstadoTexto", headerSort: false, width: 104, formatter: function (c) { return insigniaEstado(c.getData().Asp_Estado); } },
                {
                    title: "Acciones", field: "Acciones", headerSort: false, download: false, width: gestionar ? 144 : 122,
                    formatter: function (c) { return acciones(c.getData()); }
                }
            ],
            filtroExtra: function (d) {
                var niv = $("#f-nivel").val(), cun = $("#f-grado").val(), mda = $("#f-modalidad").val(), est = $("#f-estado").val();
                if (niv && String(d.Niv_Id) !== niv) return false;
                if (cun && String(d.Cun_Id) !== cun) return false;
                if (mda && String(d.Mda_Id) !== mda) return false;
                if (est === "abiertos") return d.Asp_Estado === "EE" || d.Asp_Estado === "OF";
                if (est && d.Asp_Estado !== est) return false;
                return true;
            },
            alCargar: function () { setTimeout(function () { tabla.filtrar(); }, 0); }
        });

        // ── Filtros en cascada Nivel → Grado → Modalidad ──
        $("#f-nivel").on("change", function () {
            var id = $(this).val();
            limpiarCombo($("#f-grado"), id ? "Todos los grados" : "Elija nivel");
            limpiarCombo($("#f-modalidad"), "Elija grado");
            if (id) llenarCombo($("#f-grado"), $lista.data("url-grados"), id, "Todos los grados");
            tabla.filtrar();
        });
        $("#f-grado").on("change", function () {
            var id = $(this).val();
            limpiarCombo($("#f-modalidad"), id ? "Todas" : "Elija grado");
            if (id) llenarCombo($("#f-modalidad"), $lista.data("url-modalidades"), id, "Todas");
            tabla.filtrar();
        });
        $("#f-modalidad, #f-estado").on("change", function () { tabla.filtrar(); });
        $("#f-limpiar").on("click", function () {
            $("#f-nivel").val("");
            limpiarCombo($("#f-grado"), "Elija nivel");
            limpiarCombo($("#f-modalidad"), "Elija grado");
            $("#f-estado").val("abiertos");
            $("#tabla-espera-buscar").val("");
            tabla.filtrar();
        });

        // ── Acciones de la tabla ──
        $("#tabla-espera").on("click", "[data-accion]", function () {
            var id = $(this).closest("[data-id]").data("id");
            var fila = tabla.tabla.getData().filter(function (x) { return x.Asp_Id === id; })[0];
            if (!fila) return;
            abrirModal({
                id: id, accion: $(this).data("accion"), nombre: fila.Aspirante, grado: fila.Grado, cupoDefinido: fila.CupoDefinido
            }, function () { tabla.recargar(); cargarResumen(); });
        });
    }

    /* ───────────── Detalle del aspirante ───────────── */
    var $detalle = $("#lde-aspirante");
    if ($detalle.length) {
        $detalle.on("click", "[data-accion]", function () {
            abrirModal({
                id: $detalle.data("id"), accion: $(this).data("accion"), nombre: $detalle.data("nombre"),
                grado: $detalle.data("grado"), cupoDefinido: String($detalle.data("cupo-definido")) === "1"
            }, function () { window.location.reload(); });
        });
    }

    /* ───────────── Registro de aspirante ───────────── */
    var $form = $("#lde-form");
    if ($form.length) {
        var bloqueado = String($form.data("bloqueado")) === "1";
        var aspId = parseInt($form.data("id"), 10) || 0;

        function $err(campo) { return $form.find('[data-error="' + campo + '"]'); }
        function marcar(campo, $control, mensaje) {
            $err(campo).text(mensaje || "");
            if ($control && $control.length) $control.toggleClass("is-invalid", !!mensaje).attr("aria-invalid", mensaje ? "true" : null);
        }

        // ── Cascada Nivel → Grado → Modalidad → Curso ──
        var $nivel = $("#nivel"), $grado = $("#grado"), $modalidad = $("#modalidad"), $curso = $("#curso");

        function cargarGrados(nivId) {
            limpiarCombo($grado, nivId ? "Cargando…" : "Elija el nivel primero");
            limpiarCombo($modalidad, "Elija el grado primero");
            limpiarCombo($curso, "Elija la modalidad primero");
            if (!nivId) return $.Deferred().resolve().promise();
            return llenarCombo($grado, $form.data("url-grados"), nivId, "Elija el grado").then(function () {
                if (bloqueado) $grado.prop("disabled", true);
            });
        }
        function cargarModalidades(cunId) {
            limpiarCombo($modalidad, cunId ? "Cargando…" : "Elija el grado primero");
            limpiarCombo($curso, "Elija la modalidad primero");
            if (!cunId) return $.Deferred().resolve().promise();
            return llenarCombo($modalidad, $form.data("url-modalidades"), cunId, "Cualquiera");
        }
        function cargarCursos(cunId, mdaId) {
            limpiarCombo($curso, mdaId ? "Cargando…" : "Elija la modalidad primero");
            if (!cunId || !mdaId) return $.Deferred().resolve().promise();
            return llenarCombo($curso, $form.data("url-cursos"), cunId, "Sin curso definido");
        }

        $nivel.on("change", function () { marcar("nivel", $nivel, ""); cargarGrados($nivel.val()); });
        $grado.on("change", function () { marcar("grado", $grado, ""); cargarModalidades($grado.val()); });
        $modalidad.on("change", function () { cargarCursos($grado.val(), $modalidad.val()); });

        // Edición: reconstruir la cascada con los valores guardados.
        if ($nivel.val()) {
            var g = $form.data("grado"), m = $form.data("modalidad"), cu = $form.data("curso");
            cargarGrados($nivel.val()).then(function () {
                if (!g) return;
                $grado.val(String(g));
                return cargarModalidades(g).then(function () {
                    if (!m) return;
                    $modalidad.val(String(m));
                    return cargarCursos(g, m).then(function () { if (cu) $curso.val(String(cu)); });
                });
            });
        }

        // ── Edad en vivo ──
        function edad(fecha) {
            if (!fecha) return null;
            var n = new Date(fecha + "T00:00:00"), h = new Date();
            var e = h.getFullYear() - n.getFullYear();
            if (h.getMonth() < n.getMonth() || (h.getMonth() === n.getMonth() && h.getDate() < n.getDate())) e--;
            return e;
        }
        function mostrarEdad() {
            var e = edad($("#nacimiento").val());
            var $t = $("#nacimiento-edad").removeClass("text-danger").addClass("text-muted");
            if (e === null || isNaN(e) || e < 0) { $t.text(""); return; }
            $t.text("· " + e + (e === 1 ? " año" : " años"));
            if (e < 4 || e > 22) $t.removeClass("text-muted").addClass("text-danger").text("· " + e + " años: revise la fecha");
        }
        $("#nacimiento").on("change input", mostrarEdad);
        mostrarEdad();

        // ── Prioridad: el motivo solo se pide si no es normal ──
        function mostrarMotivoPrioridad() {
            var p = parseInt($("#prioridad").val(), 10) || 0;
            $("#grupo-motivo-prioridad").toggle(p > 0);
            if (p === 0) marcar("motivoprioridad", $("#motivoprioridad"), "");
        }
        $("#prioridad").on("change", mostrarMotivoPrioridad);
        mostrarMotivoPrioridad();

        // ── Validación en vivo ──
        var REGLAS = {
            nombres: function () { return $.trim($("#nombres").val()) ? "" : "Escriba los nombres."; },
            apellidos: function () { return $.trim($("#apellidos").val()) ? "" : "Escriba los apellidos."; },
            identidad: function () { var v = $.trim($("#identidad").val()); return !v || /^\d{13}$/.test(v) ? "" : "La identidad lleva 13 números (o déjela vacía)."; },
            nacimiento: function () {
                var v = $("#nacimiento").val();
                if (!v) return "Indique la fecha de nacimiento.";
                var e = edad(v);
                return e < 0 || new Date(v) > new Date() ? "La fecha no puede ser futura." : (parseInt(v, 10) < 1990 ? "Revise el año." : "");
            },
            sexo: function () { return $("#sexo").val() ? "" : "Elija el sexo."; },
            encargado: function () { return $.trim($("#encargado").val()) ? "" : "Escriba el nombre del encargado."; },
            parentesco: function () { return $("#parentesco").val() ? "" : "Elija el parentesco."; },
            encidentidad: function () { var v = $.trim($("#encidentidad").val()); return !v || /^\d{13}$/.test(v) ? "" : "La identidad lleva 13 números (o déjela vacía)."; },
            telefono: function () { return ($("#telefono").val() || "").replace(/\D/g, "").length >= 8 ? "" : "Escriba un teléfono de al menos 8 números."; },
            correo: function () { var v = $.trim($("#correo").val()); return !v || /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(v) ? "" : "Revise el correo."; },
            anio: function () { return $("#anio").val() ? "" : "Elija el año lectivo."; },
            nivel: function () { return $nivel.val() ? "" : "Elija el nivel."; },
            grado: function () { return $grado.val() ? "" : "Elija el grado."; },
            motivoprioridad: function () {
                return (parseInt($("#prioridad").val(), 10) || 0) === 0 || $.trim($("#motivoprioridad").val()) ? "" : "Diga por qué tiene prioridad.";
            },
            solicitud: function () { var v = $("#solicitud").val(); return !v || new Date(v + "T00:00:00") <= new Date() ? "" : "La fecha de solicitud no puede ser futura."; }
        };
        function validar(campo) {
            var msg = REGLAS[campo]();
            marcar(campo, $("#" + campo), msg);
            return !msg;
        }
        Object.keys(REGLAS).forEach(function (campo) {
            $("#" + campo).on("blur change", function () { validar(campo); })
                          .on("input", function () { if ($(this).hasClass("is-invalid")) validar(campo); });
        });
        $("#identidad, #encidentidad").on("input", function () {
            var limpio = this.value.replace(/\D/g, "").slice(0, 13);
            if (limpio !== this.value) this.value = limpio;
        });

        // ── ¿Ya está en la lista? ¿Ya es alumno? ──
        var temporizador = null, hayDuplicado = false;
        function revisarDuplicado() {
            clearTimeout(temporizador);
            temporizador = setTimeout(function () {
                var ident = $.trim($("#identidad").val()), nom = $.trim($("#nombres").val()), ape = $.trim($("#apellidos").val()), nac = $("#nacimiento").val();
                var porIdentidad = /^\d{13}$/.test(ident), porNombre = nom && ape && nac;
                var $d = $("#lde-duplicado");
                if (!porIdentidad && !porNombre) { $d.attr("hidden", true).empty(); hayDuplicado = false; return; }
                $.getJSON($form.data("url-duplicado"), {
                    aniId: $("#anio").val(), identidad: porIdentidad ? ident : null,
                    nombres: porNombre ? nom : null, apellidos: porNombre ? ape : null, fechaNacimiento: porNombre ? nac : null,
                    aspId: aspId || null
                }).done(function (r) {
                    hayDuplicado = !!(r && r.duplicado);
                    if (r && r.duplicado) {
                        var html = '<i class="fas fa-exclamation-triangle mr-1" aria-hidden="true"></i> ' + esc(r.duplicado);
                        if (r.duplicadoId) html += ' <a href="' + $form.data("url-aspirante") + "/" + r.duplicadoId + '">Ver aspirante</a>';
                        $d.attr("class", "alert alert-light-danger mb-3").html(html).removeAttr("hidden");
                    } else if (r && r.alumno) {
                        $d.attr("class", "alert alert-light-warning mb-3").html('<i class="bi bi-info-circle mr-1" aria-hidden="true"></i> ' + esc(r.alumno)).removeAttr("hidden");
                    } else {
                        $d.attr("hidden", true).empty();
                    }
                });
            }, 400);
        }
        $("#identidad, #nombres, #apellidos").on("blur", revisarDuplicado);
        $("#nacimiento, #anio").on("change", revisarDuplicado);
        if (aspId) revisarDuplicado();

        // ── Guardar ──
        function entero(v) { var n = parseInt(v, 10); return isNaN(n) || n <= 0 ? null : n; }

        $form.on("submit", function (e) {
            e.preventDefault();
            $("#lde-error-general").attr("hidden", true).text("");
            var primero = null;
            Object.keys(REGLAS).forEach(function (campo) {
                if (!validar(campo) && !primero) primero = $("#" + campo);
            });
            if (primero) { primero.trigger("focus"); return; }
            if (hayDuplicado) { $("#lde-duplicado")[0].scrollIntoView({ behavior: "smooth", block: "center" }); return; }

            var prioridad = parseInt($("#prioridad").val(), 10) || 0;
            var datos = {
                Asp_Id: aspId,
                Ani_Id: entero($("#anio").val()),
                Asp_Nombres: $.trim($("#nombres").val()),
                Asp_Apellidos: $.trim($("#apellidos").val()),
                Asp_Identidad: $.trim($("#identidad").val()) || null,
                Asp_FechaNacimiento: $("#nacimiento").val() || null,
                Asp_Sexo: $("#sexo").val(),
                Asp_EscuelaProcedencia: $.trim($("#escuela").val()) || null,
                Asp_EncargadoNombre: $.trim($("#encargado").val()),
                Asp_EncargadoIdentidad: $.trim($("#encidentidad").val()) || null,
                Par_Id: entero($("#parentesco").val()),
                Asp_EncargadoTelefono: $.trim($("#telefono").val()),
                Asp_EncargadoCorreo: $.trim($("#correo").val()) || null,
                Niv_Id: entero($nivel.val()),
                Cun_Id: entero($grado.val()),
                Mda_Id: entero($modalidad.val()),
                Cur_Id: entero($curso.val()),
                Asp_FechaSolicitud: $("#solicitud").val() || null,
                Asp_Prioridad: prioridad,
                Asp_MotivoPrioridad: prioridad > 0 ? $.trim($("#motivoprioridad").val()) : null,
                Asp_Notas: $.trim($("#notas").val()) || null
            };

            var $btn = $("#lde-guardar").prop("disabled", true);
            postJson($form.data("url"), datos).done(function (r) {
                if (!r || !r.success) {
                    if (r && r.field && $err(r.field).length) {
                        marcar(r.field, $("#" + r.field), r.message);
                        $("#" + r.field).trigger("focus");
                    } else {
                        errorGeneral((r && r.message) || "No se pudo guardar el aspirante.");
                    }
                    $btn.prop("disabled", false);
                    return;
                }
                if (!r.nuevo) { window.location = r.url; return; }
                $form.attr("hidden", true);
                $("#lde-resultado-titulo").text("Aspirante registrado");
                $("#lde-resultado-texto").html('Quedó en el lugar <strong class="lde-resultado-posicion d-block my-2">' + esc(r.posicionCola) + " de " + esc(r.enCola) +
                    "</strong>en la cola de " + esc(r.grado) + ".");
                if (r.aviso) { $("#lde-resultado-aviso").text(r.aviso); $("#lde-resultado-aviso-caja").removeAttr("hidden"); }
                $("#lde-resultado-ver").attr("href", r.url);
                $("#lde-resultado").removeAttr("hidden");
                window.scrollTo({ top: 0, behavior: "smooth" });
            }).fail(function () {
                errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo.");
                $btn.prop("disabled", false);
            });
        });
    }
}(jQuery));
