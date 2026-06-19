var ArqueosCajaAdm = (function () {
    var obj = {};

    function kv(label, val) {
        return '<div class="row mb-2"><div class="col-5 text-muted small">' + label + '</div>' +
               '<div class="col-7 fw-semibold">' + (val !== undefined && val !== null ? val : '—') + '</div></div>';
    }

    obj.init = function (urlLast, urlCreate) {
        $.getJSON(urlLast, function (res) {
            var d = res.item || res;
            if (d && d.Arq_Id) {
                var sym = window.GC ? window.GC.currencySymbol() : 'Q';
                $("#info-arqueo").html(
                    kv("Fecha", d.Arq_Fecha) +
                    kv("Monto Inicial", sym + " " + d.Arq_MontoInicial) +
                    kv("Monto Final",   sym + " " + d.Arq_MontoFinal) +
                    kv("Total Efectivo",sym + " " + d.Arq_TotalEfectivo) +
                    kv("Total Tarjeta", sym + " " + d.Arq_TotalTarjeta) +
                    kv("Observación",   d.Arq_Observacion)
                );
                $("#panel-ultimo").show();
                $("#panel-sin-arqueo").hide();
            }
        });

        $("#form-arqueo").on("submit", function (e) {
            e.preventDefault();
            $.post(urlCreate, $(this).serialize(), function (res) {
                if (res && res.success) {
                    $("#modal-arqueo").modal("hide");
                    location.reload();
                }
            });
        });
    };

    return obj;
}());
