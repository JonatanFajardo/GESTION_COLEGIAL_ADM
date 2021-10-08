var Duraciones = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Dur_Id",
                    Size: 200
                },
                {
                    FieldName: "Dur_Descripcion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());