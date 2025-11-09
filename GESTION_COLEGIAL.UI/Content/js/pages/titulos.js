var Titulos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "TituloId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionTitulo"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
