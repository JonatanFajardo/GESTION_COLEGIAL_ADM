var AsistenciaAdm = (function () {
    var obj = {};
    var urlList, urlMarcar;
    var fecha, horId;

    obj.init = function (ul, um, urlDropdowns) {
        urlList   = ul;
        urlMarcar = um;

        var hoy = new Date().toISOString().split("T")[0];
        $("#f-fecha").val(hoy);

        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                var sel = $("#f-hor");
                $.each(data.horarios, function (_, h) {
                    sel.append('<option value="' + h.id + '">' + h.texto + '</option>');
                });
            });
        }

        $("#btn-cargar").on("click", cargar);
        $("#btn-guardar-todos").on("click", guardarTodos);
        $(document).on("click", ".btn-marcar", function () { guardarFila($(this).closest("tr")); });
    };

    function cargar() {
        horId = $("#f-hor").val();
        fecha = $("#f-fecha").val();
        if (!horId || !fecha) { alert("Seleccione horario y fecha."); return; }

        $.getJSON(urlList, { Hor_Id: horId, Fecha: fecha }, function (data) {
            var tbody = $("#tbody-asistencia").empty();
            if (!data || data.length === 0) {
                tbody.append('<tr><td colspan="4" class="text-center text-muted">Sin registros para este horario.</td></tr>');
                return;
            }
            $.each(data, function (_, row) {
                var estados = ["Presente","Ausente","Tardanza","Justificado"].map(function(e) {
                    var sel = row.Asi_Estado === e ? " selected" : "";
                    return "<option" + sel + ">" + e + "</option>";
                }).join("");
                tbody.append(
                    "<tr data-asi='" + (row.Asi_Id || 0) + "' data-alu='" + row.Alu_Id + "'>" +
                    "<td>" + row.Alu_NombreCompleto + "</td>" +
                    "<td><select class='form-control est-select'>" + estados + "</select></td>" +
                    "<td><input class='form-control obs-input' value='" + (row.Asi_Observacion || "") + "'></td>" +
                    "<td><button class='btn btn-sm btn-primary btn-marcar'>Guardar</button></td></tr>"
                );
            });
        });
    }

    function guardarFila(tr, cb) {
        $.post(urlMarcar, {
            Asi_Id:          tr.data("asi"),
            Hor_Id:          horId,
            Alu_Id:          tr.data("alu"),
            Asi_Fecha:       fecha,
            Asi_Estado:      tr.find(".est-select").val(),
            Asi_Observacion: tr.find(".obs-input").val()
        }, function (res) { if (cb) cb(res); });
    }

    function guardarTodos() {
        $("#tbody-asistencia tr").each(function () { guardarFila($(this)); });
    }

    return obj;
}());
