var Modalidades = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                //["Fila"],
                "Mda_Id",
                "Mda_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;

}());


