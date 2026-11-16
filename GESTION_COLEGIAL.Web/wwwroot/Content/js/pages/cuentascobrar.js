/** Cuentas por cobrar: una sola lista con filtro Pendientes / Vencidas / Pagadas / Todas y antigüedad. */
$(function () {
    var HOY = new Date(); HOY.setHours(0, 0, 0, 0);
    var estadoSel = $("#tabla-cxc").data("estado") || "pendiente";

    function dias(row) {
        var v = new Date(row.FechaVence);
        if (isNaN(v.getTime())) return 0;
        return Math.max(0, Math.floor((HOY - v) / 86400000));
    }
    function estado(row) {
        if (row.EstadoPago === "Pagado" || !(parseFloat(row.Pendiente) > 0)) return "pagada";
        return dias(row) > 0 ? "vencida" : "pendiente";
    }
    function antiguedad(d) {
        if (d <= 0) return "Al día";
        if (d <= 30) return "1-30 días";
        if (d <= 60) return "31-60 días";
        if (d <= 90) return "61-90 días";
        return "Más de 90 días";
    }
    function ubicacion(r) {
        var partes = [];
        if (r.Nivel) partes.push(r.Nivel);
        if (r.Grado) partes.push(r.Grado);
        if (r.Seccion) partes.push("Sección " + r.Seccion);
        return partes.join(" · ");
    }
    var COLOR_ANTIG = { "Al día": "success", "1-30 días": "info", "31-60 días": "warning", "61-90 días": "danger", "Más de 90 días": "danger" };
    var ETIQUETA = { pendiente: ["Pendiente", "warning"], vencida: ["Vencida", "danger"], pagada: ["Pagada", "success"] };

    function pintarKpis(filas) {
        filas.forEach(function (r) { r._dias = dias(r); r._estado = estado(r); });
        var deuda = filas.filter(function (r) { return r._estado !== "pagada"; });
        var suma = function (lista, campo) { return lista.reduce(function (s, r) { return s + (parseFloat(r[campo]) || 0); }, 0); };
        var alumnos = {};
        deuda.forEach(function (r) { alumnos[r.AlumnoId || r.Alumno] = 1; });
        $("#k-total").text(Caja.dinero(suma(deuda, "Pendiente")));
        $("#k-vencido").text(Caja.dinero(suma(deuda.filter(function (r) { return r._dias > 30; }), "Pendiente")));
        $("#k-alumnos").text(Object.keys(alumnos).length);
        $("#k-mora").text(Caja.dinero(suma(deuda, "MontoMora")));

        var n = { pendiente: 0, vencida: 0, pagada: 0, todas: filas.length };
        filas.forEach(function (r) { n[r._estado]++; });
        // "Pendientes" = todo lo que se debe (vencido o no).
        n.pendiente += n.vencida;
        Object.keys(n).forEach(function (k) { $('[data-n="' + k + '"]').text(n[k]); });
    }

    var t = GcTabla.crear({
        elemento: "#tabla-cxc",
        nombre: "cuentas-cobrar",
        url: "/CuentasCobrar/ListAsync",
        archivo: "cuentas-por-cobrar",
        tituloPdf: "Cuentas por cobrar",
        vacio: "No hay cargos con este filtro.",
        agrupar: [
            { campos: ["Alumno"], texto: "Alumno" },
            { campos: ["Concepto"], texto: "Concepto" },
            { campos: ["_antig"], texto: "Antigüedad" },
            { campos: ["Nivel"], texto: "Nivel" },
            { campos: ["Nivel", "Grado", "Seccion"], texto: "Nivel → Grado → Sección" }
        ],
        buscarEn: ["Alumno", "Concepto", "Nivel", "Grado", "Seccion", "Identidad"],
        filtroExtra: function (r) {
            var okEstado = estadoSel === "todas" ||
                (estadoSel === "pendiente" ? r._estado !== "pagada" : r._estado === estadoSel);
            var antig = $("#f-antig").val();
            return okEstado && (!antig || r._antig === antig);
        },
        alCargar: function (filas) { pintarKpis(filas); t && t.filtrar(); },
        columnas: [
            { title: "Alumno", field: "Alumno", minWidth: 170,
              mutator: function (v, d) {
                  d._dias = dias(d); d._estado = estado(d);
                  d._antig = d._estado === "pagada" ? "Al día" : antiguedad(d._dias);
                  d._ubicacion = ubicacion(d);
                  return v;
              },
              formatter: function (c) {
                  var d = c.getData();
                  return '<div class="text-truncate"><div class="font-weight-bold text-truncate">' + Caja.esc(c.getValue()) + '</div>' +
                         '<div class="small text-muted text-truncate">' + Caja.esc(d._ubicacion || "Sin ubicación") + '</div></div>';
              },
              accessorDownload: function (v, d) { return v + (d._ubicacion ? " (" + d._ubicacion + ")" : ""); } },
            { title: "Concepto", field: "Concepto", minWidth: 120,
              formatter: function (c) {
                  var d = c.getData();
                  return '<div class="text-truncate"><div class="text-truncate">' + Caja.esc(c.getValue()) + '</div>' + (d.Mes && d.Anio ? '<div class="small text-muted">' + d.Mes + '/' + d.Anio + '</div>' : '') + '</div>';
              } },
            { title: "Saldo", field: "Pendiente", width: 125, hozAlign: "right", sorter: "number",
              formatter: function (c) { return '<strong>' + Caja.dinero(c.getValue()) + '</strong>'; } },
            { title: "Mora", field: "MontoMora", width: 120, hozAlign: "right", sorter: "number",
              formatter: function (c) {
                  var v = parseFloat(c.getValue()) || 0;
                  return v > 0 ? '<span class="text-danger">' + Caja.dinero(v) + '</span>' : '<span class="text-muted">—</span>';
              } },
            { title: "Vence", field: "FechaVence", width: 125, sorter: "datetime", sorterParams: { format: "iso" },
              formatter: function (c) { return Caja.fecha(c.getValue()); },
              accessorDownload: function (v) { return Caja.fecha(v); } },
            // Estado + antigüedad en una sola columna (el filtro y el "Agrupar por" de antigüedad siguen disponibles).
            { title: "Estado", field: "_estado", width: 140, sorter: function (a, b, ra, rb) { return ra.getData()._dias - rb.getData()._dias; },
              formatter: function (c) {
                  var d = c.getData(), e = ETIQUETA[c.getValue()] || ["—", "secondary"];
                  var html = '<div><span class="badge badge-' + e[1] + '">' + e[0] + '</span>';
                  if (d._estado !== "pagada" && d._dias > 0)
                      html += '<div class="small mt-1 text-' + COLOR_ANTIG[d._antig] + '" title="' + d._antig + '">' + d._dias + (d._dias === 1 ? ' día' : ' días') + ' de atraso</div>';
                  return html + '</div>';
              },
              accessorDownload: function (v, d) { return (ETIQUETA[v] || [""])[0] + (d._estado !== "pagada" && d._dias > 0 ? " (" + d._antig + ")" : ""); } },
            { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 165, download: false,
              formatter: function (c) {
                  var d = c.getData();
                  if (!d.AlumnoId) return '<span class="small text-muted">Sin alumno</span>';
                  var cobrar = d._estado === "pagada" ? "" :
                      '<a class="btn btn-sm btn-primary mr-1 d-inline-flex align-items-center" href="/Pagos/NuevoPago?alumnoId=' + d.AlumnoId + '"><i class="bi bi-cash-coin mr-1"></i>Cobrar</a>';
                  return cobrar + '<a class="btn btn-sm btn-outline-primary" href="/CuentasCobrar/EstadoCuenta?alumnoId=' + d.AlumnoId + '" title="Abrir la cuenta del alumno" aria-label="Abrir la cuenta del alumno"><i class="bi bi-person-vcard"></i></a>';
              } }
        ]
    });

    function marcarEstado() {
        $("#f-estado button").each(function () {
            var activo = $(this).data("estado") === estadoSel;
            $(this).toggleClass("btn-primary", activo).toggleClass("btn-outline-primary", !activo);
        });
    }
    $("#f-estado").on("click", "button", function () {
        estadoSel = $(this).data("estado");
        marcarEstado();
        if (history.replaceState) history.replaceState(null, "", "?estado=" + estadoSel);
        t.filtrar();
    });
    $("#f-antig").on("change", function () { t.filtrar(); });
});
