var PiramideMatricula = (function () {
    var obj = {};
    var table;
    var urlList;

    obj.init = function (url, anioDefault) {
        urlList = url;

        if (anioDefault && !$("#filter-Anio").val()) {
            $("#filter-Anio").val(anioDefault);
        }

        table = $("#datatable-piramidematricula").DataTable({
            ajax: {
                url: urlList,
                dataSrc: "data",
                data: function () {
                    return {
                        Anio: $("#filter-Anio").val() || null
                    };
                }
            },
            columns: [
                { data: "Niv_Descripcion", title: "Nivel" },
                { data: "Cur_Nombre", title: "Curso" },
                { data: "TotalMasculino", title: "Masculino" },
                { data: "TotalFemenino", title: "Femenino" },
                { data: "TotalOtro", title: "Otro" },
                { data: "TotalGeneral", title: "Total" }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            responsive: true,
            pageLength: 25,
            paging: false,
            searching: false,
            info: false,
            footerCallback: function (row, data) {
                var api = this.api();
                var totalM = api.column(2).data().reduce(function (a, b) { return (parseFloat(a) || 0) + (parseFloat(b) || 0); }, 0);
                var totalF = api.column(3).data().reduce(function (a, b) { return (parseFloat(a) || 0) + (parseFloat(b) || 0); }, 0);
                var totalO = api.column(4).data().reduce(function (a, b) { return (parseFloat(a) || 0) + (parseFloat(b) || 0); }, 0);
                var total  = api.column(5).data().reduce(function (a, b) { return (parseFloat(a) || 0) + (parseFloat(b) || 0); }, 0);
                $(api.column(0).footer()).html("<strong>Total general</strong>");
                $(api.column(2).footer()).html("<strong>" + totalM + "</strong>");
                $(api.column(3).footer()).html("<strong>" + totalF + "</strong>");
                $(api.column(4).footer()).html("<strong>" + totalO + "</strong>");
                $(api.column(5).footer()).html("<strong>" + total + "</strong>");
            }
        });

        $("#btn-filtrar").on("click", function () {
            table.ajax.reload();
        });
    };

    return obj;
}());
