var EstadoCuentaAdm = (function () {
    var obj = {};
    var _instituto = { Nombre: "Instituto", Direccion: "" };

    obj.init = function (urlGet, urlDD) {
        // Precargar datos de la institución para el membrete de impresión
        $.getJSON("/InstitutoConfig/GetAsync", function (resp) {
            var d = (resp && resp.data) || {};
            _instituto = {
                Nombre: d.Nombre || "Instituto",
                Direccion: d.Direccion || ""
            };
        });

        $.getJSON(urlDD, function (data) {
            var sel = $("#f-alu");
            $.each(data, function (_, a) { sel.append('<option value="' + a.id + '">' + a.texto + '</option>'); });
        });

        // Botón Imprimir / PDF — abre el diálogo de impresión sobre la vista imprimible
        $("#btn-imprimir").on("click", function () { window.print(); });

        $("#btn-ver").on("click", function () {
            var id   = $("#f-alu").val();
            var anio = $("#f-anio").val();
            if (!id) { alert("Seleccione un alumno."); return; }
            var params = { Alu_Id: id };
            if (anio) params.Anio = anio;

            var nombreAlumno = $("#f-alu option:selected").text();

            $.getJSON(urlGet, params, function (res) {
                var d = res.item || res || {};
                var sym = window.GC ? window.GC.currencySymbol() : 'L';
                $("#kpi-cobrado").text(sym + " " + (d.TotalCobrado   || "0.00"));
                $("#kpi-pendiente").text(sym + " " + (d.TotalPendiente || "0.00"));
                $("#kpi-descuento").text(sym + " " + (d.TotalDescuento || "0.00"));
                $("#kpi-cuotas").text(d.CuotasPendientes || "0");

                var tbody = $("#tbody-cuentas").empty();
                var cuentas = d.Cuentas || d.detalle || [];
                if (!cuentas.length) {
                    tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin cuentas registradas.</td></tr>');
                } else {
                    $.each(cuentas, function (_, c) {
                        var cls = c.Cue_Saldo > 0 ? 'text-danger' : 'text-success';
                        tbody.append('<tr><td>' + c.Con_Descripcion + '</td>' +
                            '<td>' + sym + ' ' + c.Cue_Monto + '</td>' +
                            '<td>' + sym + ' ' + c.Cue_Pagado + '</td>' +
                            '<td class="' + cls + '">' + sym + ' ' + c.Cue_Saldo + '</td>' +
                            '<td>' + (c.Cue_FechaVence || '—') + '</td>' +
                            '<td>' + (c.Est_Descripcion || '—') + '</td></tr>');
                    });
                }
                // Poblar membrete de impresión
                $("#print-institucion").text(_instituto.Nombre);
                $("#print-direccion").text(_instituto.Direccion);
                $("#print-alumno").text(nombreAlumno);
                $("#print-anio").text(anio || new Date().getFullYear());
                $("#print-fecha").text(new Date().toLocaleDateString("es-HN", { day: "2-digit", month: "long", year: "numeric" }));

                $("#panel-estado").show();
                $("#btn-imprimir").show();
            });
        });
    };

    return obj;
}());
