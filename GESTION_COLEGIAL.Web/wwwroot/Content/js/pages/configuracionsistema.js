var ConfiguracionSistema = (function () {
    var obj = {};
    var table;
    var urlUpdate;

    obj.init = function (urlList, urlUpd) {
        urlUpdate = urlUpd;

        table = $("#datatable-configuracion").DataTable({
            ajax: { url: urlList, dataSrc: "data" },
            columns: [
                { data: "Con_Grupo",      title: "Grupo" },
                { data: "Con_Clave",      title: "Clave" },
                { data: "Con_Descripcion",title: "Descripción" },
                { data: "Con_Valor",      title: "Valor" },
                {
                    data: null,
                    title: "Acción",
                    orderable: false,
                    render: function (data, type, row) {
                        return '<button class="btn btn-sm btn-warning btn-editar" ' +
                               'data-id="' + row.Con_Id + '" ' +
                               'data-valor="' + (row.Con_Valor || "") + '" ' +
                               'data-clave="' + row.Con_Clave + '">' +
                               '<i class="fa fa-edit"></i> Editar</button>';
                    }
                }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25,
            order: [[0, "asc"]]
        });

        // Abrir modal al hacer clic en Editar
        $("#datatable-configuracion").on("click", ".btn-editar", function () {
            var id    = $(this).data("id");
            var valor = $(this).data("valor");
            var clave = $(this).data("clave");
            $("#edit-id").val(id);
            $("#edit-valor").val(valor);
            $("#edit-label").text(clave);
            $("#modalEditar").modal("show");
        });

        // Guardar cambio
        $("#btn-guardar").on("click", function () {
            var id    = $("#edit-id").val();
            var valor = $("#edit-valor").val().trim();

            $.ajax({
                url:  urlUpdate,
                type: "POST",
                data: { Con_Id: id, Con_Valor: valor },
                success: function (res) {
                    $("#modalEditar").modal("hide");
                    if (res && res.success) {
                        toastr && toastr.success("Configuración actualizada.");
                    }
                    table.ajax.reload(null, false);
                },
                error: function () {
                    toastr && toastr.error("Ocurrió un error al guardar.");
                }
            });
        });
    };

    return obj;
}());
