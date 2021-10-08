var Dias = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Dia_Id",
                "Dia_Descripcion"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());