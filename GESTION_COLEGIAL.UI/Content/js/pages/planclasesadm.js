var PlanClasesAdm = (function () {
    var obj = {};
    var urlList, urlCreate, urlEdit, urlDelete;
    var horActual = 0;

    function llenarHorarios(sel, items) {
        sel.empty().append('<option value="">-- Seleccione Horario --</option>');
        $.each(items, function (_, h) {
            sel.append('<option value="' + h.id + '">' + h.texto + '</option>');
        });
    }

    function estadoBadge(est) {
        return est === 'Impartido'
            ? '<span class="badge badge-success">Impartido</span>'
            : '<span class="badge badge-warning">Pendiente</span>';
    }

    function cargar() {
        horActual = $("#f-hor").val();
        if (!horActual) { alert("Seleccione un horario."); return; }
        $.getJSON(urlList, { Hor_Id: horActual }, function (data) {
            var rows = data || [];
            var tbody = $("#tbody-plan").empty();
            if (!rows.length) {
                tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin contenido registrado.</td></tr>');
                return;
            }
            $.each(rows, function (_, r) {
                tbody.append(
                    '<tr data-id="' + r.Pla_Id + '">' +
                    '<td>' + r.Pla_Semana + '</td>' +
                    '<td>' + r.Pla_Tema + '</td>' +
                    '<td class="text-truncate" style="max-width:200px" title="' + (r.Pla_Objetivos||'') + '">' + (r.Pla_Objetivos||'—') + '</td>' +
                    '<td>' + (r.Pla_Recursos||'—') + '</td>' +
                    '<td>' + estadoBadge(r.Pla_Estado) + '</td>' +
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

        $.getJSON(urlDD, function (data) { llenarHorarios($("#f-hor"), data.horarios); });

        $("#btn-cargar").on("click", function () { cargar(); $("#btn-nuevo").prop("disabled", false); });

        $("#btn-nuevo").on("click", function () {
            $("#modal-titulo").text("Nuevo Tema");
            $("#form-plan")[0].reset();
            $("#Pla_Id").val(0);
            $("#Hor_Id_hidden").val(horActual);
            $("#div-estado").hide();
            $("#modal-plan").modal("show");
        });

        $(document).on("click", ".btn-editar", function () {
            var r = $(this).data("row");
            $("#modal-titulo").text("Editar Tema");
            $("#Pla_Id").val(r.Pla_Id);
            $("#Hor_Id_hidden").val(r.Hor_Id);
            $("[name='Pla_Semana']").val(r.Pla_Semana);
            $("[name='Pla_Tema']").val(r.Pla_Tema);
            $("[name='Pla_Objetivos']").val(r.Pla_Objetivos);
            $("[name='Pla_Recursos']").val(r.Pla_Recursos);
            $("[name='Pla_Estado']").val(r.Pla_Estado);
            $("#div-estado").show();
            $("#modal-plan").modal("show");
        });

        $(document).on("click", ".btn-eliminar", function () {
            var id = $(this).closest("tr").data("id");
            if (!confirm("¿Eliminar este tema?")) return;
            $.post(urlDelete, { Pla_Id: id }, function (res) { if (res && res.success) cargar(); });
        });

        $("#form-plan").on("submit", function (e) {
            e.preventDefault();
            var id  = parseInt($("#Pla_Id").val());
            var url = id ? urlEdit : urlCreate;
            $.post(url, $(this).serialize(), function (res) {
                if (res && res.success) { $("#modal-plan").modal("hide"); cargar(); }
            });
        });
    };

    return obj;
}());
