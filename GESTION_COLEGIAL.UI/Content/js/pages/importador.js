var Importador = (function () {
    var obj = {};
    var urlPreview;

    obj.init = function (urlPrev) {
        urlPreview = urlPrev;

        // Mostrar nombre del archivo seleccionado
        $("#archivo-csv").on("change", function () {
            var fileName = $(this).val().split("\\").pop();
            $(this).closest(".custom-file").find(".custom-file-label").text(
                fileName || "Seleccionar archivo..."
            );
            // Ocultar preview anterior
            $("#preview-table").hide();
        });

        // Vista previa
        $("#btn-preview").on("click", function () {
            var fileInput = document.getElementById("archivo-csv");
            if (!fileInput.files || fileInput.files.length === 0) {
                alert("Por favor seleccione un archivo CSV.");
                return;
            }

            var formData = new FormData(document.getElementById("form-importar"));

            $.ajax({
                url:         urlPreview,
                type:        "POST",
                data:        formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (!res.success) {
                        alert(res.message || "Error al procesar el archivo.");
                        return;
                    }
                    if (!res.filas || res.filas.length === 0) {
                        alert("El archivo no contiene datos.");
                        return;
                    }
                    renderPreview(res.filas);
                },
                error: function () {
                    alert("Error al enviar el archivo al servidor.");
                }
            });
        });
    };

    function renderPreview(filas) {
        var keys = Object.keys(filas[0]);

        // Encabezados
        var ths = keys.map(function (k) { return "<th>" + k + "</th>"; }).join("");
        $("#preview-thead").html("<tr>" + ths + "</tr>");

        // Filas
        var rows = filas.map(function (fila) {
            var tds = keys.map(function (k) {
                return "<td>" + (fila[k] !== undefined ? fila[k] : "") + "</td>";
            }).join("");
            return "<tr>" + tds + "</tr>";
        }).join("");
        $("#preview-tbody").html(rows);

        $("#preview-table").show();
    }

    return obj;
}());
