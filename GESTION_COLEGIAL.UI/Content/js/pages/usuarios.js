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
                    Size: 100,
                    Render: function (data, type, row) {
                        return data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                    }
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
