var ConceptosPago = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "ConceptoPagoId",
                    Size: 100
                },
                {
                    FieldName: "Descripcion"
                },
                {
                    FieldName: "EsRecurrente",
                    Size: 120
                },
                {
                    FieldName: "EsObligatorio",
                    Size: 120
                },
                {
                    FieldName: "EsActivo",
                    Size: 100
                },
                {
                    FieldName: "CantTarifas",
                    Size: 100
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
