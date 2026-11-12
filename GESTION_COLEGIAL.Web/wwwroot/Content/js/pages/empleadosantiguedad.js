var EmpleadosAntiguedad = (function () {
    var obj = {};
    var table;

    // Filtro por rango de años de antigüedad (columna índice 3 = "Anos").
    // Se combina con el buscador global de DataTables (nombre/cargo) en cada draw().
    $.fn.dataTable.ext.search.push(function (settings, data) {
        if (settings.nTable.id !== "datatable-empleadosantiguedad") {
            return true;
        }

        var anios = parseFloat(data[3]) || 0;
        var min = parseFloat($("#filtro-anios-min").val());
        var max = parseFloat($("#filtro-anios-max").val());

        if (!isNaN(min) && anios < min) return false;
        if (!isNaN(max) && anios > max) return false;
        return true;
    });

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
            table.search($("#filtro-texto").val() || "").draw();
        });

        $("#filtro-texto, #filtro-anios-min, #filtro-anios-max").on("keypress", function (e) {
            if (e.which === 13) {
                e.preventDefault();
                $("#btn-filtrar").click();
            }
        });

        $("#btn-limpiar-filtro").on("click", function () {
            $("#filtro-texto").val("");
            $("#filtro-anios-min").val("");
            $("#filtro-anios-max").val("");
            table.search("").draw();
        });
    };

    return obj;
}());
