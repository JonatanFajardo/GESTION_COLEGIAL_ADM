/** Pagos del mes: tabla personalizable por usuario (ver components/gc-tabla.js). */
$(function () {
    function fecha(v) {
        var d = new Date(v);
        return isNaN(d.getTime()) ? "" : d.toLocaleDateString("es-HN");
    }

    GcTabla.crear({
        elemento: "#tabla-pagos",
        nombre: "pagos",
        url: "/Pagos/ListAsync",
        archivo: "pagos",
        tituloPdf: "Pagos del mes",
        buscarEn: ["Fila", "Fecha", "Alumno", "Encargado", "FormaPago", "MontoTotal", "Usuario"],
        columnas: [
            { title: "No. Recibo", field: "Fila", width: 120 },
            { title: "Fecha", field: "Fecha", width: 120,
              mutator: function (v, d) { return fecha(d.FechaPago); } },
            { title: "Alumno", field: "Alumno", minWidth: 180 },
            { title: "Encargado", field: "Encargado", minWidth: 180 },
            { title: "Forma de pago", field: "FormaPago", minWidth: 130 },
            { title: "Monto total", field: "MontoTotal", width: 140, hozAlign: "right", sorter: "number",
              formatter: function (c) { return '<strong class="text-success">L ' + parseFloat(c.getValue() || 0).toFixed(2) + '</strong>'; } },
            { title: "Usuario", field: "Usuario", minWidth: 110 },
            { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 130, download: false,
              formatter: function (c) {
                  return '<button type="button" class="btn btn-sm btn-outline-primary recibo-btn" data-id="' + c.getData().PagoId + '"><i class="bi bi-printer mr-1"></i>Recibo</button>';
              } }
        ]
    });

    $("#tabla-pagos").on("click", ".recibo-btn", function () {
        window.open("/Pagos/Recibo?pagoId=" + $(this).data("id"), "_blank");
    });
});
