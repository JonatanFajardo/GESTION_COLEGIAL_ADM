var MisAlumnosAdm = (function () {
    var obj = {};

    obj.init = function (urlList, urlDD) {
        $.getJSON(urlDD, function (data) {
            var sel = $("#f-hor");
            $.each(data.horarios, function (_, h) { sel.append('<option value="' + h.id + '">' + h.texto + '</option>'); });
        });

        $("#btn-cargar").on("click", function () {
            var id = $("#f-hor").val();
            if (!id) { alert("Seleccione un horario."); return; }

            $.getJSON(urlList, { Hor_Id: id }, function (data) {
                var rows = data || [];
                var panel = $("#panel-alumnos").empty().hide();
                $("#panel-vacio").hide();

                if (!rows.length) { $("#panel-vacio").show(); return; }

                $.each(rows, function (_, a) {
                    var img = a.Per_Imagen
                        ? '<img src="' + a.Per_Imagen + '" class="rounded-circle mb-2" width="64" height="64" style="object-fit:cover;">'
                        : '<div class="rounded-circle bg-secondary d-flex align-items-center justify-content-center mb-2" style="width:64px;height:64px;font-size:24px;color:#fff;margin:0 auto;"><i class="fa-solid fa-user"></i></div>';
                    panel.append(
                        '<div class="col-6 col-md-3 col-lg-2">' +
                            '<div class="card text-center p-3 h-100">' +
                                img +
                                '<div class="fw-semibold small">' + a.NombreCompleto + '</div>' +
                                '<div class="text-muted" style="font-size:11px;">' + (a.Sec_Descripcion||'') + '</div>' +
                            '</div>' +
                        '</div>'
                    );
                });
                panel.show();
            });
        });
    };

    return obj;
}());
