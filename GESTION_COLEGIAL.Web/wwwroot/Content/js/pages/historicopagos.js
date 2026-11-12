var HistoricoPagos = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                {
                    FieldName: "PagoId"
                },
                {
                    FieldName: "FechaPago"
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
        })
    }
    return obj;
}());
