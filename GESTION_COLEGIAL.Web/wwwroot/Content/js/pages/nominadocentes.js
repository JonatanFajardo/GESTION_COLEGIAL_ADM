/* Nomina de Docentes - logica de cards y modal */

(function () {
    "use strict";

    var URL_LIST = "/NominaDocentes/ListAsync";
    var URL_DETALLE = "/NominaDocentes/DetalleAsync";
    var URL_PLANILLA = "/NominaDocentes/PlanillaAsync";

    var DIAS_LABEL = {
        1: "Lunes", 2: "Martes", 3: "Miércoles", 4: "Jueves",
        5: "Viernes", 6: "Sábado", 7: "Domingo"
    };

    var AREA_LABEL = {
        MAT: "Matemáticas", LEN: "Lenguaje", CIE: "Ciencias", ING: "Inglés",
        SOC: "Sociales", CON: "Contabilidad", TEC: "Tecnología",
        ART: "Arte/EF", OTR: "Otros"
    };

    var AREA_BADGE = {
        MAT: "primary", LEN: "info", CIE: "success", ING: "secondary",
        SOC: "warning", CON: "dark", TEC: "info", ART: "danger", OTR: "secondary"
    };

    function badgeArea(area) {
        return '<span class="badge outline-badge-' + (AREA_BADGE[area] || "secondary") + '">' + escapeHtml(AREA_LABEL[area] || area) + '</span>';
    }

    var state = { all: [], filtered: [] };

    function fmtL(n) {
        if (n == null || isNaN(n)) return "L 0.00";
        return "L " + Number(n).toLocaleString("es-HN", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function escapeHtml(s) {
        if (s == null) return "";
        return String(s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function normaliza(s) {
        return (s == null ? "" : String(s)).toLowerCase()
            .normalize("NFD").replace(/[̀-ͯ]/g, "");
    }

    function setLoading(show) {
        document.getElementById("ndLoader").style.display = show ? "" : "none";
        document.getElementById("ndCards").style.display = show ? "none" : "";
    }

    function setVacio(show) {
        document.getElementById("ndVacio").style.display = show ? "" : "none";
    }

    function inicialesDe(nombre) {
        if (!nombre) return "?";
        var partes = nombre.trim().split(/\s+/);
        if (partes.length === 1) return partes[0].substring(0, 2).toUpperCase();
        return (partes[0][0] + partes[partes.length - 1][0]).toUpperCase();
    }

    /* ------------------------------------------------------------ */
    /* Carga                                                         */
    /* ------------------------------------------------------------ */
    function cargar() {
        setLoading(true);
        var anio = $("#filtroAnio").val();
        var sem = $("#filtroSemestre").val();
        $.ajax({
            url: URL_LIST,
            type: "GET",
            data: { anio: anio, semestre: sem },
            dataType: "json"
        }).done(function (resp) {
            var data = (resp && resp.data) ? resp.data : resp;
            if (!Array.isArray(data)) data = [];
            state.all = data;
            aplicarFiltros();
        }).fail(function () {
            state.all = [];
            aplicarFiltros();
        }).always(function () {
            setLoading(false);
        });
    }

    /* ------------------------------------------------------------ */
    /* Filtros                                                       */
    /* ------------------------------------------------------------ */
    function aplicarFiltros() {
        var q = normaliza($("#filtroBusqueda").val());
        var orden = $("#filtroOrden").val();

        state.filtered = state.all.filter(function (n) {
            if (!q) return true;
            var hay = normaliza(n.NombreCompleto) + " " +
                normaliza(n.Emp_Codigo) + " " +
                normaliza(n.TituloDescripcion) + " " +
                normaliza(n.AreaPrincipal) + " " +
                normaliza(n.ModalidadPrincipal);
            return hay.indexOf(q) !== -1;
        });

        switch (orden) {
            case "neto-asc":  state.filtered.sort(function (a, b) { return a.NetoAPagar - b.NetoAPagar; }); break;
            case "nombre":    state.filtered.sort(function (a, b) { return normaliza(a.NombreCompleto).localeCompare(normaliza(b.NombreCompleto)); }); break;
            case "periodos":  state.filtered.sort(function (a, b) { return b.PeriodosMensuales - a.PeriodosMensuales; }); break;
            default:          state.filtered.sort(function (a, b) { return b.NetoAPagar - a.NetoAPagar; });
        }

        render();
    }

    /* ------------------------------------------------------------ */
    /* Render                                                        */
    /* ------------------------------------------------------------ */
    function render() {
        actualizarKpis();
        actualizarContador();

        var $cont = $("#ndCards").empty();
        if (state.filtered.length === 0) { setVacio(true); return; }
        setVacio(false);

        state.filtered.forEach(function (n) { $cont.append(renderCard(n)); });
    }

    function actualizarKpis() {
        var totalNeto = state.filtered.reduce(function (a, b) { return a + (b.NetoAPagar || 0); }, 0);
        var totalDed = state.filtered.reduce(function (a, b) { return a + (b.TotalDeducciones || 0); }, 0);
        var totalPer = state.filtered.reduce(function (a, b) { return a + (b.PeriodosMensuales || 0); }, 0);
        var totalExt = state.filtered.reduce(function (a, b) { return a + (b.PeriodosExtraMensuales || 0); }, 0);

        $("#kpiTotal").text(fmtL(totalNeto));
        $("#kpiDocentes").text(state.filtered.length);
        $("#kpiDocentesSub").text(totalExt > 0 ? totalExt + " períodos extra" : "sin horas extra");
        $("#kpiPeriodos").text(totalPer.toLocaleString("es-HN"));
        $("#kpiPeriodosSub").text((totalPer / 4).toFixed(0) + " periodos/semana");
        $("#kpiDeducciones").text(fmtL(totalDed));
    }

    function actualizarContador() {
        var n = state.filtered.length;
        $("#ndContador").text(n + " " + (n === 1 ? "docente" : "docentes"));
    }

    function renderCard(n) {
        var area = n.AreaPrincipal || "OTR";
        var iniciales = inicialesDe(n.NombreCompleto);
        var mda = n.ModalidadPrincipal ? '<span class="badge outline-badge-info ml-1">' + escapeHtml(n.ModalidadPrincipal) + '</span>' : '';

        var horasExtra = n.PeriodosExtraMensuales > 0
            ? '<div class="mb-3"><span class="badge badge-warning"><i class="bi bi-lightning-fill mr-1"></i>' + n.PeriodosExtraMensuales + ' extra</span></div>'
            : '';

        var html =
            '<div class="col-xl-4 col-md-6 col-12 layout-spacing">' +
            '<div class="statbox widget box box-shadow h-100" role="button" data-emp="' + n.Emp_Id + '">' +
              '<div class="widget-content widget-content-area h-100 d-flex flex-column">' +
                '<div class="d-flex align-items-center mb-3">' +
                  '<span class="badge badge-primary mr-3">' + iniciales + '</span>' +
                  '<div>' +
                    '<h6 class="font-weight-bold mb-1">' + escapeHtml(n.NombreCompleto) + '</h6>' +
                    '<div>' + badgeArea(area) + mda + '</div>' +
                  '</div>' +
                '</div>' +

                '<p class="text-muted small mb-0">Neto a pagar</p>' +
                '<h4 class="font-weight-bold text-success mb-3">' + fmtL(n.NetoAPagar) + '</h4>' +

                '<div class="row text-center mb-3">' +
                  '<div class="col-4"><h6 class="font-weight-bold mb-0">' + n.PeriodosMensuales + '</h6><small class="text-muted">períodos/mes</small></div>' +
                  '<div class="col-4"><h6 class="font-weight-bold mb-0">' + n.MateriasDistintas + '</h6><small class="text-muted">materias</small></div>' +
                  '<div class="col-4"><h6 class="font-weight-bold mb-0">' + n.SeccionesAtendidas + '</h6><small class="text-muted">secciones</small></div>' +
                '</div>' +

                horasExtra +

                '<div class="small mb-3">' +
                  '<div class="d-flex justify-content-between"><span class="text-muted">Sueldo bruto</span><b>' + fmtL(n.SueldoBruto) + '</b></div>' +
                  (n.HorasExtraMonto > 0 ? '<div class="d-flex justify-content-between"><span class="text-muted">+ Horas extra</span><b class="text-success">' + fmtL(n.HorasExtraMonto) + '</b></div>' : '') +
                  '<div class="d-flex justify-content-between"><span class="text-muted">− Deducciones</span><b class="text-danger">' + fmtL(n.TotalDeducciones) + '</b></div>' +
                '</div>' +

                '<button type="button" class="btn btn-sm btn-outline-primary btn-block mt-auto">' +
                  '<i class="bi bi-arrows-fullscreen mr-1"></i> Ver desglose completo' +
                '</button>' +
              '</div>' +
            '</div>' +
            '</div>';

        var $c = $(html);
        $c.on("click", function () { abrirDetalle(n.Emp_Id); });
        return $c;
    }

    /* ------------------------------------------------------------ */
    /* Modal detalle                                                 */
    /* ------------------------------------------------------------ */
    function abrirDetalle(empId) {
        $("#ndModalBody").html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>');
        $("#ndModal").modal("show");

        var anio = $("#filtroAnio").val();
        var sem = $("#filtroSemestre").val();
        $.ajax({
            url: URL_DETALLE,
            type: "GET",
            data: { empId: empId, anio: anio, semestre: sem },
            dataType: "json"
        }).done(function (resp) {
            var data = (resp && resp.data) ? resp.data : resp;
            renderDetalle(data);
        }).fail(function () {
            $("#ndModalBody").html('<div class="alert alert-danger">No se pudo cargar el detalle.</div>');
        });
    }

    function renderDetalle(d) {
        if (!d || !d.Resumen) {
            $("#ndModalBody").html('<div class="alert alert-warning">Sin datos.</div>');
            return;
        }
        var r = d.Resumen;
        $("#ndModalNombre").text(r.NombreCompleto);
        var sub = [r.Emp_Codigo, r.TituloDescripcion, r.ModalidadPrincipal].filter(Boolean).join(" · ");
        $("#ndModalSubtitulo").text(sub);

        // Tabla de líneas
        var rowsLineas = (d.Lineas || []).map(function (l) {
            return '<tr>' +
                '<td>' + badgeArea(l.Area) + '</td>' +
                '<td>' + escapeHtml(l.Mat_Nombre) + '</td>' +
                '<td>' + escapeHtml(l.Mda_Descripcion || "—") + '</td>' +
                '<td class="text-right">' + l.PeriodosSemanales + '</td>' +
                '<td class="text-right">' + l.PeriodosMensuales + '</td>' +
                '<td class="text-right">' + fmtL(l.TarifaPeriodo) + '</td>' +
                '<td class="text-right"><b>' + fmtL(l.Subtotal) + '</b></td>' +
            '</tr>';
        }).join("");

        // Tabla de carga por día
        var rowsDias = (d.CargaPorDia || []).map(function (c) {
            var alerta = c.PeriodosExtra > 0
                ? '<span class="badge badge-warning">+' + c.PeriodosExtra + ' extra</span>'
                : '';
            return '<tr>' +
                '<td>' + escapeHtml(DIAS_LABEL[c.Dia_Id] || c.Dia_Descripcion) + '</td>' +
                '<td class="text-right">' + c.Periodos + '</td>' +
                '<td>' + alerta + '</td>' +
            '</tr>';
        }).join("");

        var html =
            '<div class="row">' +

              '<div class="col-lg-8 mb-3 mb-lg-0">' +
                '<h6 class="font-weight-bold mb-2"><i class="bi bi-list-ul mr-1"></i> Materias asignadas</h6>' +
                '<div class="table-responsive"><table class="table table-sm table-hover">' +
                  '<thead><tr><th>Área</th><th>Materia</th><th>Modalidad</th><th class="text-right">Sem</th><th class="text-right">Mes</th><th class="text-right">Tarifa</th><th class="text-right">Subtotal</th></tr></thead>' +
                  '<tbody>' + rowsLineas + '</tbody>' +
                '</table></div>' +

                '<h6 class="font-weight-bold mt-4 mb-2"><i class="bi bi-calendar3 mr-1"></i> Carga por día (semanal)</h6>' +
                '<div class="table-responsive"><table class="table table-sm table-hover">' +
                  '<thead><tr><th>Día</th><th class="text-right">Períodos</th><th>Estado</th></tr></thead>' +
                  '<tbody>' + rowsDias + '</tbody>' +
                '</table></div>' +
              '</div>' +

              '<div class="col-lg-4">' +
                '<div class="card component-card_1"><div class="card-body">' +
                  '<div class="d-flex justify-content-between py-1"><span class="text-muted">Sueldo bruto regular</span><b>' + fmtL(r.SueldoBruto) + '</b></div>' +
                  (r.HorasExtraMonto > 0 ? '<div class="d-flex justify-content-between py-1"><span class="text-muted">Horas extra (' + r.PeriodosExtraMensuales + ' p)</span><b class="text-success">+ ' + fmtL(r.HorasExtraMonto) + '</b></div>' : '') +
                  '<hr class="my-2">' +
                  '<div class="d-flex justify-content-between py-1 font-weight-bold"><span>Total bruto</span><b>' + fmtL(r.TotalBruto) + '</b></div>' +
                  '<div class="d-flex justify-content-between py-1"><span class="text-muted">IHSS (5%)</span><b class="text-danger">− ' + fmtL(r.DeduccionIHSS) + '</b></div>' +
                  '<div class="d-flex justify-content-between py-1"><span class="text-muted">ISR escalonado</span><b class="text-danger">− ' + fmtL(r.DeduccionISR) + '</b></div>' +
                  '<div class="d-flex justify-content-between py-1"><span class="text-muted">Colegio profesional</span><b class="text-danger">− ' + fmtL(r.DeduccionColegio) + '</b></div>' +
                  '<div class="d-flex justify-content-between py-1"><span class="text-muted">Total deducciones</span><b class="text-danger">− ' + fmtL(r.TotalDeducciones) + '</b></div>' +
                  '<hr class="my-2">' +
                  '<p class="text-muted small text-uppercase mb-0">Neto a pagar</p>' +
                  '<h4 class="font-weight-bold text-success mb-0">' + fmtL(r.NetoAPagar) + '</h4>' +
                '</div></div>' +
              '</div>' +

            '</div>';

        $("#ndModalBody").html(html);
    }

    /* ------------------------------------------------------------ */
    /* Exportación CSV (cliente, sin librerías externas)             */
    /* ------------------------------------------------------------ */
    function csvCampo(valor) {
        // Envuelve en comillas y escapa comillas internas. Prefijo BOM aparte.
        var s = (valor == null) ? "" : String(valor);
        s = s.replace(/"/g, '""');
        return '"' + s + '"';
    }

    function numeroCsv(n) {
        // Número plano con 2 decimales, punto decimal (compatible con Excel/Sheets).
        if (n == null || isNaN(n)) return "0.00";
        return Number(n).toFixed(2);
    }

    function nombrePeriodo() {
        var anio = $("#filtroAnio").val();
        var sem = $("#filtroSemestre").val();
        return anio + "-S" + sem;
    }

    function exportarCsv() {
        if (!state.filtered || state.filtered.length === 0) {
            if (window.alert) window.alert("No hay docentes para exportar en este período.");
            return;
        }

        var encabezados = [
            "Codigo", "Docente", "Titulo", "Area", "Modalidad",
            "Materias", "Secciones", "Periodos/mes", "Extra/mes",
            "Sueldo bruto", "Horas extra", "Total bruto",
            "IHSS", "ISR", "Colegio profesional", "Total deducciones", "Neto a pagar"
        ];

        var lineas = [encabezados.map(csvCampo).join(",")];

        state.filtered.forEach(function (n) {
            var fila = [
                csvCampo(n.Emp_Codigo),
                csvCampo(n.NombreCompleto),
                csvCampo(n.TituloDescripcion),
                csvCampo(AREA_LABEL[n.AreaPrincipal] || n.AreaPrincipal || ""),
                csvCampo(n.ModalidadPrincipal),
                csvCampo(n.MateriasDistintas),
                csvCampo(n.SeccionesAtendidas),
                csvCampo(n.PeriodosMensuales),
                csvCampo(n.PeriodosExtraMensuales),
                numeroCsv(n.SueldoBruto),
                numeroCsv(n.HorasExtraMonto),
                numeroCsv(n.TotalBruto),
                numeroCsv(n.DeduccionIHSS),
                numeroCsv(n.DeduccionISR),
                numeroCsv(n.DeduccionColegio),
                numeroCsv(n.TotalDeducciones),
                numeroCsv(n.NetoAPagar)
            ];
            lineas.push(fila.join(","));
        });

        // Fila de totales
        var t = state.filtered.reduce(function (acc, n) {
            acc.bruto += n.SueldoBruto || 0;
            acc.extra += n.HorasExtraMonto || 0;
            acc.totalBruto += n.TotalBruto || 0;
            acc.ihss += n.DeduccionIHSS || 0;
            acc.isr += n.DeduccionISR || 0;
            acc.colegio += n.DeduccionColegio || 0;
            acc.ded += n.TotalDeducciones || 0;
            acc.neto += n.NetoAPagar || 0;
            return acc;
        }, { bruto: 0, extra: 0, totalBruto: 0, ihss: 0, isr: 0, colegio: 0, ded: 0, neto: 0 });

        var totales = [
            csvCampo("TOTALES (" + state.filtered.length + " docentes)"),
            "", "", "", "", "", "", "", "",
            numeroCsv(t.bruto),
            numeroCsv(t.extra),
            numeroCsv(t.totalBruto),
            numeroCsv(t.ihss),
            numeroCsv(t.isr),
            numeroCsv(t.colegio),
            numeroCsv(t.ded),
            numeroCsv(t.neto)
        ];
        lineas.push(totales.join(","));

        // BOM UTF-8 (﻿) para que Excel respete acentos (á, é, í, ñ...).
        var contenido = "﻿" + lineas.join("\r\n");
        var blob = new Blob([contenido], { type: "text/csv;charset=utf-8;" });
        var nombre = "nomina-docentes-" + nombrePeriodo() + ".csv";

        var url = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = url;
        a.download = nombre;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        setTimeout(function () { URL.revokeObjectURL(url); }, 100);
    }

    /* ------------------------------------------------------------ */
    /* Impresión / PDF — abre la vista imprimible en pestaña nueva   */
    /* ------------------------------------------------------------ */
    function imprimirPlanilla() {
        var anio = $("#filtroAnio").val();
        var sem = $("#filtroSemestre").val();
        var url = URL_PLANILLA + "?anio=" + encodeURIComponent(anio) + "&semestre=" + encodeURIComponent(sem);
        window.open(url, "_blank");
    }

    /* ------------------------------------------------------------ */
    /* Eventos                                                       */
    /* ------------------------------------------------------------ */
    function bind() {
        $("#filtroAnio, #filtroSemestre").on("change", cargar);
        $("#filtroOrden").on("change", aplicarFiltros);
        $("#btnExportCsv").on("click", exportarCsv);
        $("#btnImprimir").on("click", imprimirPlanilla);
        var deb;
        $("#filtroBusqueda").on("input", function () {
            clearTimeout(deb);
            deb = setTimeout(aplicarFiltros, 180);
        });
    }

    $(function () { bind(); cargar(); });
})();
