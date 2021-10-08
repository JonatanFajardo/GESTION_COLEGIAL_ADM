var Materias = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Mat_Id",
                "Mat_Nombre"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());