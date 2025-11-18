var Tarifas = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "TarifaId"
                },
                {
                    FieldName: "ConceptoPagoId"
                },
                {
                    FieldName: "Monto"
                },
                {
                    FieldName: "AnioVigencia"
                },
                {
                    FieldName: "EsActivo",
                    Size: 100
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
