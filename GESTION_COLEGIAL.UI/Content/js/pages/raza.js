var raza = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function() {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                ["Fila"],
                ["raza_Id"],
                ["raza_Descripcion"]
            ];
            //header = [
            //    ["raza_Id", 80, false],
            //    ["raza_Descripcion", true, true]
            //];
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

$(function () {
});
