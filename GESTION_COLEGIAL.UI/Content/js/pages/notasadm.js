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
        return (esNumero(v) && parseFloat(v) < notaMinima) ? ' class="text-center nb-nota-reprobada"' : ' class="text-center"';
    }

    // ---------- Tabs ----------
    function initTabs() {
        $("#nb-tabs .nb-tab-item").on("click", function (e) {
            e.preventDefault();
            var tab = $(this).data("tab");
            $("#nb-tabs .nb-tab-item").removeClass("active");
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
            });
        });

        $(document).on("click", ".btn-guardar", function () {
            var btn  = $(this);
            var nota = btn.closest("tr").find(".nota-input").val();
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
                if (res && res.success) {
                    btn.removeClass("btn-success").addClass("btn-secondary").text("Guardado");
                }
            });
        });
    }

    // ---------- Tab 2: Boletín ----------
    function renderCabeceraBoletin() {
        var tr = $("#thead-boletin").empty();
        tr.append('<th>Materia</th>');
        $.each(parciales, function (_, p) {
            tr.append('<th class="text-center">' + p.texto + '</th>');
        });
        tr.append('<th class="text-center">Promedio</th>');
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
                var totalCols = parciales.length + 2; // Materia + parciales + Promedio

                renderCabeceraBoletin();

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
            boletinGet:   config.urlBoletinGet,
            boletinDrops: config.urlBoletinDrops,
            notaMinima:   config.urlNotaMinima
        };

        initTabs();
        initCuaderno();
        initBoletin();

        // Dropdowns del cuaderno + catálogo real de parciales (compartido con el boletín)
        $.getJSON(urls.dropdowns, function (data) {
            parciales = data.parciales || [];
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
