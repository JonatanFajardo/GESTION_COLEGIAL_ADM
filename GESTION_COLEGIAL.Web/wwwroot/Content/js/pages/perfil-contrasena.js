/* Mi Perfil — cambiar la propia contraseña (modal). El servidor vuelve a validar todo. */
(function ($) {
    "use strict";

    var $modal = $("#modal-contrasena");
    var $form = $("#form-contrasena");
    if (!$form.length) return;

    var $error = $("#contrasena-error");
    var $guardar = $("#btn-guardar-contrasena");

    function mostrarError(texto) {
        $error.text(texto).removeClass("d-none");
    }

    function avisar(texto, tipo) {
        if (typeof alertConfig !== "undefined" && alertConfig.alert) alertConfig.alert(texto, tipo);
    }

    // Cada vez que se abre: campos vacíos y sin errores.
    $modal.on("show.bs.modal", function () {
        $form[0].reset();
        $error.addClass("d-none").text("");
    });
    $modal.on("shown.bs.modal", function () {
        $("#contrasenaActual").trigger("focus");
    });

    $form.on("submit", function (e) {
        e.preventDefault();
        $error.addClass("d-none");

        var actual = $("#contrasenaActual").val();
        var nueva = $("#contrasenaNueva").val();
        var confirmar = $("#confirmarContrasena").val();

        if (!actual) return mostrarError("Escriba su contraseña actual.");
        if (!nueva || nueva.length < 8) return mostrarError("La contraseña nueva debe tener al menos 8 caracteres.");
        if (nueva !== confirmar) return mostrarError("La confirmación no coincide con la contraseña nueva.");

        $guardar.prop("disabled", true);
        $.ajax({
            type: "POST",
            url: $form.attr("action"),
            data: $form.serialize(),
            dataType: "json"
        }).done(function (r) {
            $modal.modal("hide");
            avisar((r && r.message) || "Contraseña actualizada.", "success");
        }).fail(function (xhr) {
            var r = xhr.responseJSON;
            mostrarError((r && r.message) || "No se pudo cambiar la contraseña. Intente de nuevo.");
        }).always(function () {
            $guardar.prop("disabled", false);
        });
    });
}(jQuery));
