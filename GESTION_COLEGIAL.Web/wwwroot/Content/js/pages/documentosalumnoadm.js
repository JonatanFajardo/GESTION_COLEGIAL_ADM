var DocumentosAlumnoAdm = (function () {
    var obj = {};
    var urlList, urlPend, urlUpdate;

    obj.init = function (ul, up, uu, urlDD) {
        urlList = ul; urlPend = up; urlUpdate = uu;

        $.getJSON(urlDD, function (data) {
            var sel = $("#f-alu");
            $.each(data, function (_, a) { sel.append('<option value="' + a.id + '">' + a.texto + '</option>'); });
        });

        $("#btn-cargar").on("click", function () {
            var id = $("#f-alu").val();
            if (!id) { alert("Seleccione un alumno."); return; }
            cargar(urlList + "?Alu_Id=" + id);
        });

        $("#btn-pendientes").on("click", function () { cargar(urlPend); });

        $(document).on("click", ".btn-actualizar", function () {
            var tr  = $(this).closest("tr");
            var id  = tr.data("id");
            var est = tr.find(".sel-estado").val();
            var obs = tr.find(".txt-obs").val();
            $.post(urlUpdate, { Doc_Id: id, Doc_Estado: est, Doc_Observacion: obs }, function (res) {
                if (res && res.success) $(this).text("✓").removeClass("btn-warning").addClass("btn-secondary");
            }.bind(this));
        });
    };

    function cargar(url) {
        $.getJSON(url, function (res) {
            var data = res.data || res || [];
            var tbody = $("#tbody-docs").empty();
            if (!data.length) {
                tbody.append('<tr><td colspan="4" class="text-center text-muted">Sin documentos.</td></tr>');
                return;
            }
            $.each(data, function (_, r) {
                var estados = ["Pendiente","Entregado","Rechazado"].map(function(e) {
                    return '<option' + (r.Doc_Estado === e ? ' selected' : '') + '>' + e + '</option>';
                }).join("");
                tbody.append('<tr data-id="' + r.Doc_Id + '">' +
                    '<td>' + r.Doc_Nombre + '</td>' +
                    '<td><select class="form-control sel-estado">' + estados + '</select></td>' +
                    '<td><input class="form-control txt-obs" value="' + (r.Doc_Observacion || '') + '"></td>' +
                    '<td><button class="btn btn-sm btn-warning btn-actualizar">Guardar</button></td></tr>');
            });
        });
    }

    return obj;
}());
