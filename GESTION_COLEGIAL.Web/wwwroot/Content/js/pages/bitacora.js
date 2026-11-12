var Bitacora = (function () {
    var obj = {};
    var table;

    obj.init = function (url) {
        table = $("#datatable-bitacora").DataTable({
            ajax: { url: url, dataSrc: "" },
            columns: [
                { data: "Bit_Fecha",    title: "Fecha" },
                { data: "Usu_Nombre",   title: "Usuario" },
                { data: "Bit_Tabla",    title: "Tabla" },
                { data: "Bit_Accion",   title: "Acción" },
                { data: "Bit_Detalle",  title: "Detalle" }
            ],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            order: [[0, "desc"]],
            pageLength: 50
        });

        $("#btn-filtrar").on("click", function () {
            var u = $("#filter-usu").val();
            var t = $("#filter-tabla").val();
            var n = $("#filter-top").val();
            var qs = "?";
            if (u) qs += "Usu_Id=" + u + "&";
            if (t) qs += "Bit_Tabla=" + encodeURIComponent(t) + "&";
            if (n) qs += "Top=" + n;
            table.ajax.url(url + qs).load();
        });
    };

    return obj;
}());
