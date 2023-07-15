var Empleados = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Emp_Id",
                    Size: 200
                },
                {
                    FieldName: "Emp_Codigo"
                },
                {
                    FieldName: "Emp_Nombre"
                },
                {
                    FieldName: "EsActivo"
                },
                {
                    FieldName: "Tit_Descripcion"
                },
                {
                    FieldName: "Car_Descripcion"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());