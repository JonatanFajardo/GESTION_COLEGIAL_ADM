var NuevoPago = (function () {
    var obj = {};
    var urls = {};
    var currentStep = 1;
    var selectedAlumno = null;
    var selectedConceptos = [];
    var selectedDescuento = null;
    var selectedFormaPago = null;
    var conceptosCatalogo = [];
    var tarifasCatalogo = [];
    var MENSUALIDAD_CONCEPTO_ID = 2;

    obj.init = function (config) {
        urls = config;

        $(function () {
            initializeStepNavigation();
            initializeAlumnoSearch();
            initializeGenerarMensualidad();
            initializeAgregarCargo();
            loadDescuentos();
            loadFormasPago();
            loadConceptosCatalogo();
            loadTarifas();

            var now = new Date();
            $("#mensualidad-anio").val(now.getFullYear());
            $("#mensualidad-mes").val(now.getMonth() + 1);
            var vence = new Date();
            vence.setDate(vence.getDate() + 10);
            $("#adhoc-vence").val(toIsoDate(vence));

            // Precarga desde Cartera / Estado de Cuenta: si viene identidad,
            // busca el alumno y avanza directo al paso de conceptos.
            if (config.identidad) {
                $("#alumno-identidad-search").val(config.identidad);
                buscarAlumnoPorIdentidad(config.identidad, function () {
                    goToStep(2);
                    loadConceptosAlumno();
                });
            }
        });
    }

    function initializeStepNavigation() {
        // Step 1
        $("#btn-step1-next").click(function() {
            if (selectedAlumno) {
                goToStep(2);
                loadConceptosAlumno();
            } else {
                toastr.warning("Debe seleccionar un alumno");
            }
        });

        // Step 2
        $("#btn-step2-prev").click(function() { goToStep(1); });
        $("#btn-step2-next").click(function() {
            selectedConceptos = [];
            $(".concepto-checkbox:checked").each(function() {
                selectedConceptos.push($(this).data("concepto"));
            });
            if (selectedConceptos.length > 0) {
                goToStep(3);
                updateDescuentoTotal();
            } else {
                toastr.warning("Debe seleccionar al menos un concepto");
            }
        });

        // Step 3
        $("#btn-step3-prev").click(function() { goToStep(2); });
        $("#btn-step3-next").click(function() {
            goToStep(4);
        });

        // Step 4
        $("#btn-step4-prev").click(function() { goToStep(3); });
        $("#btn-step4-next").click(function() {
            if ($("#forma-pago-select").val()) {
                goToStep(5);
                showConfirmation();
            } else {
                toastr.warning("Debe seleccionar una forma de pago");
            }
        });

        // Step 5
        $("#btn-step5-prev").click(function() { goToStep(4); });
        $("#btn-confirmar-pago").click(function() {
            // Se abre la pestaña en blanco de forma síncrona (dentro del gesto de click)
            // para evitar que el navegador bloquee el popup cuando se setee la URL más tarde.
            var reciboWindow = window.open("", "_blank");
            confirmarPago(reciboWindow);
        });

        $("#select-all-conceptos").change(function() {
            $(".concepto-checkbox").prop("checked", $(this).is(":checked"));
            calculateSubtotal();
        });
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

    function buscarAlumnoPorIdentidad(identidad, onLoaded) {
        $.ajax({
            url: urls.urlFindAlumnoByIdentidad,
            data: { identidad: identidad },
            type: "GET",
            beforeSend: function() {
                $("#btn-buscar-alumno").prop("disabled", true).html('<i class="mdi mdi-loading mdi-spin"></i> Buscando...');
            },
            success: function(response) {
                if (response && response.data && response.data.Alu_Id) {
                    selectedAlumno = response.data;
                    mostrarDatosAlumno(response.data);
                    $("#alumno-info").show();
                    $("#btn-step1-next").prop("disabled", false);
                    if (typeof onLoaded === "function") { onLoaded(); }
                } else {
                    toastr.warning("No se encontró ningún alumno con el número de identidad especificado");
                    limpiarDatosAlumno();
                }
            },
            error: function(xhr) {
                var errorMsg = "Error al buscar el alumno. Por favor intente nuevamente.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                }
                toastr.error(errorMsg);
                limpiarDatosAlumno();
            },
            complete: function() {
                $("#btn-buscar-alumno").prop("disabled", false).html('<i class="mdi mdi-magnify"></i> Buscar');
            }
        });
    }

    function mostrarDatosAlumno(alumno) {
        // Construir nombre completo
        var nombreCompleto = alumno.Per_PrimerNombre + " " +
                           alumno.Per_SegundoNombre + " " +
                           alumno.Per_ApellidoPaterno + " " +
                           alumno.Per_ApellidoMaterno;

        $("#alumno-nombre-completo").text(nombreCompleto);
        $("#alumno-identidad").text(alumno.Per_Identidad || "N/A");

        // Formatear fecha de nacimiento
        var fechaNacimiento = new Date(alumno.Per_FechaNacimiento);
        $("#alumno-fecha-nacimiento").text(fechaNacimiento.toLocaleDateString('es-HN'));

        $("#alumno-sexo").text(alumno.Per_Sexo === 'M' ? 'Masculino' : 'Femenino');
        $("#alumno-correo").text(alumno.Per_CorreoElectronico || "N/A");
        $("#alumno-telefono").text(alumno.Per_Telefono || "N/A");
        $("#alumno-nivel").text(alumno.Niv_Descripcion || "N/A");
        $("#alumno-curso").text(alumno.Cun_Descripcion || "N/A");

        // Mostrar imagen del alumno (o placeholder con ícono)
        var placeholderHtml = '<div class="rounded-circle bg-secondary d-inline-flex align-items-center justify-content-center" style="width:140px;height:140px;font-size:64px;color:#fff;">'
                            + '<i class="fa-solid fa-user"></i>'
                            + '</div>';
        var avatarHtml;
        if (alumno.Per_Imagen && String(alumno.Per_Imagen).trim()) {
            var onerrorReplace = "this.outerHTML='" + placeholderHtml.replace(/'/g, "\\'").replace(/"/g, '&quot;') + "';";
            avatarHtml = '<img src="' + alumno.Per_Imagen + '" alt="Foto del alumno" class="img-fluid rounded-circle" '
                       + 'style="width:140px;height:140px;object-fit:cover;border:2px solid #ddd;" '
                       + 'onerror="' + onerrorReplace + '" />';
        } else {
            avatarHtml = placeholderHtml;
        }
        $("#alumno-avatar-wrapper").html(avatarHtml);
    }

    function limpiarDatosAlumno() {
        selectedAlumno = null;
        $("#alumno-info").hide();
        $("#btn-step1-next").prop("disabled", true);
        $("#alumno-nombre-completo").text("");
        $("#alumno-identidad").text("");
        $("#alumno-fecha-nacimiento").text("");
        $("#alumno-sexo").text("");
        $("#alumno-correo").text("");
        $("#alumno-telefono").text("");
        $("#alumno-nivel").text("");
        $("#alumno-curso").text("");
        $("#alumno-avatar-wrapper").html('<div class="rounded-circle bg-secondary d-inline-flex align-items-center justify-content-center" style="width:140px;height:140px;font-size:64px;color:#fff;"><i class="fa-solid fa-user"></i></div>');
    }

    function loadConceptosAlumno() {
        $.ajax({
            url: urls.urlConceptosAlumno,
            data: { alumnoId: selectedAlumno.Alu_Id },
            type: "GET",
            beforeSend: function() {
                $("#conceptos-tbody").html('<tr><td colspan="6" class="text-center"><i class="mdi mdi-loading mdi-spin"></i> Cargando conceptos...</td></tr>');
            },
            success: function(response) {
                console.log("Response conceptos:", response);
                if (response && response.data && response.data.length > 0) {
                    renderConceptos(response.data);
                } else {
                    toastr.info("No hay conceptos pendientes de pago para este alumno");
                    $("#conceptos-tbody").html('<tr><td colspan="6" class="text-center">No hay conceptos pendientes</td></tr>');
                }
            },
            error: function(xhr, status, error) {
                console.error("Error al cargar conceptos:", xhr.responseText, status, error);
                toastr.error("Error al cargar los conceptos del alumno: " + (xhr.responseJSON?.message || error));
                $("#conceptos-tbody").html('<tr><td colspan="6" class="text-center text-danger">Error al cargar conceptos</td></tr>');
            }
        });
    }

    function renderConceptos(conceptos) {
        var html = "";
        if (!conceptos || conceptos.length === 0) {
            html = '<tr><td colspan="6" class="text-center">No hay conceptos pendientes de pago</td></tr>';
        } else {
            conceptos.forEach(function(concepto) {
                html += "<tr>";
                html += '<td><input type="checkbox" class="concepto-checkbox" checked data-concepto=\'' + JSON.stringify(concepto) + '\' /></td>';
                html += "<td>" + concepto.ConceptoDescripcion + "</td>";
                html += "<td>L. " + formatMoney(concepto.MontoOriginal) + "</td>";
                html += "<td>L. " + formatMoney(concepto.MontoDescuento) + "</td>";
                html += "<td>L. " + formatMoney(concepto.MontoMora) + "</td>";
                html += "<td>L. " + formatMoney(concepto.MontoPendiente) + "</td>";
                html += "</tr>";
            });
        }
        $("#conceptos-tbody").html(html);

        $(".concepto-checkbox").change(function() {
            calculateSubtotal();
        });

        // Por defecto vienen todos los cargos pendientes seleccionados.
        var hayConceptos = $(".concepto-checkbox").length > 0;
        $("#select-all-conceptos").prop("checked", hayConceptos);
        calculateSubtotal();
    }

    function calculateSubtotal() {
        var subtotal = 0;
        $(".concepto-checkbox:checked").each(function() {
            var concepto = $(this).data("concepto");
            subtotal += parseFloat(concepto.MontoPendiente);
        });
        $("#subtotal-conceptos").text(formatMoney(subtotal));
    }

    function loadDescuentos() {
        $.ajax({
            url: urls.urlDescuentos,
            success: function(response) {
                var data = response && response.data ? response.data : (response || []);
                var html = '<option value="">-- Sin descuento --</option>';
                data.forEach(function(item) {
                    if (item.EsActivo && !item.EsEliminado) {
                        html += '<option value="' + item.DescuentoId + '">' + item.Descripcion + '</option>';
                    }
                });
                $("#descuento-select").html(html);
            }
        });
    }

    function loadFormasPago() {
        $.ajax({
            url: urls.urlFormasPago,
            success: function(response) {
                var data = response && response.data ? response.data : (response || []);
                var html = '<option value="">-- Seleccione --</option>';
                data.forEach(function(item) {
                    if (item.EsActivo && !item.EsEliminado) {
                        html += '<option value="' + item.FormaPagoId + '">' + item.Descripcion + '</option>';
                    }
                });
                $("#forma-pago-select").html(html);
            }
        });
    }

    function updateDescuentoTotal() {
        var subtotal = parseFloat($("#subtotal-conceptos").text().replace(/,/g, ''));
        $("#total-con-descuento").text(formatMoney(subtotal));
    }

    function showConfirmation() {
        var nombreCompleto = selectedAlumno.Per_PrimerNombre + " " +
                           selectedAlumno.Per_SegundoNombre + " " +
                           selectedAlumno.Per_ApellidoPaterno + " " +
                           selectedAlumno.Per_ApellidoMaterno;
        $("#confirm-alumno").text(nombreCompleto);
        $("#confirm-conceptos").text(selectedConceptos.length + " concepto(s)");
        $("#confirm-forma-pago").text($("#forma-pago-select option:selected").text());
        $("#confirm-total").text($("#total-con-descuento").text());
    }

    function confirmarPago(reciboWindow) {
        // Monto total confirmado (total con descuento aplicado). Se envía al API:
        // sin él el pago se insertaba con MontoTotal = 0.
        var montoTotal = parseFloat($("#total-con-descuento").text().replace(/,/g, '')) || 0;

        var pagoData = {
            alumnoId: selectedAlumno.Alu_Id,
            formaPagoId: $("#forma-pago-select").val(),
            montoTotal: montoTotal,
            numeroReferencia: $("#numero-referencia").val(),
            observaciones: $("#observaciones").val(),
            conceptos: selectedConceptos,
            descuentoId: $("#descuento-select").val(),
            justificacionDescuento: $("#descuento-justificacion").val()
        };

        $.ajax({
            url: urls.urlRegistrarPago,
            type: "POST",
            data: JSON.stringify(pagoData),
            contentType: "application/json",
            success: function(response) {
                if (response.success) {
                    toastr.success("Pago registrado exitosamente");
                    abrirReciboUltimoPago(selectedAlumno.Alu_Id, reciboWindow);
                } else {
                    toastr.error(response.message || "Error al registrar el pago");
                    if (reciboWindow) { reciboWindow.close(); }
                }
            },
            error: function() {
                toastr.error("Error al procesar el pago. Por favor intente nuevamente.");
                if (reciboWindow) { reciboWindow.close(); }
            }
        });
    }

    // El endpoint CreateAsync de Pagos no retorna el Id del pago recién creado
    // (la API externa aún no expone el NewId del insert). Como workaround se
    // busca el pago más reciente del alumno vía el endpoint ya existente
    // ListByAlumnoAsync y se abre su recibo en la pestaña pre-abierta.
    function abrirReciboUltimoPago(alumnoId, reciboWindow) {
        $.ajax({
            url: "/Pagos/ListByAlumnoAsync",
            type: "GET",
            data: { alumnoId: alumnoId },
            success: function (response) {
                var lista = response && response.data ? response.data : response;
                var ultimoPago = null;
                if (lista && lista.length > 0) {
                    ultimoPago = lista.reduce(function (max, item) {
                        return (!max || item.PagoId > max.PagoId) ? item : max;
                    }, null);
                }
                if (ultimoPago && ultimoPago.PagoId && reciboWindow) {
                    reciboWindow.location.href = "/Pagos/Recibo?pagoId=" + ultimoPago.PagoId;
                } else if (reciboWindow) {
                    reciboWindow.close();
                }
            },
            error: function () {
                if (reciboWindow) { reciboWindow.close(); }
            },
            complete: function () {
                window.location.href = "/Pagos/Index";
            }
        });
    }

    function goToStep(step) {
        $(".payment-step").hide();
        $("#step" + step).show();

        $(".nav-link").removeClass("active");
        $("#step" + step + "-tab").addClass("active");

        currentStep = step;
    }

    function formatMoney(amount) {
        return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    function toIsoDate(d) {
        var m = (d.getMonth() + 1).toString().padStart(2, '0');
        var day = d.getDate().toString().padStart(2, '0');
        return d.getFullYear() + "-" + m + "-" + day;
    }

    function loadConceptosCatalogo() {
        $.ajax({
            url: urls.urlConceptosPago,
            type: "GET",
            success: function (response) {
                var data = response && response.data ? response.data : response;
                conceptosCatalogo = data || [];
                var html = '<option value="">-- Seleccione --</option>';
                conceptosCatalogo.forEach(function (c) {
                    if (c.EsActivo && !c.EsEliminado) {
                        html += '<option value="' + c.ConceptoPagoId + '">' + c.Descripcion + '</option>';
                    }
                });
                $("#adhoc-concepto").html(html);
            }
        });
    }

    function loadTarifas() {
        $.ajax({
            url: urls.urlTarifas,
            type: "GET",
            success: function (response) {
                var data = response && response.data ? response.data : response;
                tarifasCatalogo = data || [];
            }
        });
    }

    function montoSugeridoPorConcepto(conceptoPagoId) {
        if (!tarifasCatalogo || tarifasCatalogo.length === 0) return 0;
        var anioActual = new Date().getFullYear();
        var match = tarifasCatalogo
            .filter(function (t) { return t.ConceptoPagoId === parseInt(conceptoPagoId) && !t.EsEliminado; })
            .sort(function (a, b) { return b.AnioVigencia - a.AnioVigencia; })[0];
        return match ? parseFloat(match.Monto) : 0;
    }

    function initializeGenerarMensualidad() {
        $("#btn-generar-mensualidad").click(function () {
            if (!selectedAlumno) {
                toastr.warning("Debe seleccionar un alumno primero");
                return;
            }
            var mes = parseInt($("#mensualidad-mes").val());
            var anio = parseInt($("#mensualidad-anio").val());
            var monto = parseFloat($("#mensualidad-monto").val());
            if (!monto || monto <= 0) {
                monto = montoSugeridoPorConcepto(MENSUALIDAD_CONCEPTO_ID);
            }
            if (!mes || !anio || !monto || monto <= 0) {
                toastr.warning("Complete mes, año y monto válidos");
                return;
            }

            var vence = new Date(anio, mes, 0);

            crearCargo({
                AlumnoId: selectedAlumno.Alu_Id,
                ConceptoPagoId: MENSUALIDAD_CONCEPTO_ID,
                MontoOriginal: monto,
                MontoDescuento: 0,
                MontoMora: 0,
                FechaEmision: toIsoDate(new Date()),
                FechaVencimiento: toIsoDate(vence),
                Mes: mes,
                Anio: anio,
                Observaciones: "Mensualidad adelantada (mes " + mes + "/" + anio + ")",
                EstadoPagoId: 1
            }, "Mensualidad generada y agregada a la lista");
        });

        $("#adhoc-concepto").change(function () {
            var id = parseInt($(this).val());
            if (id) {
                var sugerido = montoSugeridoPorConcepto(id);
                if (sugerido > 0) $("#adhoc-monto").val(sugerido.toFixed(2));
            }
        });
    }

    function initializeAgregarCargo() {
        $("#btn-agregar-cargo").click(function () {
            if (!selectedAlumno) {
                toastr.warning("Debe seleccionar un alumno primero");
                return;
            }
            var conceptoId = parseInt($("#adhoc-concepto").val());
            var monto = parseFloat($("#adhoc-monto").val());
            var vence = $("#adhoc-vence").val();
            var obs = $("#adhoc-observaciones").val() || "Cargo puntual";

            if (!conceptoId) {
                toastr.warning("Seleccione un concepto");
                return;
            }
            if (!monto || monto <= 0) {
                toastr.warning("Ingrese un monto válido");
                return;
            }
            if (!vence) {
                toastr.warning("Seleccione fecha de vencimiento");
                return;
            }

            var venceDate = new Date(vence);

            crearCargo({
                AlumnoId: selectedAlumno.Alu_Id,
                ConceptoPagoId: conceptoId,
                MontoOriginal: monto,
                MontoDescuento: 0,
                MontoMora: 0,
                FechaEmision: toIsoDate(new Date()),
                FechaVencimiento: vence,
                Mes: venceDate.getMonth() + 1,
                Anio: venceDate.getFullYear(),
                Observaciones: obs,
                EstadoPagoId: 1
            }, "Cargo agregado a la lista");
        });
    }

    function crearCargo(payload, successMsg) {
        $.ajax({
            url: urls.urlCrearCargo,
            type: "POST",
            data: payload,
            success: function (response) {
                if (response && (response === true || response.success === true || response.item === true)) {
                    toastr.success(successMsg);
                    loadConceptosAlumno();
                    $("#adhoc-monto").val("");
                    $("#adhoc-observaciones").val("");
                    $("#mensualidad-monto").val("");
                } else {
                    toastr.error("No se pudo crear el cargo");
                }
            },
            error: function (xhr) {
                var msg = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : "Error al crear el cargo";
                toastr.error(msg);
            }
        });
    }

    return obj;
}());
