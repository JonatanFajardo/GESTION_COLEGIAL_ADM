// Funcionalidad para el formulario de encargados
(function() {
    'use strict';

    // Estado del formulario
    let estadoActivo = true;

    // Elementos del DOM
    const statusToggle = document.getElementById('statusToggle');
    const statusBadge = document.getElementById('statusBadge');
    const statusDot = document.getElementById('statusDot');
    const statusText = document.getElementById('statusText');

    // Función para actualizar el estado visual
    function actualizarEstado(activo) {
        estadoActivo = activo;

        if (activo) {
            statusBadge.classList.remove('inactive');
            statusBadge.classList.add('active');
            statusDot.classList.remove('inactive');
            statusDot.classList.add('active');
            statusText.textContent = 'Activo';
        } else {
            statusBadge.classList.remove('active');
            statusBadge.classList.add('inactive');
            statusDot.classList.remove('active');
            statusDot.classList.add('inactive');
            statusText.textContent = 'Inactivo';
        }
    }

    // Los checkboxes del sistema ya funcionan correctamente
    // Solo agregamos funcionalidad adicional si es necesaria

    // Inicializar cuando el DOM esté listo
    document.addEventListener('DOMContentLoaded', function() {
        // Configurar toggle de estado
        if (statusToggle && statusBadge && statusDot && statusText) {
            // Inicializar estado basado en el checkbox
            actualizarEstado(statusToggle.checked);

            // Event listener para el toggle
            statusToggle.addEventListener('change', function() {
                actualizarEstado(this.checked);
            });
        }

        // Confirmación antes de cancelar
        const cancelBtn = document.getElementById('cancelBtn');
        if (cancelBtn) {
            cancelBtn.addEventListener('click', function(e) {
                // Verificar si hay cambios en el formulario
                const form = this.closest('form');
                if (form) {
                    const nombreEncargado = form.querySelector('input[name="Per_PrimerNombre"]');
                    const ocupacion = form.querySelector('input[name="Enc_Ocupacion"]');

                    // Verificar si hay algún checkbox marcado
                    const checkboxesMarcados = form.querySelectorAll('input[type="checkbox"]:checked').length;

                    const hasChanges = (nombreEncargado && nombreEncargado.value.trim() !== '') ||
                                      (ocupacion && ocupacion.value !== '') ||
                                      checkboxesMarcados > 0;

                    if (hasChanges) {
                        if (!confirm('¿Está seguro de que desea cancelar? Los cambios no guardados se perderán.')) {
                            e.preventDefault();
                            return false;
                        }
                    }
                }
            });
        }

        // Animación de entrada para los acordeones
        const accordionHeaders = document.querySelectorAll('.accordion-header-encargado');
        accordionHeaders.forEach(function(header) {
            header.addEventListener('click', function() {
                // Agregar clase para animación suave
                this.classList.toggle('collapsed');
            });
        });
    });

    // Validación del formulario antes de enviar
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            const nombreEncargado = this.querySelector('input[name="Per_PrimerNombre"]');
            const ocupacion = this.querySelector('input[name="Enc_Ocupacion"]');

            if (!nombreEncargado || nombreEncargado.value.trim() === '') {
                alert('Por favor ingrese el nombre del encargado.');
                e.preventDefault();
                nombreEncargado.focus();
                return false;
            }

            if (!ocupacion || ocupacion.value.trim() === '') {
                alert('Por favor ingrese la ocupación del encargado.');
                e.preventDefault();
                ocupacion.focus();
                return false;
            }

            return true;
        });
    }
})();
