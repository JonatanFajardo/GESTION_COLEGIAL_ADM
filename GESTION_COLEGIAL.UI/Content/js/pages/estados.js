var Estados = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                //["Fila"],
                "Est_Id",
                "Est_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());