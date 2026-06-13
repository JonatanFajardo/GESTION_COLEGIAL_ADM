var SesionesActivas = (function () {
    var obj = {};
    var table;
    var _url;
    var _timer;
    var INTERVALO_MS = 30000;

    obj.init = function (url) {
        _url = url;

        table = $("#datatable-sesionesactivas").DataTable({
            ajax: {
                url: _url,
                dataSrc: "data"
            },
            columns: [
                { data: "Usu_Name", title: "Usuario" },
                { data: "Rol_Descripcion", title: "Rol" },
                {
                    data: "Ses_FechaInicio", title: "Inicio de sesión",
                    render: function (data) {
                        if (!data) return "-";
                        return new Date(data).toLocaleString("es-HN");
                    }
                },
                {
                    data: "Ses_UltimaActividad", title: "Última actividad",
                    render: function (data) {
                        if (!data) return "-";
                        return new Date(data).toLocaleString("es-HN");
                    }
                },
                {
                    data: "MinutosInactivo", title: "Inactividad (min)",
                    className: "text-center",
                    render: function (data) {
                        var min = parseInt(data) || 0;
                        var cls = min < 5 ? "success" : min < 15 ? "warning" : "danger";
                        return '<span class="badge badge-' + cls + '">' + min + " min</span>";
                    }
                },
                { data: "Ses_IP", title: "IP" },
                {
                    data: "MinutosInactivo", title: "Estado",
                    className: "text-center",
                    render: function (data) {
                        var min = parseInt(data) || 0;
                        if (min < 15) {
                            return '<span class="badge badge-success"><i class="fa-solid fa-circle-dot"></i> Activo</span>';
                        } else {
                            return '<span class="badge badge-secondary"><i class="fa-solid fa-moon"></i> Inactivo</span>';
                        }
                    }
                }
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
            order: [[2, "desc"]],
            pageLength: 25,
            responsive: true,
            drawCallback: function () {
                _actualizarKPIs();
            }
        });

        $("#btn-refresh").on("click", function () {
            _recargar();
        });

        _iniciarTimer();
    };

    function _actualizarKPIs() {
        var data = table.rows().data().toArray();

        var total   = data.length;
        var activos = 0;
        var ipsSet  = {};

        data.forEach(function (row) {
            var min = parseInt(row.MinutosInactivo) || 0;
            if (min < 15) activos++;
            if (row.Ses_IP) ipsSet[row.Ses_IP] = true;
        });

        $("#kpi-total-sesiones").text(total);
        $("#kpi-activos").text(activos);
        $("#kpi-inactivos").text(total - activos);
        $("#kpi-ips").text(Object.keys(ipsSet).length);
    }

    function _recargar() {
        table.ajax.reload(null, false);
        _actualizarFecha();
        clearInterval(_timer);
        _iniciarTimer();
    }

    function _iniciarTimer() {
        _timer = setInterval(function () {
            table.ajax.reload(null, false);
            _actualizarFecha();
        }, INTERVALO_MS);
    }

    function _actualizarFecha() {
        var ahora = new Date().toLocaleString("es-HN");
        $("#lbl-ultima-act").text("Actualizado: " + ahora);
    }

    return obj;
}());
