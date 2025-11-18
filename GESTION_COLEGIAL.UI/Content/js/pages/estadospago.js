var EstadosPago = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "EstadoPagoId",
                    Size: 100
                },
                {
                    FieldName: "Descripcion"
                },
                {
                    FieldName: "EsActivo",
                    Size: 100
                },
                {
                    FieldName: "CantidadCuentas",
                    Size: 120
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
