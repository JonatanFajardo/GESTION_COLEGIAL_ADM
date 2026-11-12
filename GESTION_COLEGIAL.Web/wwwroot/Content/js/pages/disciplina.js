/* Disciplina — Casos (filtros), Registrar un caso (filas de personas) y el caso (estado, acciones, notas). */
(function ($) {
    "use strict";

    function token() {
        return $('input[name="__RequestVerificationToken"]').first().val();
    }

    function postJson(url, datos) {
        return $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos),
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    function post(url, parametros) {
        return $.ajax({
            url: url + (url.indexOf("?") < 0 ? "?" : "&") + $.param(parametros),
            type: "POST",
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    /* Errores: uno por campo (data-error) o uno general arriba. Siempre como texto, nunca HTML. */
    function limpiarErrores($ambito) {
        $ambito.find("[data-error]").text("");
        $ambito.find(".is-invalid").removeClass("is-invalid").removeAttr("aria-invalid");
        $("#dis-error-general").attr("hidden", true).text("");
    }

    function mostrarError($ambito, campo, mensaje, $control) {
        var $p = campo ? $ambito.find('[data-error="' + campo + '"]').first() : $();
        if ($p.length) {
            $p.text(mensaje);
            if ($control && $control.length) $control.addClass("is-invalid").attr("aria-invalid", "true").trigger("focus");
        } else {
            $("#dis-error-general").text(mensaje).removeAttr("hidden");
            window.scrollTo({ top: 0, behavior: "smooth" });
        }
    }

    function fallaRed() {
        $("#dis-error-general").text("No se pudo conectar. Revise la conexión e intente de nuevo.").removeAttr("hidden");
    }

    /* ───────────── Casos: los filtros se aplican al cambiar ───────────── */
    var $filtros = $("#dis-form-filtros");
    if ($filtros.length) {
        $filtros.on("change", "[data-auto]", function () {
            $("#dis-cargando").removeAttr("hidden");
            // Sin campos vacíos en la dirección.
            $filtros.find("select").each(function () { $(this).prop("disabled", !$(this).val()); });
            $filtros.trigger("submit");
        });
    }

    /* ───────────── Registrar un caso ───────────── */
    var $registrar = $("#dis-form-registrar");
    if ($registrar.length) {
        var max = parseInt($registrar.data("max"), 10) || 20;
        var $filas = $("#dis-filas");
        var plantilla = document.getElementById("dis-plantilla-fila");
        var contador = 0;

        function renumerar() {
            $filas.children("li").each(function (i) {
                $(this).find("[data-quitar]").attr("aria-label", "Quitar la fila " + (i + 1));
            });
            $("#dis-agregar-persona").toggle($filas.children("li").length < max);
        }

        function agregarFila() {
            if ($filas.children("li").length >= max) return;
            contador++;
            var $fila = $(plantilla.content.cloneNode(true)).children("li");
            $fila.find("[data-campo]").each(function () {
                var id = "persona-" + contador + "-" + $(this).data("campo");
                $(this).attr("id", id);
                $fila.find('[data-for="' + $(this).data("campo") + '"]').attr("for", id);
            });
            $filas.append($fila);
            renumerar();
            return $fila;
        }

        $("#dis-agregar-persona").on("click", function () {
            var $f = agregarFila();
            if ($f) $f.find('[data-campo="alumno"]').trigger("focus");
        });

        $filas.on("click", "[data-quitar]", function () {
            $(this).closest("li").remove();
            renumerar();
        });

        // Como Skuul: el formulario arranca con una fila.
        agregarFila();

        // Protección del menor = reservado.
        $("#tipo").on("change", function () {
            $("#marca-reservado").prop("hidden", $(this).val() !== "proteccion");
        }).trigger("change");

        $registrar.on("submit", function (e) {
            e.preventDefault();
            limpiarErrores($registrar);

            var resumen = $.trim($("#resumen").val());
            var fecha = $("#fecha").val();
            var maxFecha = $("#fecha").attr("max");
            if (!resumen) return mostrarError($registrar, "resumen", "Escriba un resumen del caso.", $("#resumen"));
            if (!fecha) return mostrarError($registrar, "fecha", "Indique cuándo ocurrió.", $("#fecha"));
            if (maxFecha && fecha > maxFecha)
                return mostrarError($registrar, "fecha", "No se puede registrar un caso con una fecha que todavía no ha pasado.", $("#fecha"));

            // Una fila sin alumno simplemente no se usó.
            var personas = [];
            $filas.children("li").each(function () {
                var alu = $(this).find('[data-campo="alumno"]').val();
                if (!alu) return;
                personas.push({
                    Alu_Id: parseInt(alu, 10),
                    Papel: $(this).find('[data-campo="papel"]').val(),
                    Nota: $.trim($(this).find('[data-campo="nota"]').val()) || null
                });
            });

            var datos = {
                Cas_Resumen: resumen,
                Cas_Tipo: $("#tipo").val(),
                Cas_FechaOcurrio: fecha,
                Cas_Lugar: $.trim($("#lugar").val()) || null,
                Emp_IdAtiende: $("#atiende").val() ? parseInt($("#atiende").val(), 10) : null,
                Cas_Descripcion: $.trim($("#descripcion").val()) || null,
                Personas: personas
            };

            var $btn = $("#dis-guardar").prop("disabled", true);
            postJson($registrar.data("url"), datos)
                .done(function (r) {
                    if (r && r.success) { window.location.href = r.redirect; return; }
                    var campos = { resumen: "#resumen", tipo: "#tipo", fecha: "#fecha" };
                    mostrarError($registrar, r && r.field, (r && r.message) || "No se pudo registrar el caso.", $(campos[r && r.field]));
                    $btn.prop("disabled", false);
                })
                .fail(function () { fallaRed(); $btn.prop("disabled", false); });
        });
    }

    /* ───────────── El caso ───────────── */
    var $caso = $("#dis-caso");
    if ($caso.length) {
        var casId = parseInt($caso.data("caso"), 10);

        function enviar($form, url, datos, campos) {
            limpiarErrores($caso);
            var $btn = $form.find('button[type="submit"]').prop("disabled", true);
            return postJson(url, datos)
                .done(function (r) {
                    if (r && r.success) { window.location.reload(); return; }
                    var campo = r && r.field;
                    mostrarError($caso, campo, (r && r.message) || "No se pudo guardar.", $(campos[campo]));
                    $btn.prop("disabled", false);
                })
                .fail(function () { fallaRed(); $btn.prop("disabled", false); });
        }

        $("#dis-form-estado").on("submit", function (e) {
            e.preventDefault();
            enviar($(this), $caso.data("url-estado"), {
                Cas_Id: casId,
                Estado: $("#estado").val(),
                Motivo: $.trim($("#motivo").val()) || null
            }, { estado: "#estado", motivo: "#motivo" });
        });

        $("#dis-form-accion").on("submit", function (e) {
            e.preventDefault();
            var $f = $(this);
            limpiarErrores($caso);
            var tipo = $.trim($("#accion-tipo").val());
            var desc = $.trim($("#accion-descripcion").val());
            if (!tipo) return mostrarError($caso, "tipo", "Diga qué tipo de paso es.", $("#accion-tipo"));
            if (!desc) return mostrarError($caso, "descripcion", "Diga qué tiene que pasar.", $("#accion-descripcion"));
            enviar($f, $caso.data("url-accion"), {
                Cas_Id: casId,
                Cac_Tipo: tipo,
                Cac_Descripcion: desc,
                Cac_FechaLimite: $("#accion-fecha").val() || null,
                Emp_IdResponsable: $("#accion-responsable").val() ? parseInt($("#accion-responsable").val(), 10) : null
            }, { tipo: "#accion-tipo", descripcion: "#accion-descripcion" });
        });

        $caso.on("click", "[data-completar]", function () {
            var $btn = $(this).prop("disabled", true);
            limpiarErrores($caso);
            post($caso.data("url-completar"), { casId: casId, cacId: $btn.data("completar") })
                .done(function (r) {
                    if (r && r.success) { window.location.reload(); return; }
                    mostrarError($caso, null, (r && r.message) || "No se pudo marcar la acción como hecha.");
                    $btn.prop("disabled", false);
                })
                .fail(function () { fallaRed(); $btn.prop("disabled", false); });
        });

        $("#dis-form-nota").on("submit", function (e) {
            e.preventDefault();
            limpiarErrores($caso);
            var texto = $.trim($("#nota").val());
            if (!texto) return mostrarError($caso, "nota", "Escriba la nota.", $("#nota"));
            enviar($(this), $caso.data("url-nota"), {
                Cas_Id: casId,
                Cno_Texto: texto,
                Cno_EsPrivada: $("#nota-privada").is(":checked")
            }, { nota: "#nota" });
        });
    }
})(jQuery);
