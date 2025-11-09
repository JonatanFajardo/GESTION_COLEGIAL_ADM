var NivelesEducativos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "NivelId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionNivel"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
