var Notificaciones = (function () {
    var obj = {};
    var urlList;
    var urlMarcar;

    var tipoClases = {
        "I": { clase: "badge-tipo-I", label: "Info" },
        "A": { clase: "badge-tipo-A", label: "Alerta" },
        "E": { clase: "badge-tipo-E", label: "Error" },
        "S": { clase: "badge-tipo-S", label: "Éxito" }
    };

    obj.init = function (urlListado, urlMarcarLeida) {
        urlList   = urlListado;
        urlMarcar = urlMarcarLeida;

        _cargarNotificaciones();

        $("#chk-solo-no-leidas").on("change", function () {
            _cargarNotificaciones();
        });
    };

    function _cargarNotificaciones() {
        var soloNoLeidas = $("#chk-solo-no-leidas").is(":checked");
        var tbody = $("#notificaciones-tbody");
        tbody.html('<tr><td colspan="6" class="text-center"><i class="fa-solid fa-spinner fa-spin me-2"></i>Cargando...</td></tr>');

        $.ajax({
            url: urlList,
            type: "GET",
            data: { soloNoLeidas: soloNoLeidas },
            success: function (resp) {
                var items = resp.data || [];
                _renderNotificaciones(items);
            },
            error: function () {
                tbody.html('<tr><td colspan="6" class="text-center text-danger"><i class="fa-solid fa-circle-exclamation me-2"></i>Error al cargar las notificaciones.</td></tr>');
                toastr.error("No se pudo conectar con el servidor.", "Notificaciones");
            }
        });
    }

    function _renderNotificaciones(items) {
        var tbody = $("#notificaciones-tbody");

        if (!items || items.length === 0) {
            tbody.html('<tr><td colspan="6" class="text-center text-muted"><i class="fa-solid fa-bell-slash me-2"></i>No hay notificaciones.</td></tr>');
            return;
        }

        var html = "";
        $.each(items, function (i, item) {
            var tipoInfo = tipoClases[item.Not_Tipo] || { clase: "badge bg-secondary", label: item.Not_Tipo };
            var rowClass = item.Not_EsLeida ? "notif-leida" : "notif-no-leida";
            var fecha    = _formatFecha(item.Not_FechaEnvio);
            var estadoBadge = item.Not_EsLeida
                ? '<span class="badge bg-secondary">Leída</span>'
                : '<span class="badge bg-primary">Nueva</span>';
            var btnMarcar = !item.Not_EsLeida
                ? '<button class="btn btn-sm btn-outline-success btn-marcar" data-id="' + item.Not_Id + '" title="Marcar como leída"><i class="fa-solid fa-check"></i></button>'
                : '<span class="text-muted">—</span>';

            html += '<tr class="' + rowClass + '">';
            html += '<td><span class="badge ' + tipoInfo.clase + ' text-white">' + _escapeHtml(tipoInfo.label) + '</span></td>';
            html += '<td>' + _escapeHtml(item.Not_Titulo) + '</td>';
            html += '<td>' + _escapeHtml(item.Not_Mensaje || "—") + '</td>';
            html += '<td>' + fecha + '</td>';
            html += '<td>' + estadoBadge + '</td>';
            html += '<td>' + btnMarcar + '</td>';
            html += '</tr>';
        });

        tbody.html(html);

        // Evento marcar como leída
        $(".btn-marcar").off("click").on("click", function () {
            var notId = $(this).data("id");
            _marcarLeida(notId, $(this));
        });
    }

    function _marcarLeida(notId, btn) {
        btn.prop("disabled", true).html('<i class="fa-solid fa-spinner fa-spin"></i>');

        $.ajax({
            url: urlMarcar,
            type: "POST",
            data: { notId: notId },
            success: function (resp) {
                if (resp.success === false) {
                    toastr.error("No se pudo marcar la notificación.", "Error");
                    btn.prop("disabled", false).html('<i class="fa-solid fa-check"></i>');
                } else {
                    toastr.success("Notificación marcada como leída.", "Listo");
                    _cargarNotificaciones();
                }
            },
            error: function () {
                toastr.error("Error al comunicarse con el servidor.", "Error");
                btn.prop("disabled", false).html('<i class="fa-solid fa-check"></i>');
            }
        });
    }

    function _formatFecha(fechaStr) {
        if (!fechaStr) return "—";
        try {
            var d = new Date(fechaStr);
            var dd = String(d.getDate()).padStart(2, "0");
            var mm = String(d.getMonth() + 1).padStart(2, "0");
            var yyyy = d.getFullYear();
            var hh = String(d.getHours()).padStart(2, "0");
            var min = String(d.getMinutes()).padStart(2, "0");
            return dd + "/" + mm + "/" + yyyy + " " + hh + ":" + min;
        } catch (e) {
            return fechaStr;
        }
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
