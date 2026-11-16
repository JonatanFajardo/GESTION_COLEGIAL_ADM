/** Movimientos de caja: los cobros de un día o de un rango de fechas. Los anulados se ven tachados y no suman. */
$(function () {
    var $desde = $("#f-desde"), $hasta = $("#f-hasta");

    function rango(r) {
        var h = new Date(), d = new Date();
        if (r === "semana") d.setDate(h.getDate() - ((h.getDay() + 6) % 7));
        if (r === "mes") d = new Date(h.getFullYear(), h.getMonth(), 1);
        if (r === "anio") d = new Date(h.getFullYear(), 0, 1);
        $desde.val(Caja.iso(d)); $hasta.val(Caja.iso(h));
    }
    function url() { return "/Pagos/ListByRangoAsync?desde=" + $desde.val() + "&hasta=" + $hasta.val(); }
    function anulado(r) { return r.EsAnulado === true; }

    function pintarKpis(filas) {
        var total = 0, formas = {}, validos = 0, anulados = 0;
        filas.forEach(function (r) {
            if (anulado(r)) { anulados++; return; }
            var m = parseFloat(r.MontoTotal) || 0;
            validos++;
            total += m;
            var f = r.FormaPago || "Sin dato";
            formas[f] = (formas[f] || 0) + m;
        });
        $("#k-total").text(Caja.dinero(total));
        $("#k-recibos").text(validos);
        $("#k-anulados").text(anulados === 0 ? "Ninguno anulado" : (anulados === 1 ? "1 anulado (no suma)" : anulados + " anulados (no suman)"));
        var html = Object.keys(formas).sort(function (a, b) { return formas[b] - formas[a]; }).map(function (f) {
            return '<span class="badge badge-primary mr-2 mb-2 p-2">' + Caja.esc(f) + ': ' + Caja.dinero(formas[f]) + '</span>';
        }).join("");
        $("#k-formas").html(html || '<span class="text-muted small">Sin cobros en este período.</span>');
        var d = $desde.val(), h = $hasta.val();
        $("#titulo-rango").text(d === h ? "Cobros del " + Caja.fecha(d + "T00:00:00") :
            "Cobros del " + Caja.fecha(d + "T00:00:00") + " al " + Caja.fecha(h + "T00:00:00"));
    }

    rango($("#f-rapido").data("periodo") || "hoy");
    var t = GcTabla.crear({
        elemento: "#tabla-mov",
        nombre: "movimientos-caja",
        url: url(),
        archivo: "movimientos-caja",
        tituloPdf: "Movimientos de caja",
        vacio: "No hay cobros en este período. Probá con otro rango de fechas.",
        agrupar: [
            { campos: ["Fecha"], texto: "Día" },
            { campos: ["FormaPago"], texto: "Forma de pago" },
            { campos: ["Usuario"], texto: "Cajero" }
        ],
        buscarEn: ["NumeroRecibo", "NumeroReferencia", "Alumno", "Encargado", "FormaPago", "Usuario", "Observaciones"],
        alCargar: pintarKpis,
        formatoFila: function (row) {
            if (anulado(row.getData())) row.getElement().classList.add("fila-anulada");
        },
        // Recibo, nota y cajero se ven debajo de la fecha, del alumno y de la forma de pago;
        // sus columnas propias empiezan ocultas (se pueden mostrar con "Columnas").
        ocultas: ["NumeroRecibo", "Observaciones", "Usuario"],
        columnas: [
            { title: "Fecha y hora", field: "FechaPago", width: 160, sorter: "datetime", sorterParams: { format: "iso" },
              mutator: function (v, d) { d.Fecha = Caja.fecha(v); return v; },
              formatter: function (c) {
                  var d = c.getData(), x = new Date(c.getValue());
                  var recibo = d.NumeroRecibo || d.NumeroReferencia || "";
                  return isNaN(x) ? "" : '<div><div>' + x.toLocaleDateString("es-HN") + ' <span class="text-muted small">' + x.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" }) + '</span></div>' +
                      (recibo ? '<div class="small text-muted">' + Caja.esc(recibo) + '</div>' : '') + '</div>';
              },
              accessorDownload: function (v) { return Caja.fechaHora(v); } },
            { title: "Alumno", field: "Alumno", minWidth: 200,
              formatter: function (c) {
                  var d = c.getData();
                  var nombre = d.AlumnoId ? '<a href="/CuentasCobrar/EstadoCuenta?alumnoId=' + d.AlumnoId + '" title="Abrir la cuenta del alumno">' + Caja.esc(c.getValue()) + '</a>' : Caja.esc(c.getValue());
                  var nota = d.EsAnulado && d.MotivoAnulacion ? "Anulado: " + d.MotivoAnulacion : (d.Observaciones || "");
                  return '<div class="text-truncate"><div class="text-truncate">' + nombre + '</div>' + (nota ? '<div class="small text-muted text-truncate">' + Caja.esc(nota) + '</div>' : '') + '</div>';
              } },
            { title: "Monto", field: "MontoTotal", width: 120, hozAlign: "right", sorter: "number",
              formatter: function (c) {
                  return anulado(c.getData()) ? Caja.dinero(c.getValue()) : '<strong class="text-success">' + Caja.dinero(c.getValue()) + '</strong>';
              } },
            { title: "Estado", field: "EsAnulado", width: 110,
              formatter: function (c) {
                  var d = c.getData();
                  return d.EsAnulado ? '<span class="badge badge-danger" title="' + Caja.esc(d.MotivoAnulacion || "") + '">Anulado</span>' : '';
              },
              accessorDownload: function (v) { return v ? "Anulado" : "Válido"; } },
            { title: "Forma de pago", field: "FormaPago", minWidth: 150,
              formatter: function (c) {
                  var d = c.getData();
                  return '<div class="text-truncate"><div>' + Caja.esc(c.getValue() || "") + '</div>' + (d.Usuario ? '<div class="small text-muted text-truncate">' + Caja.esc(d.Usuario) + '</div>' : '') + '</div>';
              } },
            { title: "Recibo", field: "NumeroRecibo", width: 120,
              formatter: function (c) { var d = c.getData(); return Caja.esc(c.getValue() || d.NumeroReferencia || ""); },
              accessorDownload: function (v, d) { return v || d.NumeroReferencia || ""; } },
            { title: "Concepto / nota", field: "Observaciones", minWidth: 140 },
            { title: "Cajero", field: "Usuario", width: 120 },
            { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 75, download: false,
              formatter: function (c) {
                  return '<button type="button" class="btn btn-sm btn-outline-primary recibo-btn" title="Imprimir recibo" aria-label="Imprimir recibo" data-id="' + c.getData().PagoId + '"><i class="bi bi-printer"></i></button>';
              } }
        ]
    });

    function cargar() { t.tabla.setData(url()); }
    $("#f-rapido").on("click", "button", function () {
        $("#f-rapido button").removeClass("btn-primary").addClass("btn-outline-primary");
        $(this).removeClass("btn-outline-primary").addClass("btn-primary");
        rango($(this).data("r"));
        if (history.replaceState) history.replaceState(null, "", "?periodo=" + $(this).data("r"));
        cargar();
    });
    $("#btn-ver").on("click", function () {
        if (!$desde.val() || !$hasta.val()) { Caja.avisar("warning", "Elegí las dos fechas del rango."); return; }
        if ($desde.val() > $hasta.val()) { Caja.avisar("warning", "La fecha \"Desde\" no puede ser posterior a \"Hasta\"."); return; }
        $("#f-rapido button").removeClass("btn-primary").addClass("btn-outline-primary");
        cargar();
    });
    $("#tabla-mov").on("click", ".recibo-btn", function () {
        window.open("/Pagos/Recibo?pagoId=" + $(this).data("id"), "_blank");
    });
});
