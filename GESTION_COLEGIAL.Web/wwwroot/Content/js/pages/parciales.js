var Parciales = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Pac_Id",
                    Size: 200
                },
                {
                    FieldName: "Pac_Descripcion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());