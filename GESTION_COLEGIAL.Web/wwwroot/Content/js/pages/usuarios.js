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

            // Contraseña: siempre vacía al abrir; obligatoria solo al crear (al editar, vacía = no cambia).
            $("#edit-modal").on("show.bs.modal", function () {
                var nuevo = !($("#item-id").val() > 0);
                $("#Usu_Contrasena").val("");
                $("#contrasena-obligatoria").toggle(nuevo);
                $("#contrasena-ayuda").text(nuevo
                    ? "Contraseña inicial: al menos 8 caracteres. Désela a la persona y pídale que la cambie en Mi Perfil."
                    : "Escriba una nueva solo si quiere restablecerla; déjela vacía para no cambiarla.");
            });
        })
    }
    return obj;
}());
