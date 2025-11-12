var Deudores = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                {
                    FieldName: "NombreCompleto"
                },
                {
                    FieldName: "Identidad"
                },
                {
                    FieldName: "Nivel"
                },
                {
                    FieldName: "Curso"
                },
                {
                    FieldName: "TotalDeuda"
                },
                {
                    FieldName: "MontoPendiente"
                },
                {
                    FieldName: "UltimaFechaVencimiento"
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
