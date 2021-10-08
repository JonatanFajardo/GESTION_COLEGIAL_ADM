var Empleados = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Emp_Id",
                "Nombre",
                "Per_CorreoElectronico",
                "Car_Descripcion",
                "EsActivo"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());