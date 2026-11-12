var MorosidadPorNivel = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-morosidadpornivel").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Niv_Descripcion", title: "Nivel" },
            { data: "TotalAlumnos", title: "Alumnos" },
            { data: "MontoPendiente", title: "Pendiente" }
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25
        });

        $("#btn-filtrar").on("click", function () {

        });
    };

    return obj;
}());
