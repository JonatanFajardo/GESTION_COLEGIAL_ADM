var CursosNombres = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Cno_Id",
                "Cno_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());