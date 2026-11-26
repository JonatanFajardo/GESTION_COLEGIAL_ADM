/* Cierre de parciales — indicadores del año, una tarjeta por semestre × parcial (avance de notas, estado,
   fechas y acciones), notas faltantes por sección/materia/docente (GcTabla, exportable), historial y la
   ventana de acciones (cerrar con confirmación de faltantes, publicar, retirar publicación, reabrir con
   motivo, fechas de carga). Todo texto que escribió una persona se escapa antes de pintarlo. */
(function ($) {
    "use strict";

    var $raiz = $("#cp-raiz");
    if (!$raiz.length) return;

    var URL = {
        estado: $raiz.data("url-estado"),
        faltantes: $raiz.data("url-faltantes"),
        historial: $raiz.data("url-historial"),
        cerrar: $raiz.data("url-cerrar"),
        publicar: $raiz.data("url-publicar"),
        despublicar: $raiz.data("url-despublicar"),
        reabrir: $raiz.data("url-reabrir"),
        fechas: $raiz.data("url-fechas")
    };
    var ANIO = parseInt($raiz.data("anio"), 10);
    var PUEDE = String($raiz.data("puede-cerrar")) === "1";

    var periodos = [];
    var tabla = null, opcionesTabla = null;

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function num(n) { return (Number(n) || 0).toLocaleString("es-HN"); }

    function pct(parte, total) {
        if (!total) return null;
        return Math.floor(1000 * parte / total) / 10;
    }

    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    function fecha(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1] : "";
    }
    function fechaHora(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/.exec(v || "");
        return m ? fecha(v) + ", " + m[4] + ":" + m[5] : fecha(v);
    }
    function iso(v) { var m = /^(\d{4}-\d{2}-\d{2})/.exec(v || ""); return m ? m[1] : ""; }

    function nombre(p) { return p.Parcial + " del " + String(p.Semestre || "").toLowerCase() + " " + p.Anio; }
    function buscar(sem, pac) {
        return periodos.filter(function (p) { return p.Sem_Id === sem && p.Pac_Id === pac; })[0] || null;
    }

    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }

    function enviar(url, datos) {
        return $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos),
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    function datosDe(r) { return (r && r.data) ? r.data : (Array.isArray(r) ? r : []); }

    // ───────────────────────────── Estado ─────────────────────────────

    function cargar() {
        $.getJSON(URL.estado, { anio: ANIO })
            .done(function (r) {
                periodos = datosDe(r);
                pintarIndicadores();
                pintarPeriodos();
            })
            .fail(function () {
                $("#cp-periodos").html('<div class="alert alert-light-danger mb-4" role="alert">No se pudo calcular el avance de notas. Recargue la página.</div>');
            });
        cargarHistorial();
    }

    function pintarIndicadores() {
        var esp = 0, car = 0, falt = 0, abiertosFalt = 0, cerr = 0, pub = 0;
        periodos.forEach(function (p) {
            esp += p.Esperadas; car += p.Cargadas;
            if (p.Estado === "cerrado") cerr++;
            else if (p.Faltantes > 0) { falt += p.Faltantes; abiertosFalt++; }
            if (p.Publicado) pub++;
        });
        var total = periodos.length;
        var porc = pct(car, esp);
        $("#kpi-avance").text(porc === null ? "—" : porc.toLocaleString("es-HN") + " %");
        $("#kpi-avance-texto").text(esp ? num(car) + " de " + num(esp) + " notas esperadas del año" : "Sin notas esperadas: no hay horarios del año");
        $("#kpi-faltantes").text(num(falt));
        $("#kpi-faltantes-texto").text(falt ? "En " + abiertosFalt + " parcial" + (abiertosFalt === 1 ? "" : "es") + " abierto" + (abiertosFalt === 1 ? "" : "s") : "Los parciales abiertos están completos");
        $("#kpi-cerrados").text(cerr + " de " + total);
        $("#kpi-publicados").text(pub + " de " + total);

        var p0 = periodos[0];
        var $aviso = $("#cp-aviso-horario");
        if (p0 && p0.SeccionesSinHorario > 0) {
            $aviso.html('<div class="alert alert-light-info mb-4" role="status"><i class="bi bi-info-circle-fill mr-1" aria-hidden="true"></i>' +
                '<strong>' + num(p0.SeccionesSinHorario) + ' sección' + (p0.SeccionesSinHorario === 1 ? '' : 'es') + ' con alumnos no tiene' + (p0.SeccionesSinHorario === 1 ? '' : 'n') + ' horario en ' + ANIO + '</strong> ' +
                '(' + num(p0.AlumnosSinHorario) + ' alumno' + (p0.AlumnosSinHorario === 1 ? '' : 's') + '): no se puede saber qué notas esperar de ellos, así que no cuentan en el avance.</div>')
                .removeAttr("hidden");
        } else {
            $aviso.attr("hidden", true).empty();
        }
    }

    function insignia(p) {
        if (p.Publicado) return '<span class="badge badge-success"><i class="bi bi-megaphone mr-1" aria-hidden="true"></i>Publicado</span>';
        if (p.Estado === "cerrado") return '<span class="badge badge-info"><i class="bi bi-lock mr-1" aria-hidden="true"></i>Cerrado</span>';
        return '<span class="badge badge-warning"><i class="bi bi-unlock mr-1" aria-hidden="true"></i>Abierto</span>';
    }

    function boton(accion, p, clase, icono, texto) {
        return '<button type="button" class="btn ' + clase + ' btn-sm mr-2 mb-2" data-accion="' + accion + '" data-sem="' + p.Sem_Id + '" data-pac="' + p.Pac_Id + '">' +
            '<i class="' + icono + ' mr-1" aria-hidden="true"></i>' + texto + '</button>';
    }

    function tarjeta(p) {
        var porc = pct(p.Cargadas, p.Esperadas);
        var lineas = [];

        if (p.Esperadas === 0) {
            lineas.push('<i class="bi bi-info-circle mr-1" aria-hidden="true"></i>Sin notas esperadas (no hay horario para este semestre).');
        } else if (p.Faltantes > 0) {
            lineas.push('<span class="cp-falta"><i class="bi bi-exclamation-triangle mr-1" aria-hidden="true"></i>Faltan ' + num(p.Faltantes) + ' notas en ' +
                p.SeccionesConFaltantes + ' sección' + (p.SeccionesConFaltantes === 1 ? '' : 'es') + ' (' + p.MateriasConFaltantes + ' materia' + (p.MateriasConFaltantes === 1 ? '' : 's') + ')</span>');
        } else {
            lineas.push('<span class="cp-completo"><i class="bi bi-check-circle mr-1" aria-hidden="true"></i>Todas las notas esperadas están cargadas</span>');
        }

        if (p.FechaInicio || p.FechaLimite) {
            var t = 'Carga de notas: ' + (p.FechaInicio ? 'desde ' + fecha(p.FechaInicio) : '') +
                (p.FechaInicio && p.FechaLimite ? ' ' : '') + (p.FechaLimite ? 'hasta ' + fecha(p.FechaLimite) : '');
            lineas.push('<i class="bi bi-calendar-event mr-1" aria-hidden="true"></i>' + t +
                (p.LimiteVencido ? ' <span class="badge outline-badge-danger ml-1">Fecha límite vencida</span>' : ''));
        } else if (p.Estado !== "cerrado") {
            lineas.push('<i class="bi bi-calendar mr-1" aria-hidden="true"></i>Sin fecha límite de carga' +
                (p.SemestreFin ? ' (el semestre termina el ' + fecha(p.SemestreFin) + ')' : ''));
        }
        if (p.SemestreInicio) {
            lineas.push('<i class="bi bi-calendar-range mr-1" aria-hidden="true"></i>Semestre: desde ' + fecha(p.SemestreInicio) +
                (p.SemestreFin ? ' hasta ' + fecha(p.SemestreFin) : ''));
        }

        if (p.Estado === "cerrado" && p.FechaCierre) {
            lineas.push('<i class="bi bi-lock mr-1" aria-hidden="true"></i>Cerrado el ' + fechaHora(p.FechaCierre) + ' por ' + esc(p.UsuarioCierre || "—"));
        }
        if (p.Publicado && p.FechaPublicacion) {
            lineas.push('<i class="bi bi-megaphone mr-1" aria-hidden="true"></i>Publicado el ' + fechaHora(p.FechaPublicacion) + ' por ' + esc(p.UsuarioPublicacion || "—"));
        }

        var acciones = '';
        if (p.Faltantes > 0) acciones += boton("faltantes", p, "btn-outline-primary", "bi bi-list-check", "Ver faltantes");
        if (PUEDE) {
            if (p.Estado !== "cerrado") {
                acciones += boton("cerrar", p, "btn-primary", "bi bi-lock", "Cerrar parcial");
                acciones += boton("fechas", p, "btn-link", "bi bi-calendar-event", "Fechas de carga");
            } else {
                if (!p.Publicado) acciones += boton("publicar", p, "btn-success", "bi bi-megaphone", "Publicar resultados");
                else acciones += boton("despublicar", p, "btn-outline-secondary", "bi bi-eye-slash", "Retirar publicación");
                acciones += boton("reabrir", p, "btn-outline-danger", "bi bi-unlock", "Reabrir");
            }
        }

        var claseBarra = porc === null ? "bg-secondary" : (porc >= 100 ? "bg-success" : "bg-warning");
        return '' +
            '<div class="col-lg-6 col-12 layout-spacing">' +
            '  <div class="statbox widget box box-shadow h-100 cp-periodo" data-sem="' + p.Sem_Id + '" data-pac="' + p.Pac_Id + '">' +
            '    <div class="widget-content widget-content-area h-100 d-flex flex-column">' +
            '      <div class="d-flex align-items-start justify-content-between mb-3">' +
            '        <div class="mr-2"><h5 class="mb-0">' + esc(p.Parcial) + '</h5><span class="text-muted small">' + esc(p.Semestre) + ' ' + p.Anio + '</span>' +
            (p.SemestreVigente ? ' <span class="badge outline-badge-primary ml-1">Semestre vigente</span>' : '') + '</div>' +
            '        ' + insignia(p) +
            '      </div>' +
            '      <div class="d-flex justify-content-between small mb-1">' +
            '        <span>' + num(p.Cargadas) + ' de ' + num(p.Esperadas) + ' notas cargadas</span>' +
            '        <strong>' + (porc === null ? '—' : porc.toLocaleString("es-HN") + ' %') + '</strong>' +
            '      </div>' +
            '      <div class="progress cp-progreso mb-3" role="progressbar" aria-label="Notas cargadas" aria-valuemin="0" aria-valuemax="100" aria-valuenow="' + (porc || 0) + '">' +
            '        <div class="progress-bar ' + claseBarra + '" style="width:' + (porc || 0) + '%"></div>' +
            '      </div>' +
            '      <ul class="list-unstyled small mb-0 cp-lineas">' + lineas.map(function (l) { return '<li>' + l + '</li>'; }).join("") + '</ul>' +
            (acciones ? '      <div class="mt-auto pt-3 d-flex flex-wrap">' + acciones + '</div>' : '') +
            '    </div>' +
            '  </div>' +
            '</div>';
    }

    function pintarPeriodos() {
        if (!periodos.length) {
            $("#cp-periodos").html('<div class="alert alert-light-info mb-4" role="status">No hay semestres ni parciales en el catálogo.</div>');
            return;
        }
        var html = '', semActual = null;
        periodos.forEach(function (p) {
            if (p.Sem_Id !== semActual) {
                if (semActual !== null) html += '</div>';
                html += '<h5 class="cp-semestre mb-3">' + esc(p.Semestre) + '</h5><div class="row">';
                semActual = p.Sem_Id;
            }
            html += tarjeta(p);
        });
        html += '</div>';
        $("#cp-periodos").html(html);
    }

    // ───────────────────────────── Faltantes ─────────────────────────────

    function verFaltantes(p) {
        var url = URL.faltantes + "?anio=" + ANIO + "&semId=" + p.Sem_Id + "&pacId=" + p.Pac_Id;
        $("#cp-faltantes-titulo").text("Notas faltantes · " + nombre(p));
        $("#cp-faltantes-sub").text("Faltan " + num(p.Faltantes) + " notas en " + p.SeccionesConFaltantes + " sección" +
            (p.SeccionesConFaltantes === 1 ? "" : "es") + ". Exporte la lista para pedírselas a cada docente.");
        $("#cp-faltantes").removeAttr("hidden");

        var archivo = "notas-faltantes-" + ANIO + "-s" + p.Sem_Id + "-p" + p.Pac_Id;
        var titulo = "Notas faltantes · " + nombre(p);
        if (!tabla) {
            opcionesTabla = {
                elemento: "#tabla-faltantes", nombre: "cierre-faltantes", url: url,
                archivo: archivo, tituloPdf: titulo,
                vacio: "No faltan notas en este parcial.",
                buscarEn: ["Grupo", "Carrera", "Materia", "Docentes"],
                agrupar: [{ campos: ["Docentes"], texto: "Docente" }, { campos: ["Grado", "Seccion"], texto: "Grado → Sección" }],
                columnas: [
                    { title: "Sección", field: "Grupo", width: 145, formatter: "textarea",
                      mutator: function (v, d) { return d.Grado + " " + d.Seccion + " · " + d.Modalidad; } },
                    { title: "Carrera", field: "Carrera", width: 155, formatter: "textarea" },
                    { title: "Materia", field: "Materia", width: 155, formatter: "textarea" },
                    { title: "Docente", field: "Docentes", width: 205, formatter: "textarea" },
                    { title: "Alumnos", field: "Alumnos", hozAlign: "right", width: 90 },
                    { title: "Faltan", field: "Faltan", hozAlign: "right", width: 80,
                      formatter: function (c) { return '<strong class="cp-falta">' + num(c.getValue()) + '</strong>'; } }
                ]
            };
            tabla = GcTabla.crear(opcionesTabla);
        } else {
            opcionesTabla.archivo = archivo;
            opcionesTabla.tituloPdf = titulo;
            tabla.tabla.setData(url);
        }
        var el = document.getElementById("cp-faltantes");
        if (el && el.scrollIntoView) el.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    $("#cp-faltantes-cerrar").on("click", function () { $("#cp-faltantes").attr("hidden", true); });

    // ───────────────────────────── Historial ─────────────────────────────

    var ACCIONES = {
        cerrar: { texto: "Cerró", clase: "badge-info" },
        reabrir: { texto: "Reabrió", clase: "badge-warning" },
        publicar: { texto: "Publicó", clase: "badge-success" },
        despublicar: { texto: "Retiró publicación", clase: "badge-secondary" },
        fechas: { texto: "Fechas de carga", clase: "outline-badge-primary" }
    };

    function cargarHistorial() {
        $.getJSON(URL.historial, { anio: ANIO })
            .done(function (r) {
                var filas = datosDe(r), $tb = $("#cp-historial").empty();
                if (!filas.length) {
                    $tb.append('<tr><td colspan="6" class="text-center text-muted">Todavía no se ha cerrado ni publicado ningún parcial de ' + ANIO + '.</td></tr>');
                    return;
                }
                filas.forEach(function (h) {
                    var a = ACCIONES[h.Accion] || { texto: h.Accion, clase: "badge-secondary" };
                    $tb.append('<tr>' +
                        '<td class="text-nowrap">' + fechaHora(h.Fecha) + '</td>' +
                        '<td>' + esc(h.Parcial) + '<span class="d-block text-muted small">' + esc(h.Semestre) + '</span></td>' +
                        '<td><span class="badge ' + a.clase + '">' + esc(a.texto) + '</span></td>' +
                        '<td class="cp-motivo">' + (h.Motivo ? esc(h.Motivo) : '<span class="text-muted">—</span>') + '</td>' +
                        '<td class="text-right">' + (h.Accion === "cerrar" && h.Faltantes !== null ? num(h.Faltantes) : '<span class="text-muted">—</span>') + '</td>' +
                        '<td>' + esc(h.Usuario) + '</td>' +
                        '</tr>');
                });
            })
            .fail(function () {
                $("#cp-historial").html('<tr><td colspan="6" class="text-center text-danger">No se pudo cargar el historial.</td></tr>');
            });
    }

    // ───────────────────────────── Acciones ─────────────────────────────

    var $modal = $("#cp-modal");
    var accionActual = null, periodoActual = null;

    function errorModal(msg) {
        var $e = $("#cp-modal-error");
        if (!msg) { $e.attr("hidden", true).text(""); return; }
        $e.text(msg).removeAttr("hidden");
    }

    function abrir(accion, p) {
        accionActual = accion; periodoActual = p;
        errorModal(null);
        $("#cp-modal-faltantes, #cp-modal-confirmar-grupo, #cp-modal-motivo-grupo, #cp-modal-fechas").attr("hidden", true);
        $("#cp-modal-motivo").val("").removeClass("is-invalid");
        $("#cp-modal-confirmar").prop("checked", false);

        var titulo, texto, boton, clase, motivo = null;
        switch (accion) {
            case "cerrar":
                titulo = "Cerrar el " + p.Parcial.toLowerCase();
                texto = "Al cerrar el " + nombre(p).toLowerCase() + " nadie podrá registrar, cambiar ni quitar sus notas. Si después hace falta una corrección, puede reabrirlo con un motivo.";
                boton = "Cerrar parcial"; clase = "btn-primary";
                if (p.Faltantes > 0) {
                    $("#cp-modal-faltantes").html('<i class="bi bi-exclamation-triangle-fill mr-1" aria-hidden="true"></i><strong>Faltan ' + num(p.Faltantes) +
                        ' notas</strong> en ' + p.SeccionesConFaltantes + ' sección' + (p.SeccionesConFaltantes === 1 ? '' : 'es') +
                        '. Lo recomendable es exigirlas antes de cerrar (use "Ver faltantes").').removeAttr("hidden");
                    $("#cp-modal-confirmar-grupo").removeAttr("hidden");
                    motivo = { etiqueta: "¿Por qué lo cierra con notas faltantes?", obligatorio: true };
                } else {
                    motivo = { etiqueta: "Comentario (opcional)", obligatorio: false };
                }
                break;
            case "publicar":
                titulo = "Publicar resultados";
                texto = "Se publicarán los resultados del " + nombre(p).toLowerCase() + ": el boletín dejará de mostrarlos como provisionales.";
                boton = "Publicar resultados"; clase = "btn-success";
                break;
            case "despublicar":
                titulo = "Retirar la publicación";
                texto = "El boletín volverá a mostrar las notas del " + nombre(p).toLowerCase() + " como provisionales. El parcial sigue cerrado.";
                boton = "Retirar publicación"; clase = "btn-danger";
                motivo = { etiqueta: "Motivo", obligatorio: true };
                break;
            case "reabrir":
                titulo = "Reabrir el " + p.Parcial.toLowerCase();
                texto = "Los docentes podrán volver a registrar y cambiar las notas del " + nombre(p).toLowerCase() + "." +
                    (p.Publicado ? " También se retira la publicación de sus resultados." : "");
                boton = "Reabrir parcial"; clase = "btn-danger";
                motivo = { etiqueta: "¿Qué corrección se autorizó?", obligatorio: true };
                break;
            case "fechas":
                titulo = "Fechas de carga de notas";
                texto = "Desde y hasta cuándo se cargan las notas del " + nombre(p).toLowerCase() + ".";
                boton = "Guardar fechas"; clase = "btn-primary";
                $("#cp-modal-inicio").val(iso(p.FechaInicio));
                $("#cp-modal-limite").val(iso(p.FechaLimite));
                $("#cp-modal-fechas").removeAttr("hidden");
                break;
            default:
                return;
        }

        if (motivo) {
            $("#cp-modal-motivo-label").text(motivo.etiqueta + (motivo.obligatorio ? " *" : ""));
            $("#cp-modal-motivo").attr("aria-required", motivo.obligatorio ? "true" : "false").data("obligatorio", motivo.obligatorio);
            $("#cp-modal-motivo-grupo").removeAttr("hidden");
        }
        $("#cp-modal-titulo").text(titulo);
        $("#cp-modal-texto").text(texto);
        $("#cp-modal-aceptar").attr("class", "btn " + clase).text(boton).prop("disabled", false);
        $modal.modal("show");
    }

    function validarMotivo() {
        var $m = $("#cp-modal-motivo");
        if ($("#cp-modal-motivo-grupo").is("[hidden]") || !$m.data("obligatorio")) return true;
        var ok = $.trim($m.val()).length > 0;
        $m.toggleClass("is-invalid", !ok);
        $("#cp-modal-motivo-error").text(ok ? "" : "Escriba el motivo: queda en el historial.");
        return ok;
    }

    $("#cp-modal-motivo").on("input", function () { if ($(this).hasClass("is-invalid")) validarMotivo(); });

    $("#cp-modal-aceptar").on("click", function () {
        var p = periodoActual;
        if (!p || !accionActual) return;
        errorModal(null);

        if (accionActual === "cerrar" && p.Faltantes > 0 && !$("#cp-modal-confirmar").is(":checked")) {
            errorModal("Marque la casilla para confirmar que cierra el parcial aunque falten notas.");
            return;
        }
        if (!validarMotivo()) return;

        var inicio = $("#cp-modal-inicio").val(), limite = $("#cp-modal-limite").val();
        if (accionActual === "fechas" && inicio && limite && inicio > limite) {
            errorModal("La fecha de inicio no puede ser posterior a la fecha límite.");
            return;
        }

        var datos = {
            Anio: p.Anio, Sem_Id: p.Sem_Id, Pac_Id: p.Pac_Id,
            Motivo: $.trim($("#cp-modal-motivo").val()) || null,
            ConfirmarFaltantes: accionActual === "cerrar" && $("#cp-modal-confirmar").is(":checked"),
            FechaInicio: accionActual === "fechas" ? (inicio || null) : null,
            FechaLimite: accionActual === "fechas" ? (limite || null) : null
        };

        var $btn = $(this).prop("disabled", true);
        enviar(URL[accionActual], datos)
            .done(function (r) {
                if (r && r.success) {
                    $modal.modal("hide");
                    if (window.toastr) toastr.success(r.message || "Listo.");
                    cargar();
                } else {
                    errorModal((r && r.message) || "No se pudo completar la acción.");
                    $btn.prop("disabled", false);
                }
            })
            .fail(function () {
                errorModal("No se pudo conectar con el servidor. Intente de nuevo.");
                $btn.prop("disabled", false);
            });
    });

    // ───────────────────────────── Eventos ─────────────────────────────

    $("#cp-periodos").on("click", "[data-accion]", function () {
        var p = buscar(parseInt($(this).data("sem"), 10), parseInt($(this).data("pac"), 10));
        if (!p) return;
        var accion = $(this).data("accion");
        if (accion === "faltantes") verFaltantes(p);
        else if (PUEDE) abrir(accion, p);
    });

    $("#cp-anio").on("change", function () { this.form.submit(); });

    cargar();
}(jQuery));
