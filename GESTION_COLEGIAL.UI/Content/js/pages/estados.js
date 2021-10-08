var Estados = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Est_Id",
                    Size: 200
                },
                {
                    FieldName: "Est_Descripcion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());