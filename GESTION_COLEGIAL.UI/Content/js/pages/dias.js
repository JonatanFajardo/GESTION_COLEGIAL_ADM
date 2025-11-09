var Dias = (function () {
    console.log("sss")
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "DiaId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionDia"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
