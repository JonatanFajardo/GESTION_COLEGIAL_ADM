var Cargos = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Car_Id",
                    Size: 200
                },
                {
                    FieldName: "Car_Descripcion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());