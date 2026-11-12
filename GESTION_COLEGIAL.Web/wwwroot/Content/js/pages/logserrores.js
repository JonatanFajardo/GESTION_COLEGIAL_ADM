var LogsErrores = (function () {
    var obj = {};
    var table;
    var urlLeer;

    obj.init = function (urlList, urlRead) {
        urlLeer = urlRead;

        table = $("#datatable-logs").DataTable({
            ajax: { url: urlList, dataSrc: "" },
            columns: [
                { data: "Archivo",           title: "Archivo" },
                { data: "Tamano",            title: "Tamaño" },
                { data: "FechaModificacion", title: "Última modificación" },
                {
                    data: null,
                    title: "Acción",
                    orderable: false,
                    render: function (data, type, row) {
                        return '<button class="btn btn-sm btn-info btn-ver" ' +
                               'data-archivo="' + row.Archivo + '">' +
                               '<i class="fa fa-eye"></i> Ver</button>';
                    }
                }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25,
            order: [[2, "desc"]]
        });

        // Abrir modal y cargar contenido
        $("#datatable-logs").on("click", ".btn-ver", function () {
            var archivo = $(this).data("archivo");
            $("#modalLogLabel").text(archivo);
            $("#log-content").text("Cargando...");
            $("#modalLog").modal("show");

            $.ajax({
                url:      urlLeer,
                type:     "GET",
                data:     { archivo: archivo },
                dataType: "json",
                success: function (res) {
                    $("#log-content").text(res.contenido || "(sin contenido)");
                    // Scroll al final
                    var pre = document.getElementById("log-content");
                    pre.scrollTop = pre.scrollHeight;
                },
                error: function () {
                    $("#log-content").text("Error al cargar el archivo.");
                }
            });
        });
    };

    return obj;
}());
