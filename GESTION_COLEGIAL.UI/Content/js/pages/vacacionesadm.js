var VacacionesAdm = (function () {
    var obj = {};
    var urlList, urlCreate, urlApprove;
    var table;

    obj.init = function (ul, uc, ua, urlDropdowns) {
        urlList = ul; urlCreate = uc; urlApprove = ua;

        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                var sel = $("select[name='Emp_Id']");
                $.each(data.empleados, function (_, e) {
                    sel.append('<option value="' + e.id + '">' + e.texto + '</option>');
                });
            });
        }

        table = $("#datatable-vac").DataTable({
            ajax: { url: urlList, dataSrc: "" },
            columns: [
                { data: "Emp_NombreCompleto" },
                { data: "Vac_Tipo" },
                { data: "Vac_FechaInicio" },
                { data: "Vac_FechaFin" },
                { data: "Vac_Estado" },
                { data: null, render: function (r) {
                    if (r.Vac_Estado === "Pendiente")
                        return '<button class="btn btn-sm btn-success btn-aprobar" data-id="' + r.Vac_Id + '">Aprobar</button> ' +
                               '<button class="btn btn-sm btn-danger btn-rechazar" data-id="' + r.Vac_Id + '">Rechazar</button>';
                    return '<span class="badge badge-secondary">' + r.Vac_Estado + '</span>';
                }}
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" }
        });

        $("#form-nueva").on("submit", function (e) {
            e.preventDefault();
            $.post(urlCreate, $(this).serialize(), function (res) {
                if (res && res.success) { $("#modal-nueva").modal("hide"); table.ajax.reload(); }
            });
        });

        $(document).on("click", ".btn-aprobar", function () {
            $.post(urlApprove, { Vac_Id: $(this).data("id"), Vac_Estado: "Aprobado" },
                function () { table.ajax.reload(); });
        });
        $(document).on("click", ".btn-rechazar", function () {
            $.post(urlApprove, { Vac_Id: $(this).data("id"), Vac_Estado: "Rechazado" },
                function () { table.ajax.reload(); });
        });
    };

    return obj;
}());
