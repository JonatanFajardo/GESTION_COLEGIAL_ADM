var EmpleadosAntiguedad = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-empleadosantiguedad").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
            { data: "Emp_NombreCompleto", title: "Empleado" },
            { data: "Cargo", title: "Cargo" },
            { data: "FechaIngreso", title: "Ingreso" },
            { data: "Anos", title: "Años" }
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
