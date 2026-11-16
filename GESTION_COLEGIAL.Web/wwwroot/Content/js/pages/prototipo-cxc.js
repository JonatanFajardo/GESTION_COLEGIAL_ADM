/** Prototipo: Cuentas por cobrar unificada (Pendientes / Vencidas / Pagadas / Todas). */
$(function () {
    var HOY = new Date(); HOY.setHours(0, 0, 0, 0);
    var estadoSel = "pendiente";

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
    var COLOR_ANTIG = { "Al día": "success", "1-30 días": "info", "31-60 días": "warning", "61-90 días": "danger", "Más de 90 días": "danger" };
    var ETIQUETA = { pendiente: ["Pendiente", "warning"], vencida: ["Vencida", "danger"], pagada: ["Pagada", "success"] };

    function pintarKpis(filas) {
        filas.forEach(function (r) { r._dias = dias(r); r._estado = estado(r); });
        var deuda = filas.filter(function (r) { return r._estado !== "pagada"; });
        var total = deuda.reduce(function (s, r) { return s + (parseFloat(r.Pendiente) || 0); }, 0);
        var venc30 = deuda.filter(function (r) { return r._dias > 30; })
                          .reduce(function (s, r) { return s + (parseFloat(r.Pendiente) || 0); }, 0);
        var alumnos = {};
        deuda.forEach(function (r) { alumnos[r.Alumno] = 1; });
        $("#k-total").text(Proto.dinero(total));
        $("#k-vencido").text(Proto.dinero(venc30));
        $("#k-alumnos").text(Object.keys(alumnos).length);
        $("#k-cargos").text(deuda.length);

        var n = { pendiente: 0, vencida: 0, pagada: 0, todas: filas.length };
        filas.forEach(function (r) { n[r._estado]++; });
        // "Pendientes" = todo lo que se debe (vencido o no), como en la guía.
        n.pendiente += n.vencida;
        Object.keys(n).forEach(function (k) { $('[data-n="' + k + '"]').text(n[k]); });
    }

    Proto.cargarAlumnos().always(function () {
        var t = GcTabla.crear({
            elemento: "#tabla-cxc",
            nombre: "proto-cxc",
            url: "/CuentasCobrar/ListAsync",
            archivo: "cuentas-por-cobrar",
            tituloPdf: "Cuentas por cobrar",
            agrupar: [
                { campos: ["Alumno"], texto: "Alumno" },
                { campos: ["Concepto"], texto: "Concepto" },
                { campos: ["_antig"], texto: "Antigüedad" },
                { campos: ["Nivel"], texto: "Nivel" }
            ],
            buscarEn: ["Alumno", "Concepto", "Nivel", "Seccion", "Identidad"],
            filtroExtra: function (r) {
                var okEstado = estadoSel === "todas" ||
                    (estadoSel === "pendiente" ? r._estado !== "pagada" : r._estado === estadoSel);
                var antig = $("#f-antig").val();
                return okEstado && (!antig || r._antig === antig);
            },
            alCargar: pintarKpis,
            columnas: [
                { title: "Alumno", field: "Alumno", minWidth: 220,
                  mutator: function (v, d) {
                      var a = Proto.buscarAlumno(v);
                      d._aluId = a ? a.Alu_Id : null;
                      d.Nivel = a ? a.Niv_Descripcion : "";
                      d.Seccion = a ? (a.Grado + " · " + a.Sec_Descripcion) : "";
                      d.Identidad = a ? a.Per_Identidad : "";
                      d._dias = dias(d); d._estado = estado(d); d._antig = d._estado === "pagada" ? "Al día" : antiguedad(d._dias);
                      return v;
                  },
                  formatter: function (c) {
                      var d = c.getData();
                      return '<div class="text-truncate"><div class="font-weight-bold text-truncate">' + c.getValue() + '</div>' +
                             '<div class="small text-muted text-truncate">' + (d.Nivel ? d.Nivel + " · " + d.Seccion : "Sin ubicación") + '</div></div>';
                  } },
                { title: "Concepto", field: "Concepto", minWidth: 110 },
                { title: "Pendiente", field: "Pendiente", width: 125, hozAlign: "right", sorter: "number",
                  formatter: function (c) { return '<strong>' + Proto.dinero(c.getValue()) + '</strong>'; } },
                { title: "Vence", field: "FechaVence", width: 105, sorter: "datetime", sorterParams: { format: "iso" },
                  formatter: function (c) { return Proto.fecha(c.getValue()); },
                  accessorDownload: function (v) { return Proto.fecha(v); } },
                { title: "Antigüedad", field: "_antig", width: 150,
                  formatter: function (c) {
                      var d = c.getData();
                      if (d._estado === "pagada") return '<span class="text-muted">—</span>';
                      return '<span class="badge badge-' + COLOR_ANTIG[d._antig] + '">' + d._antig + '</span>' +
                             (d._dias > 0 ? ' <span class="small text-muted">' + d._dias + ' d</span>' : '');
                  } },
                { title: "Estado", field: "_estado", width: 100,
                  formatter: function (c) { var e = ETIQUETA[c.getValue()] || ["—", "secondary"]; return '<span class="badge badge-' + e[1] + '">' + e[0] + '</span>'; },
                  accessorDownload: function (v) { return (ETIQUETA[v] || [""])[0]; } },
                { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 185, download: false,
                  formatter: function (c) {
                      var d = c.getData();
                      if (!d._aluId) return '<span class="small text-muted">Alumno no enlazado</span>';
                      var cobrar = d._estado === "pagada" ? "" :
                          '<a class="btn btn-sm btn-primary mr-1 d-inline-flex align-items-center" href="/Pagos/NuevoPago?alumnoId=' + d._aluId + '"><i class="bi bi-cash-coin mr-1"></i>Cobrar</a>';
                      return cobrar + '<a class="btn btn-sm btn-outline-primary d-inline-flex align-items-center" href="/Prototipos/CuentaAlumno?alumnoId=' + d._aluId + '" title="Abrir cuenta del alumno"><i class="bi bi-person-vcard mr-1"></i>Cuenta</a>';
                  } }
            ]
        });

        $("#f-estado").on("click", "button", function () {
            estadoSel = $(this).data("estado");
            $("#f-estado button").removeClass("btn-primary").addClass("btn-outline-primary");
            $(this).removeClass("btn-outline-primary").addClass("btn-primary");
            t.filtrar();
        });
        $("#f-antig").on("change", function () { t.filtrar(); });
    });
});
