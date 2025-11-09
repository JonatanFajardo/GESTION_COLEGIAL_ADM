var Cargos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "CargoId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionCargo"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
