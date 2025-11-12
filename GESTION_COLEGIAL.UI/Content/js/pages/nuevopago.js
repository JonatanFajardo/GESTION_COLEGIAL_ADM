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
                alertConfig.alert("Debe seleccionar un alumno", "warning");
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
                alertConfig.alert("Debe seleccionar al menos un concepto", "warning");
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
                alertConfig.alert("Debe seleccionar una forma de pago", "warning");
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
        $("#alumno-search").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: urls.urlBuscarAlumno,
                    data: { term: request.term },
                    success: function(data) {
                        response(data);
                    }
                });
            },
            minLength: 2,
            select: function(event, ui) {
                selectedAlumno = ui.item;
                $("#alumno-nombre-selected").text(ui.item.label);
                $("#alumno-info").show();
            }
        });
    }

    function loadConceptosAlumno() {
        $.ajax({
            url: urls.urlConceptosAlumno,
            data: { alumnoId: selectedAlumno.value },
            success: function(response) {
                if (response.success) {
                    renderConceptos(response.data);
                }
            }
        });
    }

    function renderConceptos(conceptos) {
        var html = "";
        conceptos.forEach(function(concepto) {
            html += "<tr>";
            html += '<td><input type="checkbox" class="concepto-checkbox" data-concepto=\'' + JSON.stringify(concepto) + '\' /></td>';
            html += "<td>" + concepto.conceptoPago + "</td>";
            html += "<td>L. " + formatMoney(concepto.montoOriginal) + "</td>";
            html += "<td>L. " + formatMoney(concepto.montoDescuento) + "</td>";
            html += "<td>L. " + formatMoney(concepto.montoMora) + "</td>";
            html += "<td>L. " + formatMoney(concepto.montoTotal) + "</td>";
            html += "</tr>";
        });
        $("#conceptos-tbody").html(html);

        $(".concepto-checkbox").change(function() {
            calculateSubtotal();
        });
    }

    function calculateSubtotal() {
        var subtotal = 0;
        $(".concepto-checkbox:checked").each(function() {
            var concepto = $(this).data("concepto");
            subtotal += parseFloat(concepto.montoTotal);
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
        $("#confirm-alumno").text(selectedAlumno.label);
        $("#confirm-conceptos").text(selectedConceptos.length + " concepto(s)");
        $("#confirm-forma-pago").text($("#forma-pago-select option:selected").text());
        $("#confirm-total").text($("#total-con-descuento").text());
    }

    function confirmarPago() {
        var pagoData = {
            alumnoId: selectedAlumno.value,
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
                    alertConfig.alert("Pago registrado exitosamente", "success");
                    window.location.href = "/Pagos/ListaDia";
                } else {
                    alertConfig.alert(response.message, "error");
                }
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
