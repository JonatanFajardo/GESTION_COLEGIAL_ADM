var Materias = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Mat_Id",
                    Size: 200
                },
                {
                    FieldName: "Mat_Nombre"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());