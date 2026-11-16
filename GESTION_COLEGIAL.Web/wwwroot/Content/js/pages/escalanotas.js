// Escala de notas + umbrales del cuadro de honor (pantalla dedicada).
var EscalaNotas = (function () {
    "use strict";

    var cfg = { puedeEditar: false, urls: {} };

    function fmt(n) {
        var v = parseFloat(n);
        return (v % 1 === 0) ? v.toFixed(0) : v.toFixed(2).replace(/0$/, "");
    }

    // ---- Escala ----
    function cargarEscala() {
        $.getJSON(cfg.urls.list, function (resp) {
            var filas = (resp && resp.data) ? resp.data : [];
            render(filas);
            avisoCobertura(filas);
        });
    }

    function render(filas) {
        var $b = $("#tbody-escala").empty();
        if (!filas.length) {
            $b.append('<tr><td colspan="6" class="text-center text-muted py-4">Aún no hay rangos definidos.</td></tr>');
            return;
        }
        filas.forEach(function (f) {
            var aprueba = f.Esc_Aprueba
                ? '<span class="badge badge-success">Sí</span>'
                : '<span class="badge badge-light">No</span>';
            var abrev = f.Esc_Abreviatura ? '<span class="badge badge-info">' + f.Esc_Abreviatura + '</span>' : '<span class="text-muted">—</span>';
            var acciones = "";
            if (cfg.puedeEditar) {
                acciones =
                    '<td class="text-center">' +
                    '<button class="btn btn-sm btn-outline-primary mr-1 js-editar" data-id="' + f.Esc_Id + '" title="Editar"><i class="bi bi-pencil"></i></button>' +
                    '<button class="btn btn-sm btn-outline-danger js-eliminar" data-id="' + f.Esc_Id + '" data-nombre="' + $("<div>").text(f.Esc_Concepto).html() + '" title="Eliminar"><i class="bi bi-trash"></i></button>' +
                    '</td>';
            }
            $b.append(
                '<tr>' +
                '<td>' + f.Esc_Orden + '</td>' +
                '<td><strong>' + $("<div>").text(f.Esc_Concepto).html() + '</strong></td>' +
                '<td class="text-center">' + abrev + '</td>' +
                '<td class="text-center">' + fmt(f.Esc_NotaDesde) + ' – ' + fmt(f.Esc_NotaHasta) + '</td>' +
                '<td class="text-center">' + aprueba + '</td>' +
                acciones +
                '</tr>'
            );
        });
    }

    // Avisa si la escala no cubre todo 0–100 o si deja huecos.
    function avisoCobertura(filas) {
        var $c = $("#aviso-cobertura").empty();
        if (!filas.length) return;
        var rangos = filas.map(function (f) { return { d: parseFloat(f.Esc_NotaDesde), h: parseFloat(f.Esc_NotaHasta) }; })
                          .sort(function (a, b) { return a.d - b.d; });
        var huecos = [];
        if (rangos[0].d > 0) huecos.push("0–" + fmt(rangos[0].d));
        for (var i = 1; i < rangos.length; i++) {
            if (rangos[i].d > rangos[i - 1].h + 0.01) {
                huecos.push(fmt(rangos[i - 1].h) + "–" + fmt(rangos[i].d));
            }
        }
        var ultimo = rangos[rangos.length - 1];
        if (ultimo.h < 100) huecos.push(fmt(ultimo.h) + "–100");

        if (huecos.length) {
            $c.html('<div class="alert alert-light-warning mb-0"><i class="fas fa-exclamation-triangle mr-1"></i> ' +
                'La escala deja sin concepto estos promedios: <strong>' + huecos.join(", ") + '</strong>.</div>');
        } else {
            $c.html('<div class="alert alert-light-success mb-0"><i class="bi bi-check-circle mr-1"></i> La escala cubre todo el rango de 0 a 100.</div>');
        }
    }

    // ---- Modal rango ----
    function nuevoRango() {
        $("#modal-rango-title").text("Nuevo rango");
        $("#r-id").val(0);
        $("#r-concepto").val("");
        $("#r-abrev").val("");
        $("#r-orden").val(0);
        $("#r-desde").val("");
        $("#r-hasta").val("");
        $("#r-aprueba").prop("checked", true);
        $("#modal-rango").modal("show");
    }

    function editarRango(id) {
        $.getJSON(cfg.urls.find + "?id=" + id, function (resp) {
            var f = resp && resp.item ? resp.item : null;
            if (!f) { toastr.error("No se encontró el rango."); return; }
            $("#modal-rango-title").text("Editar rango");
            $("#r-id").val(f.Esc_Id);
            $("#r-concepto").val(f.Esc_Concepto);
            $("#r-abrev").val(f.Esc_Abreviatura || "");
            $("#r-orden").val(f.Esc_Orden);
            $("#r-desde").val(f.Esc_NotaDesde);
            $("#r-hasta").val(f.Esc_NotaHasta);
            $("#r-aprueba").prop("checked", !!f.Esc_Aprueba);
            $("#modal-rango").modal("show");
        });
    }

    function guardarRango() {
        var concepto = $.trim($("#r-concepto").val());
        var desde = $("#r-desde").val();
        var hasta = $("#r-hasta").val();
        if (!concepto) { toastr.warning("Ingrese el concepto."); return; }
        if (desde === "" || hasta === "") { toastr.warning("Ingrese el rango de notas."); return; }

        var model = {
            Esc_Id: parseInt($("#r-id").val(), 10) || 0,
            Esc_Concepto: concepto,
            Esc_Abreviatura: $.trim($("#r-abrev").val()),
            Esc_NotaDesde: parseFloat(desde),
            Esc_NotaHasta: parseFloat(hasta),
            Esc_Aprueba: $("#r-aprueba").is(":checked"),
            Esc_Orden: parseInt($("#r-orden").val(), 10) || 0
        };

        var $btn = $("#btn-guardar-rango").prop("disabled", true);
        $.post(cfg.urls.guardar, model, function (resp) {
            if (resp && resp.success) {
                toastr.success(resp.message || "Guardado.");
                $("#modal-rango").modal("hide");
                cargarEscala();
            } else {
                toastr.error(resp && resp.message ? resp.message : "No se pudo guardar.");
            }
        }).fail(function () {
            toastr.error("Error de comunicación con el servidor.");
        }).always(function () {
            $btn.prop("disabled", false);
        });
    }

    function pedirEliminar(id, nombre) {
        $("#del-id").val(id);
        $("#del-nombre").text(nombre);
        $("#modal-eliminar").modal("show");
    }

    function eliminar() {
        var id = parseInt($("#del-id").val(), 10);
        var $btn = $("#btn-confirmar-eliminar").prop("disabled", true);
        $.post(cfg.urls.eliminar, { id: id }, function (resp) {
            if (resp && resp.success) {
                toastr.success(resp.message || "Eliminado.");
                $("#modal-eliminar").modal("hide");
                cargarEscala();
            } else {
                toastr.error(resp && resp.message ? resp.message : "No se pudo eliminar.");
            }
        }).fail(function () {
            toastr.error("Error de comunicación con el servidor.");
        }).always(function () {
            $btn.prop("disabled", false);
        });
    }

    // ---- Cuadro de honor ----
    function cargarConfig() {
        $.getJSON(cfg.urls.config, function (resp) {
            var c = resp && resp.item ? resp.item : null;
            if (c) {
                $("#chc-honor").val(c.Chc_UmbralHonor);
                $("#chc-honor-sup").val(c.Chc_UmbralHonorSuperior);
            }
        });
    }

    function guardarConfig() {
        var model = {
            Chc_UmbralHonor: parseFloat($("#chc-honor").val()),
            Chc_UmbralHonorSuperior: parseFloat($("#chc-honor-sup").val())
        };
        if (isNaN(model.Chc_UmbralHonor) || isNaN(model.Chc_UmbralHonorSuperior)) {
            toastr.warning("Ingrese ambos umbrales.");
            return;
        }
        var $btn = $("#btn-guardar-config").prop("disabled", true);
        $.post(cfg.urls.guardarConfig, model, function (resp) {
            if (resp && resp.success) {
                toastr.success(resp.message || "Guardado.");
            } else {
                toastr.error(resp && resp.message ? resp.message : "No se pudo guardar.");
            }
        }).fail(function () {
            toastr.error("Error de comunicación con el servidor.");
        }).always(function () {
            $btn.prop("disabled", false);
        });
    }

    function init(options) {
        cfg = $.extend(true, cfg, options || {});
        cargarEscala();
        cargarConfig();

        if (cfg.puedeEditar) {
            $("#btn-nuevo-rango").on("click", nuevoRango);
            $("#btn-guardar-rango").on("click", guardarRango);
            $("#btn-confirmar-eliminar").on("click", eliminar);
            $("#btn-guardar-config").on("click", guardarConfig);
            $("#tbody-escala").on("click", ".js-editar", function () { editarRango($(this).data("id")); });
            $("#tbody-escala").on("click", ".js-eliminar", function () {
                pedirEliminar($(this).data("id"), $(this).data("nombre"));
            });
        }
    }

    return { init: init };
})();
