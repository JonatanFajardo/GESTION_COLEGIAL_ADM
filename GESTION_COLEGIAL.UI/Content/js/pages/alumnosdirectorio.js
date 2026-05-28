var AlumnosDirectorio = (function () {
    var obj = {};
    var table;
    var urlList;

    obj.init = function (url, urlCursos, urlSecciones) {
        urlList = url;

        table = $("#datatable-alumnosdirectorio").DataTable({
            ajax: {
                url: urlList,
                dataSrc: "data",
                data: function () {
                    return {
                        Cur_Id: $("#filter-Cur_Id").val() || null,
                        Sec_Id: $("#filter-Sec_Id").val() || null
                    };
                }
            },
            columns: [
                { data: "Alu_Identidad",      title: "Identidad",  defaultContent: "" },
                { data: "Alu_NombreCompleto", title: "Alumno",     defaultContent: "" },
                { data: "Cur_Nombre",         title: "Curso",      defaultContent: "" },
                { data: "Sec_Descripcion",    title: "Seccion",    defaultContent: "" },
                { data: "Enc_NombreCompleto", title: "Encargado",  defaultContent: "" },
                { data: "Enc_Telefono",       title: "Telefono",   defaultContent: "" },
                { data: "Enc_Correo",         title: "Email",      defaultContent: "" }
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25
        });

        // Cargar cursos en el dropdown
        $.ajax({
            url: urlCursos,
            type: "GET",
            success: function (response) {
                var data = response.data || [];
                var $select = $("#filter-Cur_Id");
                $select.find("option:not(:first)").remove();
                $.each(data, function (i, item) {
                    var val = item.Cun_Id || item.Cur_Id || item.id || "";
                    var txt = item.Cun_Descripcion || item.Cur_Descripcion || item.Cur_Nombre || item.descripcion || "";
                    $select.append($("<option>").val(val).text(txt));
                });
            }
        });

        // Al cambiar curso, cargar secciones filtradas
        $("#filter-Cur_Id").on("change", function () {
            var curId = $(this).val();
            var $secSelect = $("#filter-Sec_Id");
            $secSelect.find("option:not(:first)").remove();

            if (curId) {
                $.ajax({
                    url: urlSecciones,
                    type: "GET",
                    data: { Cur_Id: curId },
                    success: function (response) {
                        var data = response.data || [];
                        $.each(data, function (i, item) {
                            var val = item.Sec_Id || item.id || "";
                            var txt = item.Sec_Descripcion || item.descripcion || "";
                            $secSelect.append($("<option>").val(val).text(txt));
                        });
                    }
                });
            }
        });

        $("#btn-filtrar").on("click", function () {
            table.ajax.reload();
        });
    };

    return obj;
}());
