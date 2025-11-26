var EstadoCuenta = (function () {
    var obj = {};
    var urls = {};
    var alumnoId = 0;
    var cargosPendientesData = [];
    var historicoPagosData = [];
    var pagoIdActual = 0;

    obj.init = function (config) {
        urls = config;
        alumnoId = config.alumnoId;

        $(function () {
            initializeAlumnoSearch();
            initializeDetailEvents();

            // Si ya viene con alumnoId, cargar directamente
            if (alumnoId > 0) {
                cargarEstadoCuenta();
                $("#panel-busqueda").hide();
                $("#card-placeholder").hide();
                $("#paneles-informacion").show();
                $("#panel-info-alumno").show();
            }

            $("#btn-registrar-pago").click(function() {
                if (alumnoId > 0) {
                    window.location.href = "/Pagos/NuevoPago?alumnoId=" + alumnoId;
                } else {
                    toastr.warning("Debe seleccionar un alumno primero");
                }
            });

            $("#btn-cambiar-alumno").click(function() {
                limpiarEstadoCuenta();
                $("#panel-info-alumno").hide();
                $("#paneles-informacion").hide();
                $("#card-placeholder").show();
                $("#panel-busqueda").show();
                $("#alumno-identidad-search").val("").focus();
            });
        });
    }

    function initializeDetailEvents() {
        // Evento para ver detalle de cargo
        $(document).on("click", ".btn-ver-cargo", function() {
            var index = $(this).data("index");
            mostrarDetalleCargo(cargosPendientesData[index]);
        });

        // Evento para ver detalle de pago
        $(document).on("click", ".btn-ver-pago", function() {
            var index = $(this).data("index");
            mostrarDetallePago(historicoPagosData[index]);
        });

        // Evento para ver recibo desde modal
        $("#btn-ver-recibo-modal").click(function() {
            if (pagoIdActual > 0) {
                verRecibo(pagoIdActual);
            }
        });
    }

    function mostrarDetalleCargo(cargo) {
        $("#detail-cargo-id").text(cargo.cuentaCobrarId);
        $("#detail-cargo-concepto").text(cargo.conceptoPago);
        $("#detail-cargo-monto-original").text("L. " + formatMoney(cargo.montoOriginal));
        $("#detail-cargo-descuento").text("L. " + formatMoney(cargo.montoDescuento));
        $("#detail-cargo-mora").text("L. " + formatMoney(cargo.montoMora));
        $("#detail-cargo-total").text("L. " + formatMoney(cargo.montoTotal));
        $("#detail-cargo-fecha-venc").text(formatDate(cargo.fechaVencimiento));
        $("#detail-cargo-estado").html("<span class='badge badge-" + getEstadoClass(cargo.estadoPago) + "'>" + cargo.estadoPago + "</span>");
        $("#detail-cargo-fecha-creacion").text(cargo.fechaCreacion ? formatDate(cargo.fechaCreacion) : "N/A");
        $("#detail-cargo-observaciones").text(cargo.observaciones || "Sin observaciones");

        $("#detail-cargo-modal").modal("show");
    }

    function mostrarDetallePago(pago) {
        pagoIdActual = pago.pagoId;

        $("#detail-pago-id").text(pago.pagoId);
        $("#detail-pago-fecha").text(formatDate(pago.fechaPago));
        $("#detail-pago-monto").text("L. " + formatMoney(pago.montoTotal));
        $("#detail-pago-forma").text(pago.formaPago);
        $("#detail-pago-referencia").text(pago.numeroReferencia || "N/A");
        $("#detail-pago-recibo").text(pago.numeroRecibo || "N/A");
        $("#detail-pago-usuario").text(pago.usuario);
        $("#detail-pago-estado").html("<span class='badge badge-success'>Completado</span>");
        $("#detail-pago-observaciones").text(pago.observaciones || "Sin observaciones");

        // Cargar conceptos pagados
        var htmlConceptos = "";
        if (pago.conceptos && pago.conceptos.length > 0) {
            pago.conceptos.forEach(function(concepto) {
                htmlConceptos += "<tr>";
                htmlConceptos += "<td>" + concepto.nombre + "</td>";
                htmlConceptos += "<td>L. " + formatMoney(concepto.monto) + "</td>";
                htmlConceptos += "</tr>";
            });
        } else {
            htmlConceptos = "<tr><td colspan='2' class='text-center'>Sin detalles disponibles</td></tr>";
        }
        $("#tbody-conceptos-pago").html(htmlConceptos);

        $("#detail-pago-modal").modal("show");
    }

    function verRecibo(pagoId) {
        window.open(urls.urlRecibo + "?pagoId=" + pagoId, "_blank");
    }

    function initializeAlumnoSearch() {
        $("#btn-buscar-alumno").click(function() {
            var identidad = $("#alumno-identidad-search").val();

            if (!identidad) {
                toastr.warning("Por favor ingrese el número de identidad del alumno");
                return;
            }

            if (identidad.length !== 13) {
                toastr.warning("El número de identidad debe tener 13 dígitos");
                return;
            }

            buscarAlumnoPorIdentidad(identidad);
        });

        // Permitir buscar con Enter
        $("#alumno-identidad-search").keypress(function(e) {
            if (e.which === 13) {
                $("#btn-buscar-alumno").click();
            }
        });

        // Validar solo números
        $("#alumno-identidad-search").on('input', function() {
            this.value = this.value.replace(/[^0-9]/g, '');
        });
    }

    function buscarAlumnoPorIdentidad(identidad) {
        $.ajax({
            url: urls.urlFindAlumnoByIdentidad,
            data: { identidad: identidad },
            type: "GET",
            beforeSend: function() {
                $("#btn-buscar-alumno").prop("disabled", true).html('<i class="mdi mdi-loading mdi-spin"></i> Buscando...');
            },
            success: function(response) {
                console.log("Response completo:", response);
                console.log("response.data:", response.data);

                if (response && response.data) {
                    var alumno = response.data;
                    console.log("Alumno encontrado:", alumno);

                    if (alumno.Alu_Id) {
                        alumnoId = alumno.Alu_Id;

                        // Cargar información del alumno en el panel
                        var nombreCompleto = (alumno.Per_PrimerNombre || "") + " " +
                                            (alumno.Per_SegundoNombre || "") + " " +
                                            (alumno.Per_ApellidoPaterno || "") + " " +
                                            (alumno.Per_ApellidoMaterno || "");

                        $("#alumno-nombre").text(nombreCompleto.trim() || "N/A");
                        $("#alumno-identidad").text(alumno.Per_Identidad || "N/A");
                        $("#alumno-nivel").text(alumno.Niv_Descripcion || "N/A");
                        $("#alumno-curso").text(alumno.Cur_Descripcion || "N/A");
                        $("#alumno-seccion").text(alumno.Sec_Descripcion || "N/A");
                        $("#alumno-encargado").text("Por implementar");

                        $("#panel-busqueda").hide();
                        $("#card-placeholder").hide();
                        $("#paneles-informacion").show();
                        $("#panel-info-alumno").show();

                        toastr.success("Alumno encontrado correctamente");

                        // Cargar estado de cuenta del alumno
                        cargarEstadoCuenta();
                    } else {
                        console.log("No se encontró Alu_Id en los datos");
                        toastr.warning("No se encontró ningún alumno con el número de identidad especificado");
                    }
                } else {
                    console.log("Respuesta inválida o sin datos");
                    toastr.warning("No se encontró ningún alumno con el número de identidad especificado");
                }
            },
            error: function(xhr) {
                console.log("Error en AJAX:", xhr);
                var errorMsg = "Error al buscar el alumno. Por favor intente nuevamente.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                }
                toastr.error(errorMsg);
            },
            complete: function() {
                $("#btn-buscar-alumno").prop("disabled", false).html('<i class="mdi mdi-magnify"></i> Buscar');
            }
        });
    }

    function limpiarEstadoCuenta() {
        alumnoId = 0;
        $("#alumno-nombre").text("");
        $("#alumno-identidad").text("");
        $("#alumno-nivel").text("");
        $("#alumno-curso").text("");
        $("#alumno-seccion").text("");
        $("#alumno-encargado").text("");
        $("#total-deuda").text("L. 0.00");
        $("#total-pagado").text("L. 0.00");
        $("#total-pendiente").text("L. 0.00");
        $("#total-mora").text("L. 0.00");
        $("#tbody-cargos-pendientes").html("");
        $("#tbody-historico-pagos").html("");

        // Ocultar paneles de información
        $("#paneles-informacion").hide();
        $("#panel-info-alumno").hide();

        // Mostrar card placeholder
        $("#card-placeholder").show();
    }

    function cargarEstadoCuenta() {
        // Cargar los tres componentes del estado de cuenta por separado
        cargarResumenFinanciero();
        cargarCargosPendientesAjax();
        cargarHistoricoPagosAjax();
    }

    function cargarResumenFinanciero() {
        $.ajax({
            url: urls.urlResumenFinanciero,
            type: "GET",
            data: { alumnoId: alumnoId },
            success: function(response) {
                console.log("Response ResumenFinanciero:", response);
                if (response && response.data) {
                    var resumen = response.data;
                    $("#total-deuda").text("L. " + formatMoney(resumen.totalDeuda || 0));
                    $("#total-pagado").text("L. " + formatMoney(resumen.totalPagado || 0));
                    $("#total-pendiente").text("L. " + formatMoney(resumen.totalPendiente || 0));
                    $("#total-mora").text("L. " + formatMoney(resumen.totalMora || 0));
                } else {
                    console.log("No se pudo cargar el resumen financiero");
                }
            },
            error: function(xhr, status, error) {
                console.log("Error al cargar resumen financiero:", xhr, status, error);
                toastr.warning("No se pudo cargar el resumen financiero");
            }
        });
    }

    function cargarCargosPendientesAjax() {
        $.ajax({
            url: urls.urlCargosPendientes,
            type: "GET",
            data: { alumnoId: alumnoId },
            success: function(response) {
                console.log("Response CargosPendientes:", response);
                if (response && response.data && response.data.length > 0) {
                    cargarCargosPendientes(response.data);
                } else {
                    console.log("No hay cargos pendientes");
                    var nombreAlumno = $("#alumno-nombre").text() || "el alumno actual";
                    $("#tbody-cargos-pendientes").html(
                        "<tr><td colspan='8' class='text-center py-4'>" +
                        "<i class='mdi mdi-check-circle text-success' style='font-size: 48px;'></i>" +
                        "<p class='mt-3 mb-0'><strong>¡Excelente!</strong></p>" +
                        "<p class='text-muted'>No hay cargos pendientes para " + nombreAlumno + "</p>" +
                        "</td></tr>"
                    );
                }
            },
            error: function(xhr, status, error) {
                console.log("Error al cargar cargos pendientes:", xhr, status, error);
                toastr.warning("No se pudieron cargar los cargos pendientes");
            }
        });
    }

    function cargarHistoricoPagosAjax() {
        $.ajax({
            url: urls.urlHistoricoPagos,
            type: "GET",
            data: { alumnoId: alumnoId },
            success: function(response) {
                console.log("Response HistoricoPagos:", response);
                if (response && response.data && response.data.length > 0) {
                    cargarHistoricoPagos(response.data);
                } else {
                    console.log("No hay histórico de pagos");
                    var nombreAlumno = $("#alumno-nombre").text() || "el alumno actual";
                    $("#tbody-historico-pagos").html(
                        "<tr><td colspan='6' class='text-center py-4'>" +
                        "<i class='mdi mdi-information-outline text-info' style='font-size: 48px;'></i>" +
                        "<p class='mt-3 mb-0'><strong>Sin registros</strong></p>" +
                        "<p class='text-muted'>No hay pagos registrados para " + nombreAlumno + "</p>" +
                        "</td></tr>"
                    );
                }
            },
            error: function(xhr, status, error) {
                console.log("Error al cargar histórico de pagos:", xhr, status, error);
                toastr.warning("No se pudo cargar el histórico de pagos");
            }
        });
    }

    function cargarCargosPendientes(cargos) {
        cargosPendientesData = cargos;

        if (!cargos || cargos.length === 0) {
            var nombreAlumno = $("#alumno-nombre").text() || "el alumno actual";
            $("#tbody-cargos-pendientes").html(
                "<tr><td colspan='8' class='text-center py-4'>" +
                "<i class='mdi mdi-check-circle text-success' style='font-size: 48px;'></i>" +
                "<p class='mt-3 mb-0'><strong>¡Excelente!</strong></p>" +
                "<p class='text-muted'>No hay cargos pendientes para " + nombreAlumno + "</p>" +
                "</td></tr>"
            );
            return;
        }

        var html = "";
        cargos.forEach(function(cargo, index) {
            html += "<tr>";
            html += "<td>" + cargo.conceptoPago + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoOriginal) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoDescuento) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoMora) + "</td>";
            html += "<td>L. " + formatMoney(cargo.montoTotal) + "</td>";
            html += "<td>" + formatDate(cargo.fechaVencimiento) + "</td>";
            html += "<td><span class='badge badge-" + getEstadoClass(cargo.estadoPago) + "'>" + cargo.estadoPago + "</span></td>";
            html += "<td>";
            html += "<button class='btn btn-sm btn-primary btn-ver-cargo' data-index='" + index + "' title='Ver detalles'><i class='mdi mdi-eye'></i></button> ";
            html += "<button class='btn btn-sm btn-warning btn-aplicar-descuento' data-id='" + cargo.cuentaCobrarId + "' title='Aplicar descuento'><i class='mdi mdi-tag-outline'></i></button> ";
            html += "<button class='btn btn-sm btn-info btn-calcular-mora' data-id='" + cargo.cuentaCobrarId + "' title='Calcular mora'><i class='mdi mdi-calculator'></i></button>";
            html += "</td>";
            html += "</tr>";
        });
        $("#tbody-cargos-pendientes").html(html);
    }

    function cargarHistoricoPagos(pagos) {
        historicoPagosData = pagos;

        if (!pagos || pagos.length === 0) {
            var nombreAlumno = $("#alumno-nombre").text() || "el alumno actual";
            $("#tbody-historico-pagos").html(
                "<tr><td colspan='6' class='text-center py-4'>" +
                "<i class='mdi mdi-information-outline text-info' style='font-size: 48px;'></i>" +
                "<p class='mt-3 mb-0'><strong>Sin registros</strong></p>" +
                "<p class='text-muted'>No hay pagos registrados para " + nombreAlumno + "</p>" +
                "</td></tr>"
            );
            return;
        }

        var html = "";
        pagos.forEach(function(pago, index) {
            html += "<tr>";
            html += "<td>" + formatDate(pago.fechaPago) + "</td>";
            html += "<td>L. " + formatMoney(pago.montoTotal) + "</td>";
            html += "<td>" + pago.formaPago + "</td>";
            html += "<td>" + (pago.numeroReferencia || "N/A") + "</td>";
            html += "<td>" + pago.usuario + "</td>";
            html += "<td>";
            html += "<button class='btn btn-sm btn-primary btn-ver-pago' data-index='" + index + "' title='Ver detalles'><i class='mdi mdi-eye'></i></button> ";
            html += "<button class='btn btn-sm btn-secondary btn-ver-recibo' data-id='" + pago.pagoId + "' title='Ver recibo'><i class='mdi mdi-receipt'></i></button>";
            html += "</td>";
            html += "</tr>";
        });
        $("#tbody-historico-pagos").html(html);

        // Evento para ver recibo
        $(".btn-ver-recibo").off("click").on("click", function() {
            var pagoId = $(this).data("id");
            verRecibo(pagoId);
        });
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
