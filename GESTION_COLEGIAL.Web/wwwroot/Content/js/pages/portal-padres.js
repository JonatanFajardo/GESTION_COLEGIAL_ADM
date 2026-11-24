/* Portal de padres e invitaciones al portal.
   - Solicitud: la fecha de la falta solo aparece (y es obligatoria) al justificar una falta.
   - Retirar solicitud: confirmación.
   - Invitaciones: copiar el enlace recién generado. */
(function () {
    'use strict';

    function alternarFecha() {
        var tipo = document.getElementById('sol-tipo');
        var grupo = document.getElementById('sol-fecha-grupo');
        var fecha = document.getElementById('sol-fecha');
        if (!tipo || !grupo || !fecha || !document.getElementById('portal-solicitud')) return;
        var justificar = tipo.value === 'justificar';
        grupo.classList.toggle('d-none', !justificar);
        fecha.required = justificar;
    }

    document.addEventListener('DOMContentLoaded', function () {
        var tipo = document.getElementById('sol-tipo');
        if (tipo && document.getElementById('portal-solicitud')) {
            tipo.addEventListener('change', alternarFecha);
            alternarFecha();
        }

        document.querySelectorAll('.portal-retirar').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!window.confirm('¿Retirar esta solicitud? El colegio ya no la atenderá.')) e.preventDefault();
            });
        });

        var copiar = document.getElementById('inv-copiar');
        if (copiar) {
            copiar.addEventListener('click', function () {
                var campo = document.querySelector(copiar.getAttribute('data-copiar'));
                if (!campo) return;
                var listo = function () {
                    copiar.innerHTML = '<i class="bi bi-check2 mr-1" aria-hidden="true"></i> Copiado';
                };
                if (navigator.clipboard && window.isSecureContext) {
                    navigator.clipboard.writeText(campo.value).then(listo, function () {
                        campo.select();
                        document.execCommand('copy');
                        listo();
                    });
                } else {
                    campo.select();
                    document.execCommand('copy');
                    listo();
                }
            });
        }
    });
})();
