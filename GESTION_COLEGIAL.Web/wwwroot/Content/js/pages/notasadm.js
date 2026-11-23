/* Hub "Notas y Boletín"
   Tab 1: Cuaderno de Notas (captura por Horario + Parcial) — endpoints NotasAdm
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
            sel.append('<option value="' + item.id + '">' + item.texto + '</option>');
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
        $("#btn-cargar").on("click", function () {
            contexto = {
                Hor_Id: $("#f-hor").val(),
                Par_Id: $("#f-par").val()
            };
            if (!contexto.Hor_Id || !contexto.Par_Id) {
                alert("Seleccione el horario y el parcial.");
                return;
            }
            verificarCierre(contexto.Hor_Id, contexto.Par_Id);
            $.getJSON(urls.list, contexto, function (data) {
                var tbody = $("#tbody-notas").empty();
                if (!data || data.length === 0) {
                    tbody.append('<tr><td colspan="3" class="text-center text-muted">Sin alumnos en este horario.</td></tr>');
                    return;
                }
                $.each(data, function (_, row) {
                    var btn   = '<button class="btn btn-sm btn-success btn-guardar"' +
                                ' data-id="'  + (row.Not_Id || 0) +
                                '" data-alu="' + row.Alu_Id +
                                '" data-sec="' + row.Sec_Id +
                                '" data-mat="' + row.Mat_Id +
                                '" data-sem="' + row.Sem_Id + '">Guardar</button>';
                    var valor = esNumero(row.Not_Nota) ? parseFloat(row.Not_Nota).toFixed(2) : "";
                    var input = '<input type="number" class="form-control nota-input" step="0.01" min="0" max="100" value="' + valor + '">';
                    tbody.append("<tr><td>" + row.Alu_NombreCompleto + "</td><td>" + input + "</td><td>" + btn + "</td></tr>");
                });
                aplicarCierre();
            });
        });

        // Guarda la nota de una fila. cb(success) es opcional (usado por Guardar Todo).
        function guardarNota(tr, cb) {
            if (cierre.soloLectura) {
                if (!cb) toastr.warning(cierre.mensaje);
                if (cb) cb(false, cierre.mensaje);
                return;
            }
            var btn  = tr.find(".btn-guardar");
            var nota = tr.find(".nota-input").val();
            $.post(urls.guardar, {
                Not_Id:   btn.data("id"),
                Alu_Id:   btn.data("alu"),
                Sec_Id:   btn.data("sec"),
                Mat_Id:   btn.data("mat"),
                Sem_Id:   btn.data("sem"),
                Par_Id:   contexto.Par_Id,
                Not_Nota: nota,
                Not_Año:  new Date().getFullYear() + "-01-01"
            }, function (res) {
                var ok = !!(res && res.success);
                if (ok) {
                    btn.removeClass("btn-success").addClass("btn-secondary").text("Guardado");
                } else if (res && res.message && !cb) {
                    toastr.error(res.message);
                }
                if (res && res.cerrado) { cierre = { soloLectura: true, mensaje: res.message }; aplicarCierre(); }
                if (cb) cb(ok, res && res.message);
            }).fail(function () {
                if (cb) cb(false);
            });
        }

        $(document).on("click", ".btn-guardar", function () {
            guardarNota($(this).closest("tr"));
        });

        // ---------- Guardar Todo (mismo patrón loop que Pase de Lista en asistenciaadm.js) ----------
        $("#btn-guardar-todo").on("click", function () {
            if (cierre.soloLectura) { toastr.warning(cierre.mensaje); return; }
            var filas = $("#tbody-notas tr").filter(function () {
                return esNumero($(this).find(".nota-input").val());
            });

            if (filas.length === 0) {
                toastr.warning("No hay notas capturadas para guardar.");
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
                    $btn.prop("disabled", false).html(htmlOriginal);
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
            llenarSelect($("#f-hor"), data.horarios,  "-- Seleccione Horario --");
            llenarSelect($("#f-par"), data.parciales, "-- Seleccione Parcial --");
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
