var Semestres = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Sem_Id",
                "Sem_Descripcion"
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());