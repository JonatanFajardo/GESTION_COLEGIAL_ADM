var NotasAdm = (function () {
    var obj = {};
    var urlList, urlGuardar;
    var contexto = {};

    function llenarSelect(sel, items, textoDefault) {
        sel.empty().append('<option value="">' + textoDefault + '</option>');
        $.each(items, function (_, item) {
            sel.append('<option value="' + item.id + '">' + item.texto + '</option>');
        });
    }

    obj.init = function (ul, ug, urlDropdowns) {
        urlList   = ul;
        urlGuardar = ug;

        $.getJSON(urlDropdowns, function (data) {
            llenarSelect($("#f-hor"), data.horarios,  "-- Seleccione Horario --");
            llenarSelect($("#f-par"), data.parciales, "-- Seleccione Parcial --");
        });

        $("#btn-cargar").on("click", function () {
            contexto = {
                Hor_Id: $("#f-hor").val(),
                Par_Id: $("#f-par").val()
            };
            if (!contexto.Hor_Id || !contexto.Par_Id) {
                alert("Seleccione el horario y el parcial.");
                return;
            }
            $.getJSON(urlList, contexto, function (data) {
                var tbody = $("#tbody-notas").empty();
                if (!data || data.length === 0) {
                    tbody.append('<tr><td colspan="3" class="text-center text-muted">Sin alumnos en este horario.</td></tr>');
                    return;
                }
                $.each(data, function (_, row) {
                    var btn   = '<button class="btn btn-sm btn-success btn-guardar"' +
                                ' data-id="'  + (row.Not_Id || 0) +
                                '" data-alu="' + row.Alu_Id +
                                '" data-sec="' + row.Sec_Id +
                                '" data-mat="' + row.Mat_Id +
                                '" data-sem="' + row.Sem_Id + '">Guardar</button>';
                    var input = '<input type="number" class="form-control nota-input" step="0.01" min="0" max="100" value="' + (row.Not_Nota || "") + '">';
                    tbody.append("<tr><td>" + row.Alu_NombreCompleto + "</td><td>" + input + "</td><td>" + btn + "</td></tr>");
                });
            });
        });

        $(document).on("click", ".btn-guardar", function () {
            var btn  = $(this);
            var nota = btn.closest("tr").find(".nota-input").val();
            $.post(urlGuardar, {
                Not_Id:   btn.data("id"),
                Alu_Id:   btn.data("alu"),
                Sec_Id:   btn.data("sec"),
                Mat_Id:   btn.data("mat"),
                Sem_Id:   btn.data("sem"),
                Par_Id:   contexto.Par_Id,
                Not_Nota: nota,
                Not_Año:  new Date().getFullYear() + "-01-01"
            }, function (res) {
                if (res && res.success) {
                    btn.removeClass("btn-success").addClass("btn-secondary").text("Guardado");
                }
            });
        });
    };

    return obj;
}());
