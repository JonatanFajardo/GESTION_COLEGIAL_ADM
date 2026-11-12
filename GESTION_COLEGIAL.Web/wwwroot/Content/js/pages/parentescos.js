var Parentescos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Par_Id",
                    Size: 200
                },
                {
                    FieldName: "Par_Descripcion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());