// Toggle password visibility
document.addEventListener('DOMContentLoaded', function() {
    const togglePassword = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('Password');
    const eyeIcon = document.getElementById('eyeIcon');
    const eyeOffIcon = document.getElementById('eyeOffIcon');

    if (togglePassword && passwordInput) {
        togglePassword.addEventListener('click', function() {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);

            // Toggle icons
            eyeIcon.classList.toggle('hidden');
            eyeOffIcon.classList.toggle('hidden');
        });
    }

    // Form submission animation
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const submitButton = loginForm.querySelector('.submit-button');
            if (submitButton) {
                submitButton.style.transform = 'scale(0.95)';
                setTimeout(() => {
                    submitButton.style.transform = 'scale(1)';
                }, 100);
            }
        });
    }

    // ===== Acceso demo para reclutadores =====
    // Todos los usuarios de prueba comparten la contraseña "admin".
    const demoRoles = {
        admin:     { user: 'jona',            pass: 'admin', desc: 'Acceso total: catálogos, usuarios, matriz de roles y bitácora.' },
        director:  { user: 'director_test',   pass: 'admin', desc: 'Dashboards académicos, rendimiento por sección y alumnos en riesgo.' },
        docente:   { user: 'docente_test',    pass: 'admin', desc: 'Usuario real: ligado a un docente con horario y clases reales. Cuaderno de notas, pase de lista, tareas y mis alumnos.' },
        rrhh:      { user: 'rrhh_test',       pass: 'admin', desc: 'Empleados, asistencia, nómina docente y cumplimiento documental.' },
        cajero:    { user: 'cajero_test',     pass: 'admin', desc: 'Pagos, recibos, cuentas por cobrar y arqueo de caja.' },
        contador:  { user: 'contador_test',   pass: 'admin', desc: 'Flujo de caja, aging, top deudores y proyección de cobros.' },
        secretaria:{ user: 'secretaria_test', pass: 'admin', desc: 'Gestión de alumnos, encargados y matrícula.' },
        consulta:  { user: 'consulta_test',   pass: 'admin', desc: 'Vista de solo lectura de los módulos del sistema.' }
    };

    const demoRole = document.getElementById('demoRole');
    const usernameInput = document.getElementById('Username');
    const demoRoleMeta = document.getElementById('demoRoleMeta');
    const demoRoleMetaText = document.getElementById('demoRoleMetaText');

    if (demoRole && usernameInput && passwordInput) {
        demoRole.addEventListener('change', function () {
            const role = demoRoles[demoRole.value];
            if (!role) return;

            usernameInput.value = role.user;
            passwordInput.value = role.pass;

            if (demoRoleMeta && demoRoleMetaText) {
                demoRoleMetaText.textContent = role.desc;
                demoRoleMeta.classList.add('show');
            }

            [usernameInput, passwordInput].forEach(function (el) {
                el.classList.remove('demo-autofilled');
                void el.offsetWidth; // reinicia la animación
                el.classList.add('demo-autofilled');
            });
        });
    }

    // Add input focus animations
    const inputs = document.querySelectorAll('.input-wrapper input');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.style.transform = 'translateY(-2px)';
            this.parentElement.style.transition = 'transform 0.2s';
        });

        input.addEventListener('blur', function() {
            this.parentElement.style.transform = 'translateY(0)';
        });
    });
});
