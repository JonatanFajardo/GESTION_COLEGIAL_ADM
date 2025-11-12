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
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
