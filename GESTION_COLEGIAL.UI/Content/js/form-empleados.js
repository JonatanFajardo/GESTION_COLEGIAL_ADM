// Funcionalidad para el formulario de empleados
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
                    const nombreAlumno = form.querySelector('input[name="Alu_Nombre"]');
                    const nivelId = form.querySelector('select[name="Niv_Id"]');

                    // Verificar si hay algún checkbox marcado
                    const checkboxesMarcados = form.querySelectorAll('input[type="checkbox"]:checked').length;

                    const hasChanges = (nombreAlumno && nombreAlumno.value.trim() !== '') ||
                                      (nivelId && nivelId.value !== '') ||
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
        const accordionHeaders = document.querySelectorAll('.accordion-header-empleado');
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
            const nombreAlumno = this.querySelector('input[name="Alu_Nombre"]');
            const nivelId = this.querySelector('select[name="Niv_Id"]');

            if (!nombreAlumno || nombreAlumno.value.trim() === '') {
                alert('Por favor ingrese el nombre del empleado.');
                e.preventDefault();
                nombreAlumno.focus();
                return false;
            }

            if (!nivelId || nivelId.value === '') {
                alert('Por favor seleccione un nivel educativo.');
                e.preventDefault();
                nivelId.focus();
                return false;
            }

            return true;
        });
    }
})();
