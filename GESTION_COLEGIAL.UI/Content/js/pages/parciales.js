var Parciales = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "ParcialId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionParcial"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
