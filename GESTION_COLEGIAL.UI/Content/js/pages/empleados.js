var Empleados = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "EmpleadoId",
                    Size: 200
                },
                {
                    FieldName: "CodigoEmpleado"
                },
                {
                    FieldName: "NombreEmpleado"
                },
                {
                    FieldName: "EsActivo"
                },
                {
                    FieldName: "DescripcionTitulo"
                },
                {
                    FieldName: "DescripcionCargo"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
