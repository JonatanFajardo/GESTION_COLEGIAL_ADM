var AgingCuentas = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-agingcuentas").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Alu_NombreCompleto", title: "Alumno" },
            { data: "Tramo", title: "Tramo" },
            { data: "Total", title: "Monto" },
            { data: "Cantidad", title: "Cantidad" }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25
        });

        $("#btn-filtrar").on("click", function () {

        });
    };

    return obj;
}());
