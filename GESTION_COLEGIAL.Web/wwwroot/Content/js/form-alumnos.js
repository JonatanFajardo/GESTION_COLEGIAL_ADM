// Formulario de alumnos (crear / editar)
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var form = document.querySelector('form[action*="Guardar"]');
        if (!form) return;

        var toggle = document.getElementById('statusToggle');
        var badge = document.getElementById('statusBadge');
        var texto = document.getElementById('statusText');
        var esActivo = document.getElementById('Per_EsActivo');

        // El interruptor "Estado del alumno" manda el valor real (Per_EsActivo) al guardar.
        function pintarEstado() {
            var activo = !toggle || toggle.checked;
            if (esActivo) esActivo.value = activo ? 'true' : 'false';
            if (badge) {
                badge.classList.toggle('badge-success', activo);
                badge.classList.toggle('badge-secondary', !activo);
            }
            if (texto) texto.textContent = activo ? 'Activo' : 'Inactivo';
        }
        if (toggle) {
            toggle.addEventListener('change', pintarEstado);
            pintarEstado();
        }

        // La identidad se escribe con o sin guiones; se envía solo con dígitos.
        var identidad = document.getElementById('Per_Identidad');
        function limpiarIdentidad() { if (identidad) identidad.value = identidad.value.replace(/[\s-]/g, ''); }
        if (identidad) identidad.addEventListener('blur', limpiarIdentidad);
        var telefono = document.getElementById('Per_Telefono');
        if (telefono) telefono.addEventListener('blur', function () { telefono.value = telefono.value.replace(/[\s-]/g, ''); });

        // Cancelar: pide confirmación solo si se está creando y ya se escribió algo.
        var cancelar = document.getElementById('cancelBtn');
        if (cancelar) {
            cancelar.addEventListener('click', function (e) {
                var esNuevo = !document.getElementById('Alu_Id') || document.getElementById('Alu_Id').value === '0';
                var nombre = document.getElementById('Per_PrimerNombre');
                if (esNuevo && nombre && nombre.value.trim() !== '' &&
                    !confirm('¿Está seguro de que desea cancelar? Los cambios no guardados se perderán.')) {
                    e.preventDefault();
                }
            });
        }

        // Antes de enviar: normaliza y revisa lo mínimo (el servidor valida todo de nuevo).
        form.addEventListener('submit', function (e) {
            limpiarIdentidad();
            if (telefono) telefono.value = telefono.value.replace(/[\s-]/g, '');
            var nombre = document.getElementById('Per_PrimerNombre');
            var nivel = document.getElementById('Niv_Id');
            if (nombre && nombre.value.trim() === '') {
                alert('Por favor ingrese el primer nombre del alumno.');
                e.preventDefault();
                nombre.focus();
                return;
            }
            if (nivel && nivel.value === '') {
                alert('Por favor seleccione un nivel educativo.');
                e.preventDefault();
                nivel.focus();
            }
        });
    });
})();
