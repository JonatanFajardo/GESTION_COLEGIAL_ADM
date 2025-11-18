var NuevoPago = (function () {
    var obj = {};
    var urls = {};
    var currentStep = 1;
    var selectedAlumno = null;
    var selectedConceptos = [];
    var selectedDescuento = null;
    var selectedFormaPago = null;

    obj.init = function (config) {
        urls = config;

        $(function () {
            initializeStepNavigation();
            initializeAlumnoSearch();
            loadDescuentos();
            loadFormasPago();
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
            confirmarPago();
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

    function buscarAlumnoPorIdentidad(identidad) {
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

        // Mostrar imagen del alumno
        if (alumno.Per_Imagen) {
            $("#alumno-imagen").attr("src", "/Content/images/alumnos/" + alumno.Per_Imagen);
        } else {
            // Imagen por defecto según el sexo
            var imagenDefault = alumno.Per_Sexo === 'M' ?
                "/Content/images/default-male-avatar.png" :
                "/Content/images/default-female-avatar.png";
            $("#alumno-imagen").attr("src", imagenDefault);
        }
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
        $("#alumno-imagen").attr("src", "");
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
                html += '<td><input type="checkbox" class="concepto-checkbox" data-concepto=\'' + JSON.stringify(concepto) + '\' /></td>';
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
            success: function(data) {
                var html = '<option value="">-- Sin descuento --</option>';
                data.forEach(function(item) {
                    html += '<option value="' + item.value + '">' + item.text + '</option>';
                });
                $("#descuento-select").html(html);
            }
        });
    }

    function loadFormasPago() {
        $.ajax({
            url: urls.urlFormasPago,
            success: function(data) {
                var html = '<option value="">-- Seleccione --</option>';
                data.forEach(function(item) {
                    html += '<option value="' + item.value + '">' + item.text + '</option>';
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

    function confirmarPago() {
        var pagoData = {
            alumnoId: selectedAlumno.Alu_Id,
            formaPagoId: $("#forma-pago-select").val(),
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
                    window.location.href = "/Pagos/Index";
                } else {
                    toastr.error(response.message || "Error al registrar el pago");
                }
            },
            error: function() {
                toastr.error("Error al procesar el pago. Por favor intente nuevamente.");
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

    return obj;
}());
