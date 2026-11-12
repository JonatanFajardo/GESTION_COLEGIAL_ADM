var PagosDia = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                {
                    FieldName: "PagoId"
                },
                {
                    FieldName: "Hora"
                },
                {
                    FieldName: "NombreAlumno"
                },
                {
                    FieldName: "MontoTotal"
                },
                {
                    FieldName: "FormaPago"
                },
                {
                    FieldName: "NumeroReferencia"
                },
                {
                    FieldName: "Usuario"
                }
            ]
            datatable.init(Direction, header);

            // Calcular totales después de cargar la tabla
            calculateTotals();
        })
    }

    function calculateTotals() {
        setTimeout(function() {
            var total = 0;
            var count = 0;

            $("#datatable tbody tr").each(function() {
                var montoText = $(this).find("td:eq(3)").text();
                var monto = parseFloat(montoText.replace(/[^0-9.-]+/g,""));
                if (!isNaN(monto)) {
                    total += monto;
                    count++;
                }
            });

            $("#total-dia").text("L. " + formatMoney(total));
            $("#cantidad-pagos").text(count);
        }, 1000);
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    return obj;
}());
