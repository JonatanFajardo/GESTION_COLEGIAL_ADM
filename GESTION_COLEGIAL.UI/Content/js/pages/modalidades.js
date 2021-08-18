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
            //header = [
            //    ["raza_Id", 80, false],
            //    ["raza_Descripcion", true, true]
            //];
            console.log(Direction.listUrl);
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;

}());

$(function () {

});

