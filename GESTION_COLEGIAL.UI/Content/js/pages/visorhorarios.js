/* Visor de Horarios Academicos
   Renderiza los registros de tbHorarios como cards visuales agrupadas
   por Seccion, Profesor o Aula. Filtros y busqueda en cliente.            */

(function () {
    "use strict";

    var URL_LIST = "/VisorHorarios/ListAsync";

    // Modo docente: solo ve su propio horario. El backend ya filtra las clases
    // por su Emp_Id; aquí solo ajustamos la presentacion (vista fija + mensajes).
    var ES_DOCENTE = !!window.VH_ES_DOCENTE;
    var EMP_ID = window.VH_EMP_ID || 0;

    // Mapeo Mat_Id -> Area academica (para colorear cards).
    var AREA_POR_MATERIA = {
        2: "MAT", 15: "MAT", 16: "MAT", 17: "MAT", 19: "MAT",
        3: "LEN", 22: "LEN", 24: "LEN", 34: "LEN",
        4: "CIE", 6: "CIE", 10: "CIE", 18: "CIE",
        8: "ING", 26: "ING", 36: "ING", 47: "ING",
        14: "CON", 32: "CON", 33: "CON", 35: "CON", 37: "CON", 38: "CON",
        39: "CON", 41: "CON", 42: "CON", 43: "CON", 44: "CON", 45: "CON", 46: "CON",
        1: "SOC", 5: "SOC", 7: "SOC", 11: "SOC", 12: "SOC", 13: "SOC",
        20: "SOC", 23: "SOC", 30: "SOC", 31: "SOC",
        9: "TEC", 25: "TEC", 27: "TEC", 28: "TEC", 29: "TEC",
        21: "ART",
        40: "OTR"
    };

    var DIAS_LABEL = {
        1: "Lunes", 2: "Martes", 3: "Miércoles", 4: "Jueves",
        5: "Viernes", 6: "Sábado", 7: "Domingo"
    };

    // Bloques canonicos por modalidad. Cada fila: ["HH:MM","HH:MM"] = bloque,
    // ["RECESO","HH:MM","HH:MM"] = receso. Reflejan lo definido en
    // horarios_02_generar_horarios.sql.
    var BLOQUES_MODALIDAD = {
        1: [ // Matutina  07:00 - 12:00  receso 09:00-09:40
            ["07:00","07:40"], ["07:40","08:20"], ["08:20","09:00"],
            ["RECESO","09:00","09:40"],
            ["09:40","10:00"], ["10:00","10:40"], ["10:40","11:20"], ["11:20","12:00"]
        ],
        2: [ // Vespertina  01:00 - 06:00  receso 03:00-03:20
            ["01:00","01:40"], ["01:40","02:20"], ["02:20","03:00"],
            ["RECESO","03:00","03:20"],
            ["03:20","04:00"], ["04:00","04:40"], ["04:40","05:20"], ["05:20","06:00"]
        ],
        3: [ // Nocturna  06:00 - 09:00  receso 07:20-08:00
            ["06:00","06:40"], ["06:40","07:20"],
            ["RECESO","07:20","08:00"],
            ["08:00","08:40"], ["08:40","09:00"]
        ]
    };

    var state = {
        all: [],
        filtered: [],
        // El docente ve su horario consolidado en una sola grilla (vista "profesor").
        vista: ES_DOCENTE ? "profesor" : "seccion"
    };

    /* ------------------------------------------------------------ */
    /* Utilidades                                                    */
    /* ------------------------------------------------------------ */

    function areaDe(matId) {
        return AREA_POR_MATERIA[matId] || "OTR";
    }

    function normaliza(s) {
        return (s == null ? "" : String(s)).toLowerCase()
            .normalize("NFD").replace(/[̀-ͯ]/g, "");
    }

    // Limpia CR/LF/espacios al final de strings de hora ("07:00\r\n" -> "07:00")
    function limpiaHora(s) {
        if (s == null) return "";
        return String(s).replace(/[\r\n\s]+$/g, "").replace(/^[\r\n\s]+/g, "");
    }

    function setLoading(show) {
        document.getElementById("vhLoader").style.display = show ? "flex" : "none";
        document.getElementById("vhCards").style.display = show ? "none" : "grid";
    }

    function setVacio(show) {
        document.getElementById("vhVacio").style.display = show ? "flex" : "none";
    }

    function escapeHtml(s) {
        if (s == null) return "";
        return String(s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    /* ------------------------------------------------------------ */
    /* Carga inicial                                                 */
    /* ------------------------------------------------------------ */

    function cargar() {
        setLoading(true);
        $.ajax({
            url: URL_LIST,
            type: "GET",
            dataType: "json"
        }).done(function (resp) {
            var data = (resp && resp.data) ? resp.data : resp;
            if (!Array.isArray(data)) data = [];
            state.all = data;
            inicializarFiltros();
            aplicarFiltros();
        }).fail(function () {
            state.all = [];
            inicializarFiltros();
            aplicarFiltros();
        }).always(function () {
            setLoading(false);
        });
    }

    function inicializarFiltros() {
        var anios = unicos(state.all.map(function (h) { return h.Hor_Año; }));
        var semestres = unicos(state.all.map(function (h) { return h.Sem_Id; }));
        var modalidades = unicosObj(state.all
            .filter(function (h) { return h.Mda_Id; })
            .map(function (h) { return { id: h.Mda_Id, txt: h.Mda_Descripcion }; }));

        llenarSelect($("#filtroAnio"), anios.map(function (a) { return { id: a, txt: a }; }), null);
        llenarSelect($("#filtroSemestre"), semestres.map(function (s) { return { id: s, txt: "Semestre " + s }; }), null);
        llenarSelect($("#filtroModalidad"), modalidades, "Todas");
    }

    function unicos(arr) {
        var s = {};
        arr.forEach(function (x) { if (x != null) s[x] = true; });
        return Object.keys(s).map(function (k) { return isNaN(k) ? k : Number(k); }).sort();
    }
    function unicosObj(arr) {
        var s = {};
        arr.forEach(function (x) { if (x && x.id != null) s[x.id] = x.txt; });
        return Object.keys(s).map(function (k) { return { id: Number(k), txt: s[k] }; });
    }

    function llenarSelect($sel, items, placeholder) {
        $sel.empty();
        if (placeholder !== null && placeholder !== undefined) {
            $sel.append('<option value="">' + placeholder + '</option>');
        }
        items.forEach(function (it) {
            $sel.append('<option value="' + it.id + '">' + escapeHtml(it.txt) + '</option>');
        });
    }

    /* ------------------------------------------------------------ */
    /* Filtrado                                                      */
    /* ------------------------------------------------------------ */

    function aplicarFiltros() {
        var anio = $("#filtroAnio").val();
        var sem = $("#filtroSemestre").val();
        var mda = $("#filtroModalidad").val();
        var q = normaliza($("#filtroBusqueda").val());

        state.filtered = state.all.filter(function (h) {
            if (anio && String(h.Hor_Año) !== String(anio)) return false;
            if (sem && String(h.Sem_Id) !== String(sem)) return false;
            if (mda && String(h.Mda_Id) !== String(mda)) return false;
            if (q) {
                var hay = normaliza(h.Mat_Nombre) + " " +
                    normaliza(h.Emp_NombreCompleto) + " " +
                    normaliza(h.Sec_Descripcion) + " " +
                    normaliza(h.Cur_Nombre) + " " +
                    normaliza(h.Cun_Descripcion) + " " +
                    normaliza(h.Aul_Descripcion) + " " +
                    normaliza(h.Dia_Descripcion);
                if (hay.indexOf(q) === -1) return false;
            }
            return true;
        });

        render();
    }

    /* ------------------------------------------------------------ */
    /* Render                                                        */
    /* ------------------------------------------------------------ */

    function render() {
        actualizarKpis();
        actualizarContador();
        var $cont = $("#vhCards").empty();
        if (state.filtered.length === 0) {
            // Docente sin empleado asociado: el backend no pudo resolver su Emp_Id.
            if (ES_DOCENTE && EMP_ID <= 0) {
                $("#vhVacio p").text("No se encontró un docente asociado a tu usuario.");
            } else if (ES_DOCENTE) {
                $("#vhVacio p").text("No tienes clases programadas en tu horario.");
            }
            setVacio(true);
            return;
        }
        setVacio(false);

        var grupos = agrupar(state.filtered, state.vista);
        grupos.forEach(function (g) {
            $cont.append(renderCard(g));
        });
    }

    function actualizarKpis() {
        $("#kpiClases").text(state.filtered.length);
        $("#kpiSecciones").text(contarUnicos(state.filtered, function (h) {
            return h.Cun_Id + "-" + h.Cur_Id + "-" + h.Sec_Id + "-" + (h.Mda_Id || 0);
        }));
        $("#kpiProfesores").text(contarUnicos(state.filtered, function (h) { return h.Emp_Id; }));
        $("#kpiAulas").text(contarUnicos(state.filtered, function (h) { return h.Aul_Id; }));
    }

    function contarUnicos(arr, sel) {
        var s = {};
        arr.forEach(function (x) { s[sel(x)] = true; });
        return Object.keys(s).length;
    }

    function actualizarContador() {
        var grupos = agrupar(state.filtered, state.vista);
        var label;
        switch (state.vista) {
            case "profesor": label = grupos.length === 1 ? "profesor" : "profesores"; break;
            case "aula": label = grupos.length === 1 ? "aula" : "aulas"; break;
            default: label = grupos.length === 1 ? "sección" : "secciones";
        }
        $("#vhContador").text(grupos.length + " " + label + "  •  " + state.filtered.length + " clases");
    }

    function agrupar(rows, vista) {
        var keyFn, titFn, subFn;
        if (vista === "profesor") {
            keyFn = function (h) { return h.Emp_Id; };
            titFn = function (h) { return h.Emp_NombreCompleto || ("Profesor " + h.Emp_Id); };
            subFn = function (g) {
                var matSet = {};
                g.items.forEach(function (i) { matSet[i.Mat_Id] = true; });
                return Object.keys(matSet).length + " materia(s) · " + g.items.length + " clases";
            };
        } else if (vista === "aula") {
            keyFn = function (h) { return h.Aul_Id; };
            titFn = function (h) { return "Aula " + (h.Aul_Descripcion || h.Aul_Id); };
            subFn = function (g) {
                var secSet = {};
                g.items.forEach(function (i) { secSet[i.Cun_Id + "-" + i.Cur_Id + "-" + i.Sec_Id + "-" + (i.Mda_Id || 0)] = true; });
                return Object.keys(secSet).length + " seccion(es) · " + g.items.length + " clases";
            };
        } else {
            // Una "seccion academica" real = (Cun, Cur, Sec, Mda). Cada turno es
            // un grupo escolar distinto aunque comparta nombre con otros.
            keyFn = function (h) { return h.Cun_Id + "-" + h.Cur_Id + "-" + h.Sec_Id + "-" + (h.Mda_Id || 0); };
            titFn = function (h) { return (h.Cur_Nombre || "") + " · Sec. " + (h.Sec_Descripcion || ""); };
            subFn = function (g) {
                var f = g.items[0];
                var partes = [];
                if (f.Cun_Descripcion) partes.push(f.Cun_Descripcion);
                if (f.Mda_Descripcion) partes.push(f.Mda_Descripcion);
                partes.push(g.items.length + " clases");
                return partes.join(" · ");
            };
        }

        var map = {};
        rows.forEach(function (h) {
            var k = keyFn(h);
            if (!map[k]) map[k] = { key: k, titulo: titFn(h), items: [], first: h };
            map[k].items.push(h);
        });
        var arr = Object.keys(map).map(function (k) {
            var g = map[k];
            g.subtitulo = subFn(g);
            return g;
        });

        // ordenar grupos por titulo asc
        arr.sort(function (a, b) {
            return normaliza(a.titulo).localeCompare(normaliza(b.titulo));
        });
        return arr;
    }

    var ICON_MAP = {
        seccion: '<i class="bi bi-people-fill"></i>',
        profesor: '<i class="bi bi-person-badge"></i>',
        aula: '<i class="bi bi-door-open"></i>'
    };

    /* Render card */
    function renderCard(grupo) {
        var $card = $('<div class="vh-card"></div>');
        $card.attr("data-key", grupo.key);

        var firstMda = (grupo.items[0] && grupo.items[0].Mda_Descripcion)
            ? grupo.items[0].Mda_Descripcion : "";

        var $header = $(
            '<div class="vh-card-header">' +
            '  <div class="vh-card-icon">' + ICON_MAP[state.vista] + '</div>' +
            '  <div class="vh-card-title-wrap">' +
            '    <div class="vh-card-title">' + escapeHtml(grupo.titulo) + '</div>' +
            '    <div class="vh-card-subtitle">' + escapeHtml(grupo.subtitulo) + '</div>' +
            '  </div>' +
            (firstMda ? '  <span class="vh-badge">' + escapeHtml(firstMda) + '</span>' : '') +
            '  <div class="vh-card-expand" title="Ver horario completo"><i class="bi bi-arrows-fullscreen"></i></div>' +
            '</div>'
        );
        $card.append($header);

        // Mini-grid semanal
        $card.append(renderMiniGrid(grupo, false));

        // Click en cualquier parte de la card (excepto celdas) abre el modal grande
        $card.on("click", function (e) {
            if ($(e.target).closest(".vh-mini-clase").length > 0) return;
            abrirCardModal(grupo);
        });

        return $card;
    }

    function renderMiniGrid(grupo, expanded) {
        // dias presentes en este grupo (siempre L-V por defecto)
        var diasSet = {};
        var modalidadesSet = {};
        grupo.items.forEach(function (it) {
            diasSet[it.Dia_Id] = true;
            if (it.Mda_Id) modalidadesSet[it.Mda_Id] = true;
        });

        var diasArr = Object.keys(diasSet).map(Number).sort(function (a, b) { return a - b; });
        if (diasArr.length === 0) diasArr = [1, 2, 3, 4, 5];

        // Construir lista de filas canonicas en base a las modalidades presentes.
        // Cada item: { type: "bloque"|"receso", h1, h2 }
        var filas = construirFilasCanonicas(modalidadesSet);

        // Mapa de clases por (dia, horaIni|horaFin) — normalizamos las strings
        // de hora porque tbHoras heredaba CR/LF en algunos catalogos.
        var mapa = {};
        grupo.items.forEach(function (it) {
            var hi = limpiaHora(it.Hor_HoraInicioDescripcion);
            var hf = limpiaHora(it.Hor_HoraFinalizaDescripcion);
            var k = it.Dia_Id + "_" + hi + "|" + hf;
            if (!mapa[k]) mapa[k] = [];
            mapa[k].push(it);
        });

        var maxLen = expanded ? 40 : 22;
        var $tbl = $('<table class="vh-mini' + (expanded ? ' vh-mini--expanded' : '') + '"></table>');
        var $thead = $("<thead></thead>");
        var $trHead = $("<tr></tr>");
        $trHead.append('<th class="vh-mini-hora-col">Hora</th>');
        diasArr.forEach(function (d) {
            $trHead.append('<th>' + escapeHtml(DIAS_LABEL[d] || ("Día " + d)) + '</th>');
        });
        $thead.append($trHead);
        $tbl.append($thead);

        var $tbody = $("<tbody></tbody>");
        var colSpan = diasArr.length + 1;

        filas.forEach(function (fila) {
            if (fila.type === "receso") {
                var $trRec = $('<tr class="vh-receso-row"></tr>');
                $trRec.append(
                    '<td colspan="' + colSpan + '">' +
                    '<i class="bi bi-cup-hot"></i>RECESO  ' +
                    escapeHtml(fila.h1) + ' – ' + escapeHtml(fila.h2) +
                    '</td>'
                );
                $tbody.append($trRec);
                return;
            }

            var h = fila.h1 + "|" + fila.h2;
            var $tr = $("<tr></tr>");
            $tr.append('<td class="vh-mini-hora-cel"><span>' + escapeHtml(fila.h1) + '</span><small>' + escapeHtml(fila.h2) + '</small></td>');
            diasArr.forEach(function (d) {
                var k = d + "_" + h;
                var items = mapa[k];
                if (!items || items.length === 0) {
                    $tr.append('<td class="vh-mini-vacio"></td>');
                } else {
                    var it = items[0];
                    var area = areaDe(it.Mat_Id);
                    var contenido;
                    if (state.vista === "seccion") {
                        contenido = '<div class="vh-clase-mat">' + escapeHtml(truncar(it.Mat_Nombre, maxLen)) + '</div>'
                                  + '<div class="vh-clase-sec">' + escapeHtml(truncar(it.Emp_NombreCompleto, maxLen + 6)) + '</div>'
                                  + '<div class="vh-clase-aula">Aula ' + escapeHtml(it.Aul_Descripcion) + '</div>';
                    } else if (state.vista === "profesor") {
                        contenido = '<div class="vh-clase-mat">' + escapeHtml(truncar(it.Mat_Nombre, maxLen)) + '</div>'
                                  + '<div class="vh-clase-sec">' + escapeHtml(truncar(it.Cur_Nombre + " " + (it.Sec_Descripcion || ""), maxLen + 6)) + '</div>'
                                  + '<div class="vh-clase-aula">Aula ' + escapeHtml(it.Aul_Descripcion) + '</div>';
                    } else {
                        contenido = '<div class="vh-clase-mat">' + escapeHtml(truncar(it.Mat_Nombre, maxLen)) + '</div>'
                                  + '<div class="vh-clase-sec">' + escapeHtml(truncar(it.Cur_Nombre + " " + (it.Sec_Descripcion || ""), maxLen + 6)) + '</div>'
                                  + '<div class="vh-clase-aula">' + escapeHtml(truncar(it.Emp_NombreCompleto, maxLen + 6)) + '</div>';
                    }
                    var extra = items.length > 1 ? '<span class="vh-multi">+' + (items.length - 1) + '</span>' : '';
                    var $cel = $('<td class="vh-mini-clase vh-area-' + area + '" title="' + escapeHtml(it.Mat_Nombre + ' — ' + (it.Emp_NombreCompleto || '')) + '">' + contenido + extra + '</td>');
                    // Closure captura dia/hora/items para el handler de click
                    (function (cgrupo, cdia, ch, citems) {
                        $cel.on("click", function (e) {
                            e.stopPropagation();
                            if (expanded) {
                                mostrarDetalleEnPanel(cgrupo, cdia, ch, citems);
                            } else {
                                abrirCardModal(cgrupo, cdia, ch, citems);
                            }
                        });
                    })(grupo, d, h, items);
                    $tr.append($cel);
                }
            });
            $tbody.append($tr);
        });
        $tbl.append($tbody);

        if (grupo.items.length === 0) {
            return $('<div class="vh-card-sin-data">Sin clases programadas</div>');
        }
        return $tbl;
    }

    // Construye la lista de filas (bloques + recesos) en base a las modalidades
    // presentes en el grupo. Si hay solo 1 modalidad: usa sus bloques canonicos.
    // Si hay varias: las concatena en orden 1 (Matutina) -> 2 (Vespertina) -> 3 (Nocturna).
    function construirFilasCanonicas(modalidadesSet) {
        var modalidades = Object.keys(modalidadesSet).map(Number).sort();
        if (modalidades.length === 0) modalidades = [1, 2, 3];

        var filas = [];
        modalidades.forEach(function (mda) {
            var def = BLOQUES_MODALIDAD[mda];
            if (!def) return;
            def.forEach(function (b) {
                if (b[0] === "RECESO") {
                    filas.push({ type: "receso", h1: b[1], h2: b[2] });
                } else {
                    filas.push({ type: "bloque", h1: b[0], h2: b[1] });
                }
            });
        });
        return filas;
    }

    function truncar(s, n) {
        if (!s) return "";
        return s.length > n ? s.substring(0, n - 1) + "…" : s;
    }

    /* Modal grande con horario completo de la sección/profesor/aula
       Si recibe (dia, hora, items) opcionales, abre con esa clase pre-seleccionada. */
    function abrirCardModal(grupo, diaInicial, horaInicial, itemsIniciales) {
        $("#vhModalIcon").html(ICON_MAP[state.vista]);
        $("#vhModalTitulo").text(grupo.titulo);
        $("#vhModalSubtitulo").text(grupo.subtitulo);

        // Layout: grid grande a la izquierda, panel detalle a la derecha
        var $body = $("#vhModalBody").empty();
        var $grid = $('<div class="vh-modal-grid-col"></div>');
        $grid.append(renderMiniGrid(grupo, true));

        var $panel = $('<div class="vh-modal-panel-col"></div>');
        $panel.append('<div class="vh-modal-panel-title"><i class="bi bi-info-circle"></i> Detalle</div>');
        $panel.append('<div class="vh-modal-panel-body" id="vhModalPanelBody">' +
            '<div class="vh-modal-empty"><i class="bi bi-hand-index-thumb"></i><p>Selecciona una clase para ver el detalle completo</p></div>' +
            '</div>');

        var $layout = $('<div class="vh-modal-layout"></div>').append($grid).append($panel);
        $body.append($layout);

        $("#vhModalDetalle").modal("show");

        // Si vino con clase preseleccionada (click directo en celda de card pequeña),
        // poblar el panel inmediatamente.
        if (diaInicial && horaInicial && itemsIniciales) {
            setTimeout(function () {
                mostrarDetalleEnPanel(grupo, diaInicial, horaInicial, itemsIniciales);
            }, 50);
        }
    }

    /* Detalle de una clase específica en el panel lateral del modal */
    function mostrarDetalleEnPanel(grupo, dia, horaKey, items) {
        var partes = horaKey.split("|");
        var cabecera = '<div class="vh-modal-panel-hint">' +
            '<i class="bi bi-calendar-event"></i> ' +
            escapeHtml(DIAS_LABEL[dia] || ("Día " + dia)) +
            ' · ' + escapeHtml(partes[0] || "") + ' – ' + escapeHtml(partes[1] || "") +
            '</div>';

        var html = items.map(function (it) {
            var area = areaDe(it.Mat_Id);
            return '<div class="vh-modal-clase vh-area-' + area + '">' +
                '<div class="vh-modal-mat">' + escapeHtml(it.Mat_Nombre) + '</div>' +
                '<div class="vh-modal-row"><i class="bi bi-person-badge"></i> <span>' + escapeHtml(it.Emp_NombreCompleto || "") + '</span></div>' +
                '<div class="vh-modal-row"><i class="bi bi-people-fill"></i> <span>' + escapeHtml(it.Cur_Nombre + " · Sec " + (it.Sec_Descripcion || "")) + '</span></div>' +
                '<div class="vh-modal-row"><i class="bi bi-door-open"></i> <span>Aula ' + escapeHtml(it.Aul_Descripcion) + '</span></div>' +
                '<div class="vh-modal-row"><i class="bi bi-mortarboard"></i> <span>' + escapeHtml(it.Cun_Descripcion || "") + '</span></div>' +
                (it.Mda_Descripcion ? '<div class="vh-modal-row"><i class="bi bi-sun"></i> <span>' + escapeHtml(it.Mda_Descripcion) + '</span></div>' : '') +
                '</div>';
        }).join("");

        $("#vhModalPanelBody").html(cabecera + html);
    }

    /* ------------------------------------------------------------ */
    /* Eventos                                                       */
    /* ------------------------------------------------------------ */

    function bindEventos() {
        $(".vh-toggle-btn").on("click", function () {
            $(".vh-toggle-btn").removeClass("is-active");
            $(this).addClass("is-active");
            state.vista = $(this).data("vista");
            render();
        });

        $("#filtroAnio, #filtroSemestre, #filtroModalidad").on("change", aplicarFiltros);

        var debounce;
        $("#filtroBusqueda").on("input", function () {
            clearTimeout(debounce);
            debounce = setTimeout(aplicarFiltros, 180);
        });

        $("#btnLimpiar").on("click", function () {
            $("#filtroModalidad").val("");
            $("#filtroBusqueda").val("");
            aplicarFiltros();
        });
    }

    /* ------------------------------------------------------------ */

    $(function () {
        bindEventos();
        cargar();
    });

})();
