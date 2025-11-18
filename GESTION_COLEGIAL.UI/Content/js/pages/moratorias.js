var Moratorias = (function () {
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
                    FieldName: "DiasVencido"
                },
                {
                    FieldName: "MontoMora"
                },
                {
                    FieldName: "TotalConMora"
                },
                {
                    FieldName: "FechaVencimiento"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
