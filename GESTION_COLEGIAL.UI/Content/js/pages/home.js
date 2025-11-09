$(document).ready(function () {

    // Sistema de notificaciones mejorado
    const NotificationManager = {
        errors: [],
        maxErrors: 3, // M�ximo de errores antes de mostrar resumen
        showTimeout: null,

        // Agregar error al stack
        addError(title, message, severity = 'error') {
            this.errors.push({ title, message, severity, timestamp: Date.now() });
            this.processErrors();
        },

        // Procesar errores acumulados
        processErrors() {
            clearTimeout(this.showTimeout);

            // Esperar 2 segundos para acumular errores similares
            this.showTimeout = setTimeout(() => {
                this.showNotifications();
            }, 2000);
        },

        // Mostrar notificaciones de forma inteligente
        showNotifications() {
            if (this.errors.length === 0) return;

            if (this.errors.length === 1) {
                // Solo un error - mostrar normal
                this.showSingleNotification(this.errors[0]);
            } else if (this.errors.length <= this.maxErrors) {
                // Pocos errores - mostrar toast discreto
                this.showMultipleErrorsToast();
            } else {
                // Muchos errores - mostrar resumen
                this.showErrorSummary();
            }

            this.errors = []; // Limpiar errores procesados
        },

        // Mostrar una sola notificaci�n
        showSingleNotification(error) {
            if (typeof toastr !== 'undefined') {
                toastr.error(error.message, error.title, {
                    timeOut: 5000,
                    closeButton: true,
                    progressBar: true
                });
            } else if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: error.title,
                    text: error.message,
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 5000,
                    timerProgressBar: true
                });
            } else {
                this.showInPageNotification(error.title, error.message, 'error');
            }
        },

        // Mostrar toast discreto para m�ltiples errores
        showMultipleErrorsToast() {
            const message = `Se encontraron ${this.errors.length} problemas al cargar algunos elementos de la p�gina.`;

            if (typeof toastr !== 'undefined') {
                toastr.warning(message, 'Algunos componentes no se cargaron', {
                    timeOut: 7000,
                    closeButton: true,
                    progressBar: true,
                    onclick: () => this.showErrorDetails()
                });
            } else if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Algunos componentes no se cargaron',
                    text: message + ' Haz clic para ver detalles.',
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: true,
                    confirmButtonText: 'Ver detalles',
                    timer: 7000,
                    timerProgressBar: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        this.showErrorDetails();
                    }
                });
            } else {
                this.showInPageNotification(
                    'Algunos componentes no se cargaron',
                    message,
                    'warning'
                );
            }
        },

        // Mostrar resumen de errores
        showErrorSummary() {
            const message = `Se encontraron ${this.errors.length} problemas al cargar la pagina. Algunos elementos pueden no estar disponibles.`;

            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Problemas de Carga Detectados',
                    text: message,
                    showCancelButton: true,
                    confirmButtonText: 'Ver detalles',
                    cancelButtonText: 'Continuar',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        this.showErrorDetails();
                    }
                });
            } else {
                this.showInPageNotification(
                    'Problemas de Carga Detectados',
                    message,
                    'warning'
                );
            }
        },

        // Mostrar detalles de errores
        showErrorDetails() {
            let detailsHtml = '<div class="error-details" style="text-align: left; max-height: 300px; overflow-y: auto;">';

            this.errors.forEach((error, index) => {
                detailsHtml += `
                    <div style="margin-bottom: 10px; padding: 8px; border-left: 3px solid #dc3545; background: #f8f9fa;">
                        <strong>${error.title}</strong><br>
                        <small>${error.message}</small>
                    </div>
                `;
            });

            detailsHtml += '</div>';

            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Detalles de los Errores',
                    html: detailsHtml,
                    width: '600px',
                    confirmButtonText: 'Entendido'
                });
            } else {
                alert('Detalles de errores:\n' + this.errors.map(e => `${e.title}: ${e.message}`).join('\n'));
            }
        },

        // Notificaci�n in-page como fallback
        showInPageNotification(title, message, type = 'error') {
            // Crear contenedor si no existe
            let container = document.getElementById('notification-container');
            if (!container) {
                container = document.createElement('div');
                container.id = 'notification-container';
                container.style.cssText = `
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 9999;
                    max-width: 350px;
                `;
                document.body.appendChild(container);
            }

            // Crear notificaci�n
            const notification = document.createElement('div');
            const bgColor = type === 'error' ? '#dc3545' : '#ffc107';
            const textColor = type === 'error' ? 'white' : '#212529';

            notification.style.cssText = `
                background-color: ${bgColor};
                color: ${textColor};
                padding: 15px;
                margin-bottom: 10px;
                border-radius: 5px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.2);
                cursor: pointer;
                transition: all 0.3s ease;
            `;

            notification.innerHTML = `
                <strong>${title}</strong><br>
                <small>${message}</small>
                <div style="position: absolute; top: 5px; right: 10px; font-size: 18px;">&times;</div>
            `;

            // Auto-eliminar despu�s de 5 segundos
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.style.opacity = '0';
                    setTimeout(() => notification.remove(), 300);
                }
            }, 5000);

            // Eliminar al hacer clic
            notification.onclick = () => {
                notification.style.opacity = '0';
                setTimeout(() => notification.remove(), 300);
            };

            container.appendChild(notification);
        }
    };

    // funcion para reportar errores
    function reportError(title, message, severity = 'error') {
        // Log para desarrolladores
        console.error(`${title}: ${message}`);

        // Agregar al sistema de notificaciones
        NotificationManager.addError(title, message, severity);
    }

    // Funciones principales con manejo de errores mejorado
    async function HomeAndChartsList() {
        const url = 'HomeAndCharts/HomeAndChartsList';
        const dataType = 'json';

        try {
            const response = await $.ajax({
                url: url,
                dataType: dataType,
            });

            const container = $('#HomeAndCharts');
            container.empty();

            $.each(response.data, function (index, course) {
                const changeClass = course.PorcentajeDiferencia < 0 ? 'negative' : 'positive';
                const icon = course.PorcentajeDiferencia < 0 ?
                    '<i class="bi bi-graph-down-arrow"></i>' :
                    '<i class="bi bi-graph-up-arrow"></i>';

                const item = $(`
                    <div class="enrollment-item-modern">
                        <div class="enrollment-name-modern">${course.NombreCurso}</div>
                        <div class="enrollment-change-modern ${changeClass}">
                            ${icon}
                            <span>${course.PorcentajeDiferencia}%</span>
                        </div>
                    </div>
                `);

                setTimeout(function () {
                    container.append(item);
                }, index * 50);
            });

        } catch (error) {
            console.error('HomeAndChartsList - Error detallado:', error);

            // Mostrar placeholder o mensaje discreto
            const container = $('#HomeAndCharts');
            container.html(`
                <div class="text-center text-muted p-4">
                    <i class="bi bi-exclamation-triangle"></i>
                    No se pudieron cargar los datos
                </div>
            `);

            reportError(
                'Lista de Cursos',
                'No se pudieron cargar los datos de cursos'
            );
        }
    }

    async function obtenerCantidadAlumnosPorCurso() {
        const ctx = document.getElementById('obtenerCantidadAlumnosPorCurso')?.getContext('2d');
        let delayed;

        if (!ctx) {
            console.error('obtenerCantidadAlumnosPorCurso - Canvas element not found');
            return;
        }

        try {
            const response = await fetch('HomeAndCharts/ObtenerCantidadAlumnosPorCursoList', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP Error: ${response.status} - ${response.statusText}`);
            }

            const data = await response.json();
            const labels = data.data.map(item => item.NombreCurso);
            const cantidades = data.data.map(item => parseInt(item.CantidadAlumnos));

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Cantidad de Alumnos',
                        data: cantidades,
                        backgroundColor: [
                            '#fb923c',  // orange
                            '#fbbf24',  // amber
                            '#fde047',  // yellow
                            '#a3e635',  // lime
                            '#4ade80',  // green
                            '#34d399',  // emerald
                            '#2dd4bf',  // teal
                            '#22d3ee',  // cyan
                            '#38bdf8'   // blue
                        ],
                        borderColor: '#ffffff',
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                pointStyle: 'circle',
                                padding: 12,
                                font: {
                                    size: 11,
                                    family: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
                                },
                                color: '#64748b'
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(15, 23, 42, 0.9)',
                            padding: 12,
                            cornerRadius: 8,
                            titleFont: {
                                size: 13,
                                weight: '600'
                            },
                            bodyFont: {
                                size: 12
                            }
                        }
                    },
                    animation: {
                        animateRotate: true,
                        animateScale: true,
                        duration: 1000,
                        easing: 'easeInOutQuart'
                    },
                    cutout: '50%'
                }
            });
        } catch (error) {
            console.error('obtenerCantidadAlumnosPorCurso - Error detallado:', error);

            // Mostrar mensaje en el canvas
            const canvas = document.getElementById('obtenerCantidadAlumnosPorCurso');
            if (canvas) {
                canvas.style.display = 'none';
                const container = canvas.parentElement;
                if (container) {
                    const errorDiv = document.createElement('div');
                    errorDiv.className = 'text-center text-muted p-4';
                    errorDiv.innerHTML = `
                        <i class="bi bi-exclamation-triangle fs-1"></i><br>
                        <small>Grafico no disponible</small>
                    `;
                    container.appendChild(errorDiv);
                }
            }

            reportError(
                'Grafico de Alumnos',
                'No se pudo cargar el Grafico de cantidad de alumnos'
            );
        }
    }

    const apiKey = '565c6073a5c841bd81385441242012';
    const query = 'Honduras';
    const weatherUrl = `https://api.weatherapi.com/v1/current.json?key=${apiKey}&q=${query}&aqi=no`;

    // Elementos del DOM
    const locationElement = $('#location');
    const weatherIcon = $('#weather-icon');
    const tempC = $('#temp-c');
    const weatherCondition = $('#weather-condition');
    const feelsLike = $('#feels-like');
    const humidity = $('#humidity');
    const windSpeed = $('#wind-speed');
    const windDirection = $('#wind-direction');
    const visibility = $('#visibility');
    const errorMessage = $('#error-message');

    async function obtenerDatosClima() {
        try {
            const data = await $.ajax({
                url: weatherUrl,
                dataType: 'json'
            });

            locationElement.text(`${data.location.name}, ${data.location.country}`);
            weatherIcon.attr('src', data.current.condition.icon);
            weatherIcon.attr('alt', data.current.condition.text);
            tempC.text(data.current.temp_c);
            weatherCondition.text(data.current.condition.text);
            feelsLike.text(data.current.feelslike_c);
            humidity.text(data.current.humidity);
            windSpeed.text(data.current.wind_kph);
            windDirection.text(data.current.wind_dir);
            visibility.text(data.current.vis_km);

            errorMessage.text("");
        } catch (error) {
            console.error('obtenerDatosClima - Error detallado:', error);

            // Mostrar datos placeholder
            locationElement.text("Honduras");
            tempC.text("--");
            weatherCondition.text("No disponible");
            feelsLike.text("--");
            humidity.text("--");
            windSpeed.text("--");
            windDirection.text("--");
            visibility.text("--");

            // Solo mostrar error en el elemento designado, no popup
            errorMessage.text("Datos del clima no disponibles");

            reportError(
                'Datos del Clima',
                'No se pudieron obtener los datos meteorol�gicos'
            );
        }
    }

    async function ObtenerPromedioCursoUltimosAnios() {
        const ctx = document.getElementById('ObtenerPromedioCursoUltimosAnios')?.getContext('2d');
        const mensajeErrorDiv = document.getElementById('mensajeError');
        let delayed;

        if (!ctx) {
            console.error('ObtenerPromedioCursoUltimosAnios - Canvas element not found');
            return;
        }

        try {
            await $.ajax({
                url: 'HomeAndCharts/ObtenerPromedioCursoUltimosAnios',
                dataType: 'json',
                success: function (response) {
                    const anios = response.data.map(item => item.AnioCursado);
                    const promedios = response.data.map(item => item.PromedioAnual);

                    new Chart(ctx, {
                        type: 'line',
                        data: {
                            labels: anios,
                            datasets: [{
                                label: 'Promedio Anual',
                                data: promedios,
                                borderColor: '#fb923c',
                                backgroundColor: 'rgba(251, 146, 60, 0.1)',
                                fill: true,
                                tension: 0.4,
                                borderWidth: 3,
                                pointRadius: 5,
                                pointBackgroundColor: '#fb923c',
                                pointBorderColor: '#ffffff',
                                pointBorderWidth: 2,
                                pointHoverRadius: 7,
                                pointHoverBackgroundColor: '#fb923c',
                                pointHoverBorderColor: '#ffffff',
                                pointHoverBorderWidth: 3
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: true,
                            plugins: {
                                legend: {
                                    display: false
                                },
                                tooltip: {
                                    backgroundColor: 'rgba(15, 23, 42, 0.9)',
                                    padding: 12,
                                    cornerRadius: 8,
                                    titleFont: {
                                        size: 13,
                                        weight: '600'
                                    },
                                    bodyFont: {
                                        size: 12
                                    },
                                    callbacks: {
                                        label: function (context) {
                                            return 'Promedio: ' + context.parsed.y;
                                        }
                                    }
                                }
                            },
                            scales: {
                                x: {
                                    grid: {
                                        display: true,
                                        color: 'rgba(226, 232, 240, 0.5)',
                                        drawBorder: false
                                    },
                                    ticks: {
                                        color: '#64748b',
                                        font: {
                                            size: 12
                                        }
                                    }
                                },
                                y: {
                                    grid: {
                                        display: true,
                                        color: 'rgba(226, 232, 240, 0.5)',
                                        drawBorder: false
                                    },
                                    ticks: {
                                        color: '#64748b',
                                        font: {
                                            size: 12
                                        }
                                    }
                                }
                            },
                            animation: {
                                duration: 1500,
                                easing: 'easeInOutQuart'
                            }
                        }
                    });
                },
                error: function (xhr, status, error) {
                    console.error("ObtenerPromedioCursoUltimosAnios - Error detallado:", {
                        xhr: xhr,
                        status: status,
                        error: error
                    });

                    if (mensajeErrorDiv) {
                        mensajeErrorDiv.style.display = 'block';
                    }

                    // Mostrar placeholder en el canvas
                    const canvas = document.getElementById('ObtenerPromedioCursoUltimosAnios');
                    if (canvas) {
                        canvas.style.display = 'none';
                        const container = canvas.parentElement;
                        if (container) {
                            const errorDiv = document.createElement('div');
                            errorDiv.className = 'text-center text-muted p-4';
                            errorDiv.innerHTML = `
                                <i class="bi bi-exclamation-triangle fs-1"></i><br>
                                <small>Grafico no disponible</small>
                            `;
                            container.appendChild(errorDiv);
                        }
                    }

                    reportError(
                        'Grafico de Promedios',
                        'No se pudo cargar el Grafico de promedios'
                    );
                }
            });
        } catch (error) {
            console.error("ObtenerPromedioCursoUltimosAnios - Error en funcion async:", error);

            reportError(
                'Grafico de Promedios',
                'Error inesperado al cargar el Grafico de promedios'
            );
        }
    }

    function animateNumbers() {
        const counters = document.querySelectorAll('.number');
        counters.forEach(counter => {
            const target = +counter.getAttribute('data-target');
            const increment = target / 200;
            let current = 0;

            function updateCounter() {
                current += increment;
                if (current < target) {
                    counter.textContent = Math.floor(current);
                    requestAnimationFrame(updateCounter);
                } else {
                    counter.textContent = target;
                }
            }

            updateCounter();
        });
    }

    async function LoadCards() {
        try {
            const result = await $.ajax({
                url: 'HomeAndCharts/CardsInHomeList',
                dataType: 'json'
            });

            $('#graduados').text(result.data[0].Graduados);
            $('#diferenciaPromedioAnual').text(result.data[0].DiferenciaPromedioAnual + "%");
            $('#actualPromedioAnual').text(result.data[0].ActualPromedioAnual);
            $('#diferenciaNuevoIngreso').text(result.data[0].DiferenciaNuevoIngreso + "%");

        } catch (error) {
            console.error("LoadCards - Error detallado:", error);

            // Mostrar valores placeholder en las tarjetas
            $('#graduados').text('--');
            $('#diferenciaPromedioAnual').text('--%');
            $('#actualPromedioAnual').text('--');
            $('#diferenciaNuevoIngreso').text('--%');

            reportError(
                'Tarjetas de Datos',
                'No se pudieron cargar los datos de las tarjetas'
            );
        }
    }

    // Inicializar animaci�n de n�meros
    animateNumbers();

    // Llamar funciones de forma secuencial para mejor control
    async function initializePage() {
        const functions = [
            { name: 'HomeAndChartsList', fn: HomeAndChartsList },
            { name: 'obtenerCantidadAlumnosPorCurso', fn: obtenerCantidadAlumnosPorCurso },
            { name: 'obtenerDatosClima', fn: obtenerDatosClima },
            { name: 'ObtenerPromedioCursoUltimosAnios', fn: ObtenerPromedioCursoUltimosAnios },
            { name: 'LoadCards', fn: LoadCards }
        ];

        // Ejecutar todas las funciones
        const promises = functions.map(({ fn }) =>
            fn().catch(error => {
                console.error(`Error in function:`, error);
                return null; // Continuar con otras funciones
            })
        );

        await Promise.allSettled(promises);
    }

    // Inicializar p�gina
    initializePage().catch(error => {
        console.error('Error global al inicializar:', error);
        reportError(
            'Error de Inicializaci�n',
            'Error inesperado al cargar la p�gina'
        );
    });

});
