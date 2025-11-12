var FormasPago = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "FormaPagoId",
                    Size: 100
                },
                {
                    FieldName: "Descripcion"
                },
                {
                    FieldName: "EsActivo",
                    Size: 100
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
