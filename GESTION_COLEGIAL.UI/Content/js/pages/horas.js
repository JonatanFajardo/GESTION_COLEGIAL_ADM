var Horas = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Hor_Id",
                "Hor_Hora"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());