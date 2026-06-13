var EstadoCuentaAdm = (function () {
    var obj = {};

    obj.init = function (urlGet, urlDD) {
        $.getJSON(urlDD, function (data) {
            var sel = $("#f-alu");
            $.each(data, function (_, a) { sel.append('<option value="' + a.id + '">' + a.texto + '</option>'); });
        });

        $("#btn-ver").on("click", function () {
            var id   = $("#f-alu").val();
            var anio = $("#f-anio").val();
            if (!id) { alert("Seleccione un alumno."); return; }
            var params = { Alu_Id: id };
            if (anio) params.Anio = anio;

            $.getJSON(urlGet, params, function (res) {
                var d = res.item || res || {};
                $("#kpi-cobrado").text("Q " + (d.TotalCobrado   || "0.00"));
                $("#kpi-pendiente").text("Q " + (d.TotalPendiente || "0.00"));
                $("#kpi-descuento").text("Q " + (d.TotalDescuento || "0.00"));
                $("#kpi-cuotas").text(d.CuotasPendientes || "0");

                var tbody = $("#tbody-cuentas").empty();
                var cuentas = d.Cuentas || d.detalle || [];
                if (!cuentas.length) {
                    tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin cuentas registradas.</td></tr>');
                } else {
                    $.each(cuentas, function (_, c) {
                        var cls = c.Cue_Saldo > 0 ? 'text-danger' : 'text-success';
                        tbody.append('<tr><td>' + c.Con_Descripcion + '</td>' +
                            '<td>Q ' + c.Cue_Monto + '</td>' +
                            '<td>Q ' + c.Cue_Pagado + '</td>' +
                            '<td class="' + cls + '">Q ' + c.Cue_Saldo + '</td>' +
                            '<td>' + (c.Cue_FechaVence || '—') + '</td>' +
                            '<td>' + (c.Est_Descripcion || '—') + '</td></tr>');
                    });
                }
                $("#panel-estado").show();
            });
        });
    };

    return obj;
}());
