var Mensajes = (function () {
    var obj = {};
    var urlList, urlCreate, urlDelete;
    var table;

    obj.init = function (ul, uc, ud, urlDropdowns) {
        urlList   = ul;
        urlCreate = uc;
        urlDelete = ud;

        // Cargar dropdowns
        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                var sel = $("select[name='Alu_Id']");
                $.each(data.alumnos, function (_, a) {
                    sel.append('<option value="' + a.id + '">' + a.texto + '</option>');
                });
            });
        }

        // DataTable
        table = $("#datatable-mensajes").DataTable({
            ajax: { url: urlList, dataSrc: "data" },
            columns: [
                { data: "Alu_NombreCompleto" },
                { data: "Msg_Asunto" },
                { data: "Msg_Resumen" },
                { data: "Msg_FechaEnvio" },
                {
                    data: "Msg_Estado",
                    render: function (val) {
                        var cls = val === "Leido" ? "badge-success" : "badge-info";
                        return '<span class="badge ' + cls + '">' + val + '</span>';
                    }
                },
                {
                    data: null,
                    render: function (r) {
                        return '<button class="btn btn-sm btn-info btn-ver" '
                             + 'data-asunto="' + (r.Msg_Asunto || '').replace(/"/g, '&quot;') + '" '
                             + 'data-alumno="' + (r.Alu_NombreCompleto || '').replace(/"/g, '&quot;') + '" '
                             + 'data-fecha="'  + (r.Msg_FechaEnvio || '') + '" '
                             + 'data-estado="' + (r.Msg_Estado || '') + '" '
                             + 'data-contenido="' + (r.Msg_Resumen || '').replace(/"/g, '&quot;') + '">'
                             + '<i class="fa fa-eye"></i> Ver</button> '
                             + '<button class="btn btn-sm btn-danger btn-eliminar" data-id="' + r.Msg_Id + '">'
                             + '<i class="fa fa-trash"></i> Eliminar</button>';
                    }
                }
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" }
        });

        // Enviar nuevo mensaje
        $("#form-nuevo").on("submit", function (e) {
            e.preventDefault();
            $.post(urlCreate, $(this).serialize(), function (res) {
                if (res && !res.success && res.type) {
                    // Error
                    toastr && toastr.error(res.message || "Error al enviar el mensaje.");
                    return;
                }
                $("#modal-nuevo").modal("hide");
                $("#form-nuevo")[0].reset();
                table.ajax.reload();
                toastr && toastr.success("Mensaje enviado correctamente.");
            });
        });

        // Ver mensaje (muestra resumen en el modal de lectura)
        $(document).on("click", ".btn-ver", function () {
            var btn = $(this);
            $("#ver-asunto").text(btn.data("asunto"));
            $("#ver-alumno").text(btn.data("alumno"));
            $("#ver-fecha").text(btn.data("fecha"));
            $("#ver-estado").text(btn.data("estado"));
            $("#ver-contenido").text(btn.data("contenido"));
            $("#modal-ver").modal("show");
        });

        // Eliminar mensaje
        $(document).on("click", ".btn-eliminar", function () {
            var id = $(this).data("id");
            if (!confirm("¿Desea eliminar este mensaje?")) return;
            $.post(urlDelete, { Msg_Id: id }, function (res) {
                if (res && !res.success && res.type) {
                    toastr && toastr.error(res.message || "Error al eliminar.");
                    return;
                }
                table.ajax.reload();
                toastr && toastr.success("Mensaje eliminado.");
            });
        });

        // Limpiar modal al cerrar
        $("#modal-nuevo").on("hidden.bs.modal", function () {
            $("#form-nuevo")[0].reset();
        });
    };

    return obj;
}());
