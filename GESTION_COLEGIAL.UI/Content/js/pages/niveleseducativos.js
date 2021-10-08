var NivelesEducativos = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Niv_Id",
                "Niv_Descripcion"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());