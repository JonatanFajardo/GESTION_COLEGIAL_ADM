var CursosNiveles = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Cun_Id",
                "Cun_Descripcion"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());