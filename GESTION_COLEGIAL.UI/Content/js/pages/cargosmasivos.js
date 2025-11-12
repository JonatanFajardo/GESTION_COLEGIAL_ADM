var CargosMasivos = (function () {
    var obj = {};
    var urls = {};

    obj.init = function (Direction) {
        urls = Direction;
        $(function () {
            cargarConceptos();
            cargarNiveles();
            cargarCursos();
            cargarSecciones();

            // Evento previsualizar
            $("#btn-previsualizar").click(function() {
                previsualizar();
            });

            // Año actual por defecto
            $("#anio").val(new Date().getFullYear());
        });
    }

    function cargarConceptos() {
        $.ajax({
            url: urls.urlConceptos,
            type: "GET",
            success: function(data) {
                if (data && data.length > 0) {
                    var html = "";
                    data.forEach(function(item) {
                        html += '<div class="form-check">';
                        html += '<input class="form-check-input concepto-check" type="checkbox" value="' + item.value + '" id="concepto_' + item.value + '">';
                        html += '<label class="form-check-label" for="concepto_' + item.value + '">' + item.text + '</label>';
                        html += '</div>';
                    });
                    $("#conceptos-list").html(html);
                }
            }
        });
    }

    function cargarNiveles() {
        $.ajax({
            url: urls.urlNiveles,
            type: "GET",
            success: function(data) {
                if (data && data.length > 0) {
                    var html = '<option value="">-- Todos --</option>';
                    data.forEach(function(item) {
                        html += '<option value="' + item.value + '">' + item.text + '</option>';
                    });
                    $("#nivelId").html(html);
                }
            }
        });
    }

    function cargarCursos() {
        $.ajax({
            url: urls.urlCursos,
            type: "GET",
            success: function(data) {
                if (data && data.length > 0) {
                    var html = '<option value="">-- Todos --</option>';
                    data.forEach(function(item) {
                        html += '<option value="' + item.value + '">' + item.text + '</option>';
                    });
                    $("#cursoId").html(html);
                }
            }
        });
    }

    function cargarSecciones() {
        $.ajax({
            url: urls.urlSecciones,
            type: "GET",
            success: function(data) {
                if (data && data.length > 0) {
                    var html = '<option value="">-- Todas --</option>';
                    data.forEach(function(item) {
                        html += '<option value="' + item.value + '">' + item.text + '</option>';
                    });
                    $("#seccionId").html(html);
                }
            }
        });
    }

    function previsualizar() {
        var conceptosSeleccionados = [];
        $(".concepto-check:checked").each(function() {
            conceptosSeleccionados.push($(this).val());
        });

        if (conceptosSeleccionados.length === 0) {
            alertConfig.alert("Debe seleccionar al menos un concepto de pago", "warning");
            return;
        }

        var data = {
            anio: $("#anio").val(),
            nivelId: $("#nivelId").val(),
            cursoId: $("#cursoId").val(),
            seccionId: $("#seccionId").val(),
            conceptos: conceptosSeleccionados
        };

        $.ajax({
            url: urls.urlPrevisualizar,
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function(response) {
                if (response.success) {
                    $("#panel-previsualizacion").show();
                    var html = "<p><strong>Total de alumnos afectados: " + response.data.length + "</strong></p>";
                    html += "<ul>";
                    response.data.forEach(function(alumno) {
                        html += "<li>" + alumno.nombreCompleto + " - " + alumno.nivel + " " + alumno.curso + "</li>";
                    });
                    html += "</ul>";
                    $("#lista-alumnos").html(html);
                } else {
                    alertConfig.alert(response.message, "error");
                }
            }
        });
    }

    return obj;
}());
