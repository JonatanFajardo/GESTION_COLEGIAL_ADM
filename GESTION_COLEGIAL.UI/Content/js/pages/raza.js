var raza = (function () {
   
    var obj = {};

    obj.datatable = function (Direction) {
        $(function() {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                ["Fila"],
                ["raza_Id"],
                ["raza_Descripcion"]
            ];
            //header = [
            //    ["raza_Id", 80, false],
            //    ["raza_Descripcion", true, true]
            //];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;

}());

$(function () {

});

           