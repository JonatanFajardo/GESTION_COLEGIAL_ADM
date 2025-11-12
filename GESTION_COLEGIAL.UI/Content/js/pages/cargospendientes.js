var CargosPendientes = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                {
                    FieldName: "CuentaCobrarId"
                },
                {
                    FieldName: "NombreAlumno"
                },
                {
                    FieldName: "ConceptoPago"
                },
                {
                    FieldName: "MontoOriginal"
                },
                {
                    FieldName: "MontoDescuento"
                },
                {
                    FieldName: "MontoMora"
                },
                {
                    FieldName: "MontoTotal"
                },
                {
                    FieldName: "FechaVencimiento"
                },
                {
                    FieldName: "EstadoPago"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
