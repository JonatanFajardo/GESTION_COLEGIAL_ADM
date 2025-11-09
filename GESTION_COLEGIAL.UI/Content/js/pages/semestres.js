var Semestres = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "SemestreId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionSemestre"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
