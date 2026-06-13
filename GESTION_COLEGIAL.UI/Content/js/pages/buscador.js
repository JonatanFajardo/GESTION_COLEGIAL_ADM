var Buscador = (function () {
    var obj = {};
    var urlSearch;

    var tipoLabels = {
        "Alumno":   { icon: "fa-user-graduate", color: "#4361ee" },
        "Empleado": { icon: "fa-briefcase",      color: "#805dca" }
    };

    obj.init = function (url) {
        urlSearch = url;

        // Botón buscar
        $("#btn-buscar").on("click", function () {
            _buscar();
        });

        // Enter en el input
        $("#filter-q").on("keypress", function (e) {
            if (e.which === 13) _buscar();
        });

        // Atajo Ctrl+K global para enfocar el input
        $(document).on("keydown", function (e) {
            if (e.ctrlKey && e.key === "k") {
                e.preventDefault();
                $("#filter-q").focus().select();
            }
        });
    };

    function _buscar() {
        var q = $("#filter-q").val().trim();

        if (q.length < 2) {
            toastr.warning("Ingresa al menos 2 caracteres para buscar.", "Buscador");
            return;
        }

        var tbody = $("#buscador-tbody");
        tbody.html('<tr><td colspan="4" class="text-center"><i class="fa-solid fa-spinner fa-spin me-2"></i>Buscando...</td></tr>');

        $.ajax({
            url: urlSearch,
            type: "GET",
            data: { q: q },
            success: function (resp) {
                var items = resp.data || [];
                _renderResultados(items);
            },
            error: function () {
                tbody.html('<tr><td colspan="4" class="text-center text-danger"><i class="fa-solid fa-circle-exclamation me-2"></i>Error al realizar la búsqueda.</td></tr>');
                toastr.error("No se pudo conectar con el servidor.", "Buscador");
            }
        });
    }

    function _renderResultados(items) {
        var tbody = $("#buscador-tbody");

        if (!items || items.length === 0) {
            tbody.html('<tr><td colspan="4" class="text-center text-muted"><i class="fa-solid fa-face-meh me-2"></i>Sin resultados para la búsqueda.</td></tr>');
            return;
        }

        var html = "";
        $.each(items, function (i, item) {
            var info    = tipoLabels[item.Tipo] || { icon: "fa-circle-question", color: "#6c757d" };
            var detalle = item.Detalle ? '<span class="text-muted small">' + _escapeHtml(item.Detalle) + '</span>' : "—";

            html += '<tr>';
            html += '<td><span style="color:' + info.color + ';"><i class="fa-solid ' + info.icon + ' me-1"></i>' + _escapeHtml(item.Tipo) + '</span></td>';
            html += '<td>' + _escapeHtml(item.Nombre) + '</td>';
            html += '<td>' + _escapeHtml(item.Identificador || "—") + '</td>';
            html += '<td>' + detalle + '</td>';
            html += '</tr>';
        });

        tbody.html(html);
    }

    function _escapeHtml(str) {
        if (str == null) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    return obj;
}());
