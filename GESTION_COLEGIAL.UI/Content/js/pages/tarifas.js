var Tarifas = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Tar_Id"
                },
                {
                    FieldName: "ConceptoPago"
                },
                {
                    FieldName: "Nivel"
                },
                {
                    FieldName: "Curso"
                },
                {
                    FieldName: "Tar_Monto"
                },
                {
                    FieldName: "Tar_AnioVigencia"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
