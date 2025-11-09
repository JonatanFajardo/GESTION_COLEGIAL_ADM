var Estados = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "EstadoId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionEstado"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
