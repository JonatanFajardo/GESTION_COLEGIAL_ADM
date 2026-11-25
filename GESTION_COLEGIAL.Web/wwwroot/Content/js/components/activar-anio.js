/* Abrir un año lectivo como activo. Si hay alumnos que cambiarían de sección, el servidor no
   hace nada hasta que el usuario confirme aquí (se vuelve a enviar con confirmar=true). */
window.ActivarAnio = (function () {
    function esc(t) { return $("<div>").text(t == null ? "" : String(t)).html(); }

    function modal() {
        var $m = $("#modal-activar-anio");
        if ($m.length) return $m;
        $m = $(
            '<div class="modal fade" id="modal-activar-anio" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-dialog-centered" role="document"><div class="modal-content">' +
            '<div class="modal-header"><h5 class="modal-title">Abrir el año como activo</h5>' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Cerrar"><span aria-hidden="true">&times;</span></button></div>' +
            '<div class="modal-body" id="activar-anio-cuerpo"></div>' +
            '<div class="modal-footer"><button type="button" class="btn" data-dismiss="modal">Cancelar</button>' +
            '<button type="button" class="btn btn-primary" id="activar-anio-ok">Sí, abrir el año</button></div>' +
            '</div></div></div>');
        $("body").append($m);
        return $m;
    }

    /* url: dirección de SetActivoAsync. onListo(resp) al terminar bien; onCancelar() si no se confirma o falla. */
    function pedir(url, id, onListo, onCancelar, aviso) {
        function fallo(msg) { aviso("error", msg); if (onCancelar) onCancelar(); }
        $.post(url, { id: id, confirmar: false }).done(function (r) {
            if (r && r.requiereConfirmacion) {
                var $m = modal();
                var html = "<p><strong>" + esc(r.mover) + " alumnos cambiarán de sección</strong> al abrir este año: pasan a la sección en la que están inscritos en él.</p>" +
                    (r.revisar > 0 ? "<p class='text-muted mb-1'>" + esc(r.revisar) + " cambian de nivel y habrá que revisar su curso.</p>" : "") +
                    (r.noActivos > 0 ? "<p class='text-muted mb-1'>" + esc(r.noActivos) + " inscritos no se tocan por no estar Activos.</p>" : "") +
                    "<p class='mb-0'>Queda un registro de cada cambio. ¿Desea continuar?</p>";
                $("#activar-anio-cuerpo").html(html);
                var confirmado = false;
                $("#activar-anio-ok").off("click").on("click", function () {
                    var $b = $(this).prop("disabled", true);
                    confirmado = true;
                    $.post(url, { id: id, confirmar: true }).done(function (r2) {
                        $m.modal("hide");
                        if (r2 && r2.success === false) fallo(r2.message); else { aviso("success", r2 && r2.message); if (onListo) onListo(r2); }
                    }).fail(function () { $m.modal("hide"); fallo("Ocurrió un error."); })
                      .always(function () { $b.prop("disabled", false); });
                });
                $m.off("hidden.bs.modal").on("hidden.bs.modal", function () { if (!confirmado && onCancelar) onCancelar(); });
                $m.modal("show");
            } else if (r && r.success === false) {
                fallo(r.message);
            } else {
                aviso("success", r && r.message); if (onListo) onListo(r);
            }
        }).fail(function () { fallo("Ocurrió un error."); });
    }
    return { pedir: pedir };
})();
