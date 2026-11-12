var TareasAdm = (function () {
    var obj = {};
    var urlList, urlCreate, urlEdit, urlDelete;
    var horActual = 0;

    function estadoBadge(est) {
        return est === 'Cerrada'
            ? '<span class="badge badge-secondary">Cerrada</span>'
            : '<span class="badge badge-success">Activa</span>';
    }

    function cargar() {
        horActual = $("#f-hor").val();
        if (!horActual) { alert("Seleccione un horario."); return; }
        $.getJSON(urlList, { Hor_Id: horActual }, function (data) {
            var rows = data || [];
            var tbody = $("#tbody-tareas").empty();
            if (!rows.length) {
                tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin tareas registradas.</td></tr>');
                return;
            }
            $.each(rows, function (_, r) {
                var fecha = r.Tar_FechaEntrega ? r.Tar_FechaEntrega.substring(0, 10) : '—';
                tbody.append(
                    '<tr data-id="' + r.Tar_Id + '">' +
                    '<td>' + r.Tar_Titulo + '</td>' +
                    '<td class="text-truncate" style="max-width:180px" title="' + (r.Tar_Descripcion||'') + '">' + (r.Tar_Descripcion||'—') + '</td>' +
                    '<td>' + fecha + '</td>' +
                    '<td>' + r.Tar_Punteo + '</td>' +
                    '<td>' + estadoBadge(r.Tar_Estado) + '</td>' +
                    '<td>' +
                        '<button class="btn btn-xs btn-primary btn-editar me-1" data-row='' + JSON.stringify(r) + ''>Editar</button>' +
                        '<button class="btn btn-xs btn-danger btn-eliminar">Eliminar</button>' +
                    '</td></tr>'
                );
            });
        });
    }

    obj.init = function (ul, uc, ue, ud, urlDD) {
        urlList = ul; urlCreate = uc; urlEdit = ue; urlDelete = ud;

        $.getJSON(urlDD, function (data) {
            var sel = $("#f-hor").empty().append('<option value="">-- Seleccione Horario --</option>');
            $.each(data.horarios, function (_, h) { sel.append('<option value="' + h.id + '">' + h.texto + '</option>'); });
        });

        $("#btn-cargar").on("click", function () { cargar(); $("#btn-nuevo").prop("disabled", false); });

        $("#btn-nuevo").on("click", function () {
            $("#modal-titulo").text("Nueva Tarea");
            $("#form-tarea")[0].reset();
            $("#Tar_Id").val(0);
            $("#Hor_Id_hidden").val(horActual);
            $("#div-estado").hide();
            $("#modal-tarea").modal("show");
        });

        $(document).on("click", ".btn-editar", function () {
            var r = $(this).data("row");
            $("#modal-titulo").text("Editar Tarea");
            $("#Tar_Id").val(r.Tar_Id);
            $("#Hor_Id_hidden").val(r.Hor_Id);
            $("[name='Tar_Titulo']").val(r.Tar_Titulo);
            $("[name='Tar_Descripcion']").val(r.Tar_Descripcion);
            $("[name='Tar_FechaEntrega']").val(r.Tar_FechaEntrega ? r.Tar_FechaEntrega.substring(0,10) : '');
            $("[name='Tar_Punteo']").val(r.Tar_Punteo);
            $("[name='Tar_Estado']").val(r.Tar_Estado);
            $("#div-estado").show();
            $("#modal-tarea").modal("show");
        });

        $(document).on("click", ".btn-eliminar", function () {
            var id = $(this).closest("tr").data("id");
            if (!confirm("¿Eliminar esta tarea?")) return;
            $.post(urlDelete, { Tar_Id: id }, function (res) { if (res && res.success) cargar(); });
        });

        $("#form-tarea").on("submit", function (e) {
            e.preventDefault();
            var id  = parseInt($("#Tar_Id").val());
            var url = id ? urlEdit : urlCreate;
            $.post(url, $(this).serialize(), function (res) {
                if (res && res.success) { $("#modal-tarea").modal("hide"); cargar(); }
            });
        });
    };

    return obj;
}());
