// Funcionalidad para el formulario de cursos
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

        statusBadge.classList.toggle('badge-success', activo);
        statusBadge.classList.toggle('badge-secondary', !activo);
        statusText.textContent = activo ? 'Activo' : 'Inactivo';
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
                    const nombreCurso = form.querySelector('input[name="Cur_Nombre"]');
                    const nivelId = form.querySelector('select[name="Niv_Id"]');

                    // Verificar si hay algún checkbox marcado
                    const checkboxesMarcados = form.querySelectorAll('input[type="checkbox"]:checked').length;

                    const hasChanges = (nombreCurso && nombreCurso.value.trim() !== '') ||
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

    });

    // Validación del formulario antes de enviar
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            const nombreCurso = this.querySelector('input[name="Cur_Nombre"]');
            const nivelId = this.querySelector('select[name="Niv_Id"]');

            if (!nombreCurso || nombreCurso.value.trim() === '') {
                alert('Por favor ingrese el nombre del curso.');
                e.preventDefault();
                nombreCurso.focus();
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
