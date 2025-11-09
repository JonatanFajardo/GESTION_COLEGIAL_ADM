var Secciones = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "SeccionId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionSeccion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
