var Morosos = (function () {
    var obj = {};
    var diasFiltro = 0;

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                {
                    FieldName: "NombreAlumno"
                },
                {
                    FieldName: "Identidad"
                },
                {
                    FieldName: "NombreEncargado"
                },
                {
                    FieldName: "Telefono"
                },
                {
                    FieldName: "Email"
                },
                {
                    FieldName: "TotalDeuda"
                },
                {
                    FieldName: "MontoMora"
                },
                {
                    FieldName: "DiasAtraso"
                }
            ]
            datatable.init(Direction, header);

            // Filtros de días
            $(".btn-group button").click(function() {
                $(".btn-group button").removeClass("active");
                $(this).addClass("active");
                diasFiltro = $(this).data("dias");
                $("#datatable").DataTable().ajax.reload();
            });

            // Calcular resumen
            calculateResumen();
        })
    }

    function calculateResumen() {
        setTimeout(function() {
            var totalMorosos = 0;
            var deudaTotal = 0;
            var moraTotal = 0;

            $("#datatable tbody tr").each(function() {
                totalMorosos++;
                var deuda = parseFloat($(this).find("td:eq(5)").text().replace(/[^0-9.-]+/g,""));
                var mora = parseFloat($(this).find("td:eq(6)").text().replace(/[^0-9.-]+/g,""));

                if (!isNaN(deuda)) deudaTotal += deuda;
                if (!isNaN(mora)) moraTotal += mora;
            });

            $("#total-morosos").text(totalMorosos);
            $("#deuda-total").text("L. " + formatMoney(deudaTotal));
            $("#mora-acumulada").text("L. " + formatMoney(moraTotal));
            $("#promedio-deuda").text("L. " + formatMoney(totalMorosos > 0 ? deudaTotal / totalMorosos : 0));
        }, 1000);
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    return obj;
}());
