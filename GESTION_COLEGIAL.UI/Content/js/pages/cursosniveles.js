var CursosNiveles = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "CursoNivelId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionCursoNivel"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
