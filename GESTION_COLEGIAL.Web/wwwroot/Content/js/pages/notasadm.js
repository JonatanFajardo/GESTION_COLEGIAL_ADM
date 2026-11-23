/* Hub "Notas y Boletín"
   Tab 1: Cuaderno de Notas (captura por Semestre + Materia/Sección + Parcial) — endpoints NotasAdm
   Tab 2: Boletín (consulta por Alumno + Año) — endpoints BoletinAdm (reusados vía AJAX)
   Columnas del boletín dinámicas según el catálogo real de Parciales.
   Umbral rojo = nota mínima configurable (GraduacionAlumnos/NotaMinimaAsync, fallback 70). */
var NotasAdm = (function () {
    var obj = {};
    var urls = {};
    var contexto = {};
    var parciales = [];        // catálogo real de parciales [{id, texto}]
    var escala = [];           // rangos de la escala de notas [{Esc_Concepto, Esc_Abreviatura, Esc_NotaDesde, ...}]
    var horarios = [];         // [{id, sem, anio, texto}] para saber el período del horario elegido
    var cierre = { soloLectura: false, mensaje: "" };   // parcial cerrado por Dirección (Cierre de parciales)

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    // Parcial cerrado = solo lectura: aviso arriba de la tabla y sin campos ni botones para guardar.
    function aplicarCierre() {
        var $aviso = $("#nb-aviso-cierre");
        if (cierre.soloLectura) {
            $aviso.html('<div class="alert alert-light-warning mb-4" role="alert"><i class="bi bi-lock-fill mr-1" aria-hidden="true"></i>' +
                '<strong>Solo lectura.</strong> ' + esc(cierre.mensaje) + '</div>').removeAttr("hidden");
        } else {
            $aviso.attr("hidden", true).empty();
        }
        $("#tbody-notas .nota-input, #tbody-notas .btn-guardar, #btn-guardar-todo").prop("disabled", cierre.soloLectura);
    }

    function verificarCierre(horId, parId) {
        var h = horarios.filter(function (x) { return String(x.id) === String(horId); })[0];
        cierre = { soloLectura: false, mensaje: "" };
        aplicarCierre();
        if (!h || !urls.periodo) return;
        $.getJSON(urls.periodo, { anio: h.anio || new Date().getFullYear(), semId: h.sem, pacId: parId }, function (r) {
            cierre = { soloLectura: !!(r && r.cerrado), mensaje: (r && r.mensaje) || "" };
            aplicarCierre();
        });
    }

    // Traduce un promedio al concepto de la escala (misma lógica que FN_EscalaConcepto:
    // el rango con la mayor nota mínima que no supera el promedio).
    function conceptoDe(prom) {
        if (prom === null || isNaN(parseFloat(prom))) return "";
        var v = parseFloat(prom), mejor = null;
        $.each(escala, function (_, e) {
            var d = parseFloat(e.Esc_NotaDesde);
            if (v >= d && (mejor === null || d > parseFloat(mejor.Esc_NotaDesde))) mejor = e;
        });
        if (!mejor) return "";
        return mejor.Esc_Concepto + (mejor.Esc_Abreviatura ? " (" + mejor.Esc_Abreviatura + ")" : "");
    }
    var notaMinima = 70;       // fallback si el GET de configuración falla

    // ---------- Utilidades ----------
    function llenarSelect(sel, items, textoDefault) {
        sel.empty().append('<option value="">' + textoDefault + '</option>');
        $.each(items, function (_, item) {
            sel.append('<option value="' + item.id + '">' + esc(item.texto) + '</option>');
        });
    }

    function esNumero(v) {
        return v !== null && v !== undefined && v !== "" && !isNaN(parseFloat(v));
    }

    function fmtNota(v) {
        return esNumero(v) ? parseFloat(v).toFixed(2) : "-";
    }

    function claseNota(v) {
        return (esNumero(v) && parseFloat(v) < notaMinima) ? ' class="text-center text-danger font-weight-bold"' : ' class="text-center"';
    }

    // ---------- Tabs ----------
    function initTabs() {
        $("#nb-tabs .nav-link").on("click", function (e) {
            e.preventDefault();
            var tab = $(this).data("tab");
            $("#nb-tabs .nav-link").removeClass("active");
            $(this).addClass("active");
            $(".nb-tab-panel").attr("hidden", true);
            $("#nb-panel-" + tab).removeAttr("hidden");
        });
    }

    // ---------- Tab 1: Cuaderno de Notas ----------
    function initCuaderno() {
        // Semestre -> materia y sección (cascada: el segundo se habilita al elegir el primero)
        $("#f-sem").on("change", function () {
            var sem = $(this).val();
            var $hor = $("#f-hor");
            var items = horarios.filter(function (h) { return String(h.sem) === String(sem); });
            llenarSelect($hor, items, "-- Seleccione Materia y sección --");
            $hor.prop("disabled", !sem);
            limpiarTabla();
        });
        $("#f-hor, #f-par").on("change", limpiarTabla);

        function limpiarTabla() {
            contexto = {};
            $("#tbody-notas").empty();
            $("#btn-guardar-todo").prop("disabled", true);
            cierre = { soloLectura: false, mensaje: "" };
            aplicarCierre();
        }

        $("#btn-cargar").on("click", function () {
            contexto = {
                Hor_Id: $("#f-hor").val(),
                Par_Id: $("#f-par").val()
            };
            if (!contexto.Hor_Id || !contexto.Par_Id) {
                toastr.warning("Seleccione el semestre, la materia con su sección y el parcial.");
                return;
            }
            var h = horarios.filter(function (x) { return String(x.id) === String(contexto.Hor_Id); })[0];
            contexto.Anio = h ? h.anio : new Date().getFullYear();
            contexto.Sem_Id = h ? h.sem : "";
            verificarCierre(contexto.Hor_Id, contexto.Par_Id);
            $.getJSON(urls.list, { Hor_Id: contexto.Hor_Id, Par_Id: contexto.Par_Id }, function (data) {
                var tbody = $("#tbody-notas").empty();
                if (data && data.success === false) {
                    toastr.error(data.message || "No se pudo cargar la lista.");
                    tbody.append('<tr><td colspan="3" class="text-center text-muted">' + esc(data.message || "No se pudo cargar la lista.") + '</td></tr>');
                    $("#btn-guardar-todo").prop("disabled", true);
                    return;
                }
                if (!data || data.length === 0) {
                    tbody.append('<tr><td colspan="3" class="text-center text-muted">No hay alumnos activos en esta sección.</td></tr>');
                    $("#btn-guardar-todo").prop("disabled", true);
                    return;
                }
                $.each(data, function (_, row) {
                    var valor = esNumero(row.Not_Nota) ? String(parseInt(row.Not_Nota, 10)) : "";
                    var btn   = '<button class="btn btn-sm btn-success btn-guardar" data-alu="' + row.Alu_Id + '">Guardar</button>';
                    var input = '<input type="number" class="form-control nota-input" step="1" min="0" max="100"' +
                                ' data-orig="' + valor + '" value="' + valor + '" aria-label="Nota de ' + esc(row.Alu_NombreCompleto) + '">';
                    tbody.append("<tr><td>" + esc(row.Alu_NombreCompleto) + "</td><td style=\"width:160px\">" + input + "</td><td style=\"width:120px\">" + btn + "</td></tr>");
                });
                $("#btn-guardar-todo").prop("disabled", false);
                aplicarCierre();
            }).fail(function () {
                toastr.error("No se pudo cargar la lista de alumnos.");
            });
        });

        // Un cambio en la nota vuelve a habilitar su botón Guardar.
        $(document).on("input", ".nota-input", function () {
            var tr = $(this).closest("tr");
            tr.find(".btn-guardar").removeClass("btn-secondary").addClass("btn-success").text("Guardar");
        });

        // Valida 0-100 entero. Devuelve el mensaje de error o "" si está bien.
        function validarNota(v) {
            if (!esNumero(v)) return "Escriba la nota.";
            var n = parseFloat(v);
            if (n < 0 || n > 100 || n !== Math.floor(n)) return "La nota debe ser un número entero entre 0 y 100.";
            return "";
        }

        // Guarda la nota de una fila. cb(ok, mensaje) es opcional (usado por Guardar Todo).
        function guardarNota(tr, cb) {
            if (cierre.soloLectura) {
                if (!cb) toastr.warning(cierre.mensaje);
                if (cb) cb(false, cierre.mensaje);
                return;
            }
            var btn   = tr.find(".btn-guardar");
            var input = tr.find(".nota-input");
            var nota  = input.val();
            var error = validarNota(nota);
            if (error) {
                input.addClass("is-invalid");
                if (!cb) toastr.error(error);
                if (cb) cb(false, error);
                return;
            }
            input.removeClass("is-invalid");
            $.post(urls.guardar, {
                Hor_Id:   contexto.Hor_Id,
                Alu_Id:   btn.data("alu"),
                Par_Id:   contexto.Par_Id,
                Anio:     contexto.Anio,
                Sem_Id:   contexto.Sem_Id,
                Not_Nota: nota
            }, function (res) {
                var ok = !!(res && res.success);
                if (ok) {
                    input.data("orig", String(nota)).attr("data-orig", String(nota));
                    btn.removeClass("btn-success").addClass("btn-secondary").text("Guardado");
                    if (!cb) toastr.success("Nota guardada.");
                } else if (res && res.message && !cb) {
                    toastr.error(res.message);
                }
                if (res && res.cerrado) { cierre = { soloLectura: true, mensaje: res.message }; aplicarCierre(); }
                if (cb) cb(ok, res && res.message);
            }).fail(function () {
                if (!cb) toastr.error("No se pudo guardar la nota. Intente de nuevo.");
                if (cb) cb(false, "No se pudo guardar la nota.");
            });
        }

        $(document).on("click", ".btn-guardar", function () {
            guardarNota($(this).closest("tr"));
        });

        // ---------- Guardar Todo: solo las notas escritas o cambiadas ----------
        $("#btn-guardar-todo").on("click", function () {
            if (cierre.soloLectura) { toastr.warning(cierre.mensaje); return; }
            var filas = $("#tbody-notas tr").filter(function () {
                var $i = $(this).find(".nota-input");
                return $i.length && esNumero($i.val()) && String($i.val()) !== String($i.attr("data-orig"));
            });

            if (filas.length === 0) {
                toastr.warning("No hay notas nuevas ni cambiadas para guardar.");
                return;
            }

            var $btn = $(this).prop("disabled", true);
            var htmlOriginal = $btn.html();
            var total = filas.length, procesadas = 0, exitosas = 0, ultimoError = "";

            function actualizarProgreso() {
                $btn.html('<i class="fa-solid fa-spinner fa-spin"></i> Guardando ' + procesadas + '/' + total + '...');
            }
            actualizarProgreso();

            filas.each(function () {
                guardarNota($(this), function (ok, mensaje) {
                    procesadas++;
                    if (ok) exitosas++;
                    else if (mensaje) ultimoError = mensaje;
                    if (procesadas < total) {
                        actualizarProgreso();
                        return;
                    }
                    $btn.prop("disabled", cierre.soloLectura).html(htmlOriginal);
                    if (exitosas === total) {
                        toastr.success("Se guardaron " + exitosas + " nota(s) correctamente.");
                    } else {
                        toastr.warning(exitosas + " de " + total + " nota(s) guardadas. " + (total - exitosas) + " fallaron" +
                            (ultimoError ? ": " + ultimoError : ", intente de nuevo."));
                    }
                });
            });
        });
    }

    // ---------- Tab 2: Boletín ----------
    function renderCabeceraBoletin() {
        var tr = $("#thead-boletin").empty();
        tr.append('<th>Materia</th>');
        $.each(parciales, function (_, p) {
            tr.append('<th class="text-center" data-pac="' + p.id + '">' + p.texto + '</th>');
        });
        tr.append('<th class="text-center">Promedio</th>');
        tr.append('<th class="text-center">Concepto</th>');
    }

    // Busca la nota de un parcial del catálogo dentro del mapa de notas de una materia.
    // idx es la posición (1-based) del parcial en el catálogo, como fallback por número.
    function notaDeParcial(notas, parcial, idx) {
        var candidatos = [String(parcial.id), parcial.texto, String(idx), "Parcial " + idx];
        for (var i = 0; i < candidatos.length; i++) {
            if (notas.hasOwnProperty(candidatos[i])) return notas[candidatos[i]];
        }
        return null;
    }

    function initBoletin() {
        $("#btn-ver").on("click", function () {
            var alu  = $("#f-alu").val();
            var anio = $("#f-anio").val();
            if (!alu || !anio) { alert("Seleccione el alumno y el año."); return; }

            $.getJSON(urls.boletinGet, { Alu_Id: alu, Anio: anio }, function (res) {
                // La API puede devolver directo el array o envuelto en .data
                var data = (res && res.data) ? res.data : (res || []);
                var tbody = $("#tbody-boletin").empty();
                var totalCols = parciales.length + 3; // Materia + parciales + Promedio + Concepto

                renderCabeceraBoletin();

                // Parciales que Dirección aún no publica: aviso y marca "Provisional" en su columna.
                if (window.BoletinProvisional && urls.boletinPublicacion) {
                    BoletinProvisional.marcar(urls.boletinPublicacion, anio, "#nb-boletin-provisional", function (porPac) {
                        $("#thead-boletin th[data-pac]").each(function () {
                            if (porPac[$(this).data("pac")]) $(this).append(' <span class="badge outline-badge-warning ml-1">Provisional</span>');
                        });
                    });
                }

                if (!data || data.length === 0) {
                    tbody.append('<tr><td colspan="' + totalCols + '" class="text-center text-muted">Sin notas registradas para este alumno.</td></tr>');
                    $("#panel-boletin").show();
                    return;
                }

                // Agrupar por materia; indexar la nota bajo todas las llaves conocidas del parcial
                var materias = {};
                $.each(data, function (_, r) {
                    var mat = r.Mat_Nombre || "Sin materia";
                    if (!materias[mat]) materias[mat] = {};
                    var llaves = [r.Par_Id, r.Pac_Id, r.Par_Numero, r.Pac_Numero, r.Pac_Descripcion, r.Par_Descripcion];
                    $.each(llaves, function (_, k) {
                        if (k !== null && k !== undefined && k !== "") materias[mat][String(k)] = r.Not_Nota;
                    });
                });

                $.each(materias, function (mat, notas) {
                    var celdas = '<td>' + mat + '</td>';
                    var vals = [];
                    $.each(parciales, function (i, p) {
                        var nota = notaDeParcial(notas, p, i + 1);
                        if (esNumero(nota)) vals.push(parseFloat(nota));
                        celdas += '<td' + claseNota(nota) + '>' + fmtNota(nota) + '</td>';
                    });

                    // Promedio aritmético simple sobre los parciales con nota
                    var prom = vals.length > 0
                        ? vals.reduce(function (a, b) { return a + b; }, 0) / vals.length
                        : null;
                    celdas += '<td' + claseNota(prom) + '><strong>' + fmtNota(prom) + '</strong></td>';
                    celdas += '<td class="text-center text-muted">' + conceptoDe(prom) + '</td>';

                    tbody.append('<tr>' + celdas + '</tr>');
                });

                $("#panel-boletin").show();
            });
        });
    }

    // ---------- Init ----------
    obj.init = function (config) {
        urls = {
            list:         config.urlList,
            guardar:      config.urlGuardar,
            dropdowns:    config.urlDropdowns,
            periodo:      config.urlPeriodo,
            boletinPublicacion: config.urlBoletinPublicacion,
            boletinGet:   config.urlBoletinGet,
            boletinDrops: config.urlBoletinDrops,
            boletinEscala:config.urlBoletinEscala,
            notaMinima:   config.urlNotaMinima
        };

        initTabs();
        initCuaderno();
        initBoletin();

        // Escala de notas para traducir el promedio a concepto en el boletín (opcional).
        if (urls.boletinEscala) {
            $.getJSON(urls.boletinEscala, function (res) {
                escala = (res && res.data) ? res.data : (res || []);
            });
        }

        // Dropdowns del cuaderno + catálogo real de parciales (compartido con el boletín)
        $.getJSON(urls.dropdowns, function (data) {
            parciales = data.parciales || [];
            horarios  = data.horarios  || [];
            var sems = {}, listaSem = [];
            $.each(horarios, function (_, h) {
                if (!sems[h.sem]) { sems[h.sem] = true; listaSem.push({ id: h.sem, texto: h.semTxt }); }
            });
            llenarSelect($("#f-sem"), listaSem, "-- Seleccione Semestre --");
            if (listaSem.length === 1) { $("#f-sem").val(listaSem[0].id).trigger("change"); }
            llenarSelect($("#f-par"), data.parciales, "-- Seleccione Parcial --");
            if (!horarios.length) { toastr.info("No tiene materias asignadas en el año lectivo activo."); }
            renderCabeceraBoletin();
        });

        // Dropdown de alumnos del boletín
        $.getJSON(urls.boletinDrops, function (data) {
            var sa = $("#f-alu");
            $.each(data.alumnos, function (_, a) {
                sa.append('<option value="' + a.id + '">' + a.texto + '</option>');
            });
        });

        // Nota mínima de aprobación configurable (fallback 70)
        $.getJSON(urls.notaMinima)
            .done(function (res) {
                if (res && esNumero(res.valor)) notaMinima = parseFloat(res.valor);
            })
            .fail(function () { notaMinima = 70; });
    };

    return obj;
}());
