var AlumnosBusqueda = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-alumnosbusqueda").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Alu_NombreCompleto", title: "Alumno" },
            { data: "Sec_Descripcion", title: "Sección" },
            { data: "Cur_Nombre", title: "Curso" }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25
        });

        $("#btn-filtrar").on("click", function () {
        var Termino = $("#filter-Termino").val();
        table.ajax.reload();
        });
    };

    return obj;
}());
