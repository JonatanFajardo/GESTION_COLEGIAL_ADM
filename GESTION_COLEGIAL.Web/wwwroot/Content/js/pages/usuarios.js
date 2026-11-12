var Usuarios = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Usu_Id",
                    Size: 80
                },
                {
                    FieldName: "Usu_Name",
                    Size: 150
                },
                {
                    FieldName: "Rol_Descripcion",
                    Size: 150
                },
                {
                    FieldName: "Usu_EsActivo",
                    Size: 100
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
