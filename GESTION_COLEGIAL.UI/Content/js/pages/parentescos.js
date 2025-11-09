var Parentescos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "ParentescoId",
                    Size: 200
                },
                {
                    FieldName: "DescripcionParentesco"
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
