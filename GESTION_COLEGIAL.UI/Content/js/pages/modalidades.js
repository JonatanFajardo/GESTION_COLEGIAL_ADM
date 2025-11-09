var Modalidades = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "ModalidadId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionModalidad"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
