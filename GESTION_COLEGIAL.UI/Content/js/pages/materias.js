var Materias = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "MateriaId",
                    Size: 200
                },
                {
                    FieldName: "NombreMateria"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
