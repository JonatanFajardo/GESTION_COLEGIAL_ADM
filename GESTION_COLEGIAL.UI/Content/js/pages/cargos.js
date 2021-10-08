var Cargos = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Car_Id",
                "Car_Descripcion"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());