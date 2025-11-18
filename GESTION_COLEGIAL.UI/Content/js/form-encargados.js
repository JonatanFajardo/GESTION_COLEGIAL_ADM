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
            statusText.textContent = 'Estado Activo';
        } else {
            statusBadge.classList.remove('active');
            statusBadge.classList.add('inactive');
            statusDot.classList.remove('active');
            statusDot.classList.add('inactive');
            statusText.textContent = 'Estado Inactivo';
        }
    }

    // Inicializar cuando el DOM esté listo
    document.addEventListener('DOMContentLoaded', function() {
        // Verificar que los elementos existen
        if (!statusToggle || !statusBadge || !statusDot || !statusText) {
            console.warn('No se encontraron todos los elementos necesarios para el toggle de estado');
            return;
        }

        // Inicializar estado basado en el checkbox
        actualizarEstado(statusToggle.checked);

        // Event listener para el toggle
        statusToggle.addEventListener('change', function() {
            actualizarEstado(this.checked);
        });

        // Validación en tiempo real para el correo electrónico
        const correoInput = document.querySelector('input[name="Per_CorreoElectronico"]');
        if (correoInput) {
            correoInput.addEventListener('blur', function() {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (this.value && !emailRegex.test(this.value)) {
                    this.style.borderColor = '#ef4444';
                } else {
                    this.style.borderColor = '#cbd5e1';
                }
            });
        }

        // Validación para teléfono (solo números)
        const telefonoInput = document.querySelector('input[name="Per_Telefono"]');
        if (telefonoInput) {
            telefonoInput.addEventListener('input', function() {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
        }

        // Validación para identidad (solo números)
        const identidadInput = document.querySelector('input[name="Per_Identidad"]');
        if (identidadInput) {
            identidadInput.addEventListener('input', function() {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
        }

        // Confirmación antes de cancelar
        const cancelBtn = document.getElementById('cancelBtn');
        if (cancelBtn) {
            cancelBtn.addEventListener('click', function(e) {
                // Verificar si hay cambios en el formulario
                const form = this.closest('form');
                if (form) {
                    const hasChanges = Array.from(form.elements).some(function(element) {
                        if (element.type === 'text' || element.type === 'date' || element.type === 'email' || element.tagName === 'SELECT') {
                            return element.value.trim() !== '';
                        }
                        return false;
                    });

                    if (hasChanges) {
                        if (!confirm('¿Está seguro de que desea cancelar? Los cambios no guardados se perderán.')) {
                            e.preventDefault();
                            return false;
                        }
                    }
                }
            });
        }
    });
})();
