/* Constancias y expediente — listado (filtro de año), Emitir (vista previa y emisión) y el documento (imprimir). */
(function ($) {
    "use strict";

    /* ───────────── Imprimir ───────────── */
    $(document).on("click", "[data-imprimir]", function () { window.print(); });

    /* ───────────── Listado: el año se aplica al cambiar ───────────── */
    var $filtros = $("#cst-form-filtros");
    if ($filtros.length) {
        $filtros.on("change", "[data-auto]", function () {
            $("#cst-cargando").removeAttr("hidden");
            $filtros.trigger("submit");
        });
        // Sin campos vacíos en la dirección.
        $filtros.on("submit", function () {
            $filtros.find("select, input").each(function () {
                if (!$(this).val()) $(this).prop("disabled", true);
            });
        });
    }

    /* ───────────── Emitir ───────────── */
    var $form = $("#cst-form-emitir");
    if (!$form.length) return;

    var $previa = $("#cst-previa");
    var $emitir = $("#cst-emitir");
    var $nota = $("#cst-emitir-nota");
    var $motivo = $("#motivo");
    var revisionSiguiente = 0;   // 0 = sin vista previa válida
    var firmaPrevia = "";        // la selección que produjo la vista previa

    function token() {
        return $form.find('input[name="__RequestVerificationToken"]').val();
    }

    function tipoSel() { return $form.find('input[name="tipo"]:checked'); }

    function datos() {
        var $t = tipoSel();
        var pideAnio = $t.data("pide-anio") === 1 || $t.data("pide-anio") === "1";
        var pideParcial = $t.data("pide-parcial") === 1 || $t.data("pide-parcial") === "1";
        return {
            Tipo: $t.val() || "",
            AluId: parseInt($("#alumno").val(), 10) || 0,
            AniId: pideAnio ? (parseInt($("#anio").val(), 10) || null) : null,
            PacId: pideParcial ? (parseInt($("#parcial").val(), 10) || null) : null,
            Motivo: $.trim($motivo.val() || "")
        };
    }

    function firma(d) { return [d.Tipo, d.AluId, d.AniId, d.PacId].join("|"); }

    /* Errores: uno por campo (data-error) o uno general arriba. Siempre como texto. */
    function limpiarErrores() {
        $form.find("[data-error]").text("");
        $form.find(".is-invalid").removeClass("is-invalid").removeAttr("aria-invalid");
        $("#cst-error-general").attr("hidden", true).text("");
    }

    var controles = { tipo: null, alumno: "#alumno", anio: "#anio", motivo: "#motivo" };

    function mostrarError(campo, mensaje) {
        var $p = campo ? $form.find('[data-error="' + campo + '"]').first() : $();
        if ($p.length) {
            $p.text(mensaje);
            var $c = controles[campo] ? $(controles[campo]) : $();
            if ($c.length) $c.addClass("is-invalid").attr("aria-invalid", "true").trigger("focus");
            else $p[0].scrollIntoView({ behavior: "smooth", block: "center" });
        } else {
            $("#cst-error-general").text(mensaje).removeAttr("hidden");
            window.scrollTo({ top: 0, behavior: "smooth" });
        }
    }

    function validar(d, paraEmitir) {
        if (!d.Tipo) return { campo: "tipo", mensaje: "Elija qué documento quiere emitir." };
        if (!d.AluId) return { campo: "alumno", mensaje: "Elija el alumno." };
        var pideAnio = tipoSel().data("pide-anio");
        if ((pideAnio === 1 || pideAnio === "1") && !d.AniId) return { campo: "anio", mensaje: "Elija el año lectivo." };
        if (paraEmitir && revisionSiguiente > 1 && !d.Motivo)
            return { campo: "motivo", mensaje: "Escriba el motivo: este documento ya se emitió antes y esta será la revisión " + revisionSiguiente + "." };
        if (d.Motivo.length > 500) return { campo: "motivo", mensaje: "El motivo admite hasta 500 caracteres." };
        return null;
    }

    /* Cualquier cambio de selección invalida la vista previa. */
    function invalidarPrevia() {
        revisionSiguiente = 0;
        firmaPrevia = "";
        $previa.empty();
        $("#cst-aviso-revision").attr("hidden", true).text("");
        $("#cst-campo-motivo").attr("hidden", true);
        $emitir.prop("disabled", true);
        $nota.text("Primero vea la vista previa.").removeAttr("hidden");
    }

    /* Año: oculto para el expediente. Parcial: solo para el boletín. */
    function ajustarCampos() {
        var $t = tipoSel();
        var pideAnio = $t.length === 0 || $t.data("pide-anio") === 1 || $t.data("pide-anio") === "1";
        var pideParcial = $t.data("pide-parcial") === 1 || $t.data("pide-parcial") === "1";
        $("#cst-campos-anio").prop("hidden", !pideAnio);
        $("#cst-anio-expediente").prop("hidden", pideAnio);
        $("#cst-campo-parcial").prop("hidden", !pideParcial);
    }

    $form.on("change", 'input[name="tipo"], #alumno, #anio, #parcial', function () {
        limpiarErrores();
        ajustarCampos();
        invalidarPrevia();
    });

    /* Buscar alumno: acorta la lista del select. */
    $("#cst-buscar-alumno").on("input", function () {
        var q = $.trim($(this).val() || "").toLowerCase();
        var $sel = $("#alumno");
        var visibles = 0, ultimo = null;
        $sel.find("option").each(function () {
            if (!this.value) return;
            var ok = !q || ($(this).data("buscar") || "").toString().indexOf(q) >= 0;
            this.hidden = !ok;
            this.disabled = !ok;
            if (ok) { visibles++; ultimo = this.value; }
        });
        // Si queda uno solo, se elige.
        if (visibles === 1 && $sel.val() !== ultimo) $sel.val(ultimo).trigger("change");
    });

    $("#cst-ver-previa").on("click", function () {
        limpiarErrores();
        var d = datos();
        var e = validar(d, false);
        if (e) { mostrarError(e.campo, e.mensaje); return; }

        var $btn = $(this).prop("disabled", true);
        invalidarPrevia();
        $("#cst-previa-cargando").removeAttr("hidden");

        $.ajax({
            url: $form.data("url-previa"),
            type: "GET",
            data: { tipo: d.Tipo, aluId: d.AluId, aniId: d.AniId || "", pacId: d.PacId || "" }
        }).done(function (resp, _s, xhr) {
            var tipoResp = (xhr.getResponseHeader("Content-Type") || "").toLowerCase();
            if (tipoResp.indexOf("json") >= 0 || typeof resp === "object") {
                var r = typeof resp === "string" ? JSON.parse(resp) : resp;
                mostrarError(r.field || null, r.message || "No se pudo preparar la vista previa.");
                return;
            }
            $previa.html(resp);
            revisionSiguiente = parseInt($previa.find("[data-revision]").first().attr("data-revision"), 10) || 1;
            firmaPrevia = firma(d);

            if (revisionSiguiente > 1) {
                $("#cst-aviso-revision")
                    .text("Este documento ya se emitió antes. Lo que emita ahora será la revisión " + revisionSiguiente +
                          " y la anterior quedará marcada como reemplazada. Escriba el motivo de la corrección.")
                    .removeAttr("hidden");
                $("#cst-campo-motivo").removeAttr("hidden");
            }
            $emitir.prop("disabled", false);
            $nota.attr("hidden", true);
        }).fail(function () {
            mostrarError(null, "No se pudo conectar. Revise la conexión e intente de nuevo.");
        }).always(function () {
            $btn.prop("disabled", false);
            $("#cst-previa-cargando").attr("hidden", true);
        });
    });

    $form.on("submit", function (ev) {
        ev.preventDefault();
        limpiarErrores();
        var d = datos();
        if (!revisionSiguiente || firma(d) !== firmaPrevia) {
            mostrarError(null, "Vea la vista previa antes de emitir.");
            return;
        }
        var e = validar(d, true);
        if (e) { mostrarError(e.campo, e.mensaje); return; }

        $emitir.prop("disabled", true).find("i").attr("class", "bi bi-hourglass-split");
        $.ajax({
            url: $form.data("url-emitir"),
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(d),
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        }).done(function (r) {
            if (r && r.success) {
                window.location.href = r.redirect;
                return;
            }
            if (r && r.codigo === -4) {
                // La API dice que ya hay una emisión previa: pedir el motivo.
                revisionSiguiente = Math.max(revisionSiguiente, 2);
                $("#cst-campo-motivo").removeAttr("hidden");
            }
            mostrarError(r && r.field || null, (r && r.message) || "No se pudo emitir el documento.");
            $emitir.prop("disabled", false);
        }).fail(function () {
            mostrarError(null, "No se pudo conectar. Revise la conexión e intente de nuevo.");
            $emitir.prop("disabled", false);
        }).always(function () {
            $emitir.find("i").attr("class", "bi bi-check2-circle");
        });
    });

    ajustarCampos();
})(jQuery);
