var Duraciones = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "DuracionId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionDuracion"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
