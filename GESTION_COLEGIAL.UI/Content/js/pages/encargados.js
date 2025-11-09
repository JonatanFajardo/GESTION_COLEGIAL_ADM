var Encargados = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "EncargadoId"
                },
                {
                    FieldName: "NumeroIdentidad"
                },
                {
                    FieldName: "NombreEncargado"
                },
                {
                    FieldName: "Telefono"
                },
                {
                    FieldName: "OcupacionEncargado"
                },
                {
                    FieldName: "EsActivo"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
