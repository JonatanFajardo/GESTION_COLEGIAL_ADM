/** Prototipo: Movimientos de caja (un día o un rango de fechas). */
$(function () {
    var $desde = $("#f-desde"), $hasta = $("#f-hasta");

    function rango(r) {
        var h = new Date(), d = new Date();
        if (r === "semana") d.setDate(h.getDate() - ((h.getDay() + 6) % 7));
        if (r === "mes") d = new Date(h.getFullYear(), h.getMonth(), 1);
        if (r === "anio") d = new Date(h.getFullYear(), 0, 1);
        $desde.val(Proto.iso(d)); $hasta.val(Proto.iso(h));
    }
    function url() { return "/Prototipos/PagosRangoAsync?desde=" + $desde.val() + "&hasta=" + $hasta.val(); }

    function pintarKpis(filas) {
        var total = 0, formas = {};
        filas.forEach(function (r) {
            var m = parseFloat(r.MontoTotal) || 0;
            total += m;
            formas[r.FormaPago || "Sin dato"] = (formas[r.FormaPago || "Sin dato"] || 0) + m;
        });
        $("#k-total").text(Proto.dinero(total));
        $("#k-recibos").text(filas.length);
        var html = Object.keys(formas).sort(function (a, b) { return formas[b] - formas[a]; }).map(function (f) {
            return '<span class="badge badge-primary mr-2 mb-2 p-2">' + f + ': ' + Proto.dinero(formas[f]) + '</span>';
        }).join("");
        $("#k-formas").html(html || '<span class="text-muted small">Sin cobros en este período.</span>');
        var d = $desde.val(), h = $hasta.val();
        $("#titulo-rango").text(d === h ? "Cobros del " + Proto.fecha(d + "T00:00:00") :
            "Cobros del " + Proto.fecha(d + "T00:00:00") + " al " + Proto.fecha(h + "T00:00:00"));
    }

    rango("hoy");
    var t = GcTabla.crear({
        elemento: "#tabla-mov",
        nombre: "proto-movimientos",
        url: url(),
        archivo: "movimientos-caja",
        tituloPdf: "Movimientos de caja",
        vacio: "No hay cobros en este período. Probá con otro rango de fechas.",
        agrupar: [
            { campos: ["Fecha"], texto: "Día" },
            { campos: ["FormaPago"], texto: "Forma de pago" },
            { campos: ["Usuario"], texto: "Cajero" }
        ],
        buscarEn: ["NumeroReferencia", "Alumno", "Encargado", "FormaPago", "Usuario", "Observaciones"],
        alCargar: pintarKpis,
        columnas: [
            { title: "Fecha y hora", field: "FechaPago", width: 150, sorter: "datetime", sorterParams: { format: "iso" },
              mutator: function (v, d) { d.Fecha = Proto.fecha(v); return v; },
              formatter: function (c) {
                  var x = new Date(c.getValue());
                  return isNaN(x) ? "" : x.toLocaleDateString("es-HN") + ' <span class="text-muted small ml-1">' + x.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" }) + '</span>';
              },
              accessorDownload: function (v) { return new Date(v).toLocaleString("es-HN"); } },
            { title: "Referencia", field: "NumeroReferencia", width: 115 },
            { title: "Alumno", field: "Alumno", minWidth: 170 },
            { title: "Monto", field: "MontoTotal", width: 120, hozAlign: "right", sorter: "number",
              formatter: function (c) { return '<strong class="text-success">' + Proto.dinero(c.getValue()) + '</strong>'; } },
            { title: "Concepto / nota", field: "Observaciones", minWidth: 120 },
            { title: "Forma de pago", field: "FormaPago", width: 120 },
            { title: "Cajero", field: "Usuario", width: 110 },
            { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 80, download: false,
              formatter: function (c) {
                  return '<button type="button" class="btn btn-sm btn-outline-primary recibo-btn" title="Imprimir recibo" data-id="' + c.getData().PagoId + '"><i class="bi bi-printer"></i></button>';
              } }
        ]
    });

    function cargar() { t.tabla.replaceData(url()); }
    $("#f-rapido").on("click", "button", function () {
        $("#f-rapido button").removeClass("btn-primary").addClass("btn-outline-primary");
        $(this).removeClass("btn-outline-primary").addClass("btn-primary");
        rango($(this).data("r")); cargar();
    });
    $("#btn-ver").on("click", function () {
        $("#f-rapido button").removeClass("btn-primary").addClass("btn-outline-primary");
        cargar();
    });
    $("#tabla-mov").on("click", ".recibo-btn", function () {
        window.open("/Pagos/Recibo?pagoId=" + $(this).data("id"), "_blank");
    });
});
