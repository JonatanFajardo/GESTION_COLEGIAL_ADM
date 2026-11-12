var AlumnosEnRiesgo = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-alumnosenriesgo").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Alu_NombreCompleto", title: "Alumno" },
            { data: "Sec_Descripcion", title: "Sección" },
            { data: "Promedio", title: "Promedio" }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25
        });

        $("#btn-filtrar").on("click", function () {
        var PromedioMinimo = $("#filter-PromedioMinimo").val();
        var Anio = $("#filter-Anio").val();
        table.ajax.reload();
        });
    };

    return obj;
}());
