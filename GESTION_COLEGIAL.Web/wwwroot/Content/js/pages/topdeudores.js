var TopDeudores = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-topdeudores").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Alu_NombreCompleto", title: "Alumno" },
            { data: "TotalDeuda", title: "Deuda Total" },
            { data: "CantCuotas", title: "Cuotas" }
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
