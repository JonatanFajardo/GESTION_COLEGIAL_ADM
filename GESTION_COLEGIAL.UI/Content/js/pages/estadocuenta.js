var EstadoCuenta = (function () {
    var obj = {};
    var urls = {};
    var alumnoId = 0;

    obj.init = function (config) {
        urls = config;
        alumnoId = config.alumnoId;

        $(function () {
            cargarEstadoCuenta();

            $("#btn-registrar-pago").click(function() {
                window.location.href = "/Pagos/NuevoPago?alumnoId=" + alumnoId;
            });
        });
    }

    function cargarEstadoCuenta() {
        $.ajax({
            url: urls.urlEstadoCuenta,
            type: "GET",
            data: { alumnoId: alumnoId },
            success: function(response) {
                if (response.success) {
                    var data = response.data;

                    // Información del alumno
                    $("#alumno-nombre").text(data.alumno.nombreCompleto);
                    $("#alumno-identidad").text(data.alumno.identidad);
                    $("#alumno-nivel").text(data.alumno.nivel);
                    $("#alumno-curso").text(data.alumno.curso);
                    $("#alumno-seccion").text(data.alumno.seccion);
                    $("#alumno-encargado").text(data.alumno.encargado);

                    // Resumen financiero
                    $("#total-deuda").text("L. " + formatMoney(data.resumen.totalDeuda));
                    $("#total-pagado").text("L. " + formatMoney(data.resumen.totalPagado));
                    $("#total-pendiente").text("L. " + formatMoney(data.resumen.totalPendiente));
                    $("#total-mora").text("L. " + formatMoney(data.resumen.totalMora));

                    // Cargos pendientes
                    cargarCargosPendientes(data.cargosPendientes);

                    // Histórico de pagos
                    cargarHistoricoPagos(data.historicoPagos);
                }
            }
        });
    }

    function cargarCargosPendientes(cargos) {
        var html = "";
        cargos.forEach(function(cargo) {
            html += "<tr>";
            html += "<td>" + cargo.conceptoPago + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoOriginal) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoDescuento) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoMora) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoTotal) + "</td>";
            html += "<td>" + formatDate(cargo.fechaVencimiento) + "</td>";
            html += "<td><span class='badge badge-" + getEstadoClass(cargo.estadoPago) + "'>" + cargo.estadoPago + "</span></td>";
            html += "<td>";
            html += "<button class='btn btn-sm btn-warning btn-aplicar-descuento' data-id='" + cargo.cuentaCobrarId + "'><i class='mdi mdi-tag-outline'></i></button> ";
            html += "<button class='btn btn-sm btn-info btn-calcular-mora' data-id='" + cargo.cuentaCobrarId + "'><i class='mdi mdi-calculator'></i></button>";
            html += "</td>";
            html += "</tr>";
        });
        $("#tbody-cargos-pendientes").html(html);
    }

    function cargarHistoricoPagos(pagos) {
        var html = "";
        pagos.forEach(function(pago) {
            html += "<tr>";
            html += "<td>" + formatDate(pago.fechaPago) + "</td>";
            html += "<td>L. " + formatMoney(pago.montoTotal) + "</td>";
            html += "<td>" + pago.formaPago + "</td>";
            html += "<td>" + pago.numeroReferencia + "</td>";
            html += "<td>" + pago.usuario + "</td>";
            html += "<td>";
            html += "<button class='btn btn-sm btn-secondary btn-ver-recibo' data-id='" + pago.pagoId + "'><i class='mdi mdi-receipt'></i> Recibo</button>";
            html += "</td>";
            html += "</tr>";
        });
        $("#tbody-historico-pagos").html(html);
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    function formatDate(dateString) {
        var date = new Date(dateString);
        return date.toLocaleDateString('es-HN');
    }

    function getEstadoClass(estado) {
        switch(estado.toLowerCase()) {
            case 'pendiente':
                return 'warning';
            case 'vencido':
                return 'danger';
            case 'pagado':
                return 'success';
            default:
                return 'secondary';
        }
    }

    return obj;
}());
