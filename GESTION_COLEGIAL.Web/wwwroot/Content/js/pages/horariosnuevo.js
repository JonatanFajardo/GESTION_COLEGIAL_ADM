var HorariosNuevo = (function () {
    var obj = {};

    obj.datatableInit = function (cfg) {
        $(function () {
            datatableCatalogs.init(cfg, [
                { FieldName: "Hor_Año",                   Size: 70 },
                { FieldName: "Sem_Descripcion",           Size: 100 },
                { FieldName: "Dia_Descripcion",           Size: 100 },
                { FieldName: "Hor_HoraInicioDescripcion", Size: 90 },
                { FieldName: "Mat_Nombre" },
                { FieldName: "Emp_NombreCompleto" },
                { FieldName: "Sec_Descripcion",           Size: 90 },
                { FieldName: "Aul_Descripcion",           Size: 90 }
            ]);
        });
    };

    return obj;
}());
