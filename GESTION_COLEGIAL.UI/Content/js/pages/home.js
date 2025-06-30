$(document).ready(function () {

    // Sistema de notificaciones mejorado
    const NotificationManager = {
        errors: [],
        maxErrors: 3, // Máximo de errores antes de mostrar resumen
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

        // Mostrar una sola notificación
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

        // Mostrar toast discreto para múltiples errores
        showMultipleErrorsToast() {
            const message = `Se encontraron ${this.errors.length} problemas al cargar algunos elementos de la página.`;

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

        // Notificación in-page como fallback
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

            // Crear notificación
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

            // Auto-eliminar después de 5 segundos
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

            const table = $('#HomeAndCharts');
            table.find('tbody').empty();

            $.each(response.data, function (index, course) {
                const row = $('<tr>');

                let trendIndicator = '';
                if (course.PorcentajeDiferencia > 0) {
                    trendIndicator = '<i class="bi bi-caret-up-fill text-success"></i>';
                } else {
                    trendIndicator = '<i class="bi bi-caret-down-fill text-danger"></i>';
                }

                row.append(`<td>${trendIndicator} ${course.PorcentajeDiferencia}%</td>`);
                row.append(`<td>${course.NombreCurso}</td>`);
                row.addClass('loading-row');

                setTimeout(function () {
                    table.find('tbody').append(row);
                    row.removeClass('loading-row');
                }, index * 100);
            });

        } catch (error) {
            console.error('HomeAndChartsList - Error detallado:', error);

            // Mostrar placeholder o mensaje discreto en la tabla
            const table = $('#HomeAndCharts');
            table.find('tbody').html(`
                <tr>
                    <td colspan="2" class="text-center text-muted">
                        <i class="bi bi-exclamation-triangle"></i> 
                        No se pudieron cargar los datos
                    </td>
                </tr>
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
            const labels = data.data.map(item => item.Cur_Nombre);
            const cantidades = data.data.map(item => parseInt(item.CantidadAlumnos));

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Cantidad de Alumnos',
                        data: cantidades,
                        backgroundColor: [
                            'rgba(255, 255, 0, 0.6)',
                            'rgba(255, 204, 0, 0.6)',
                            'rgba(255, 235, 59, 0.6)',
                            'rgba(255, 193, 7, 0.6)',
                            'rgba(255, 167, 38, 0.6)',
                            'rgba(205, 220, 57, 0.6)',
                            'rgba(255, 245, 157, 0.6)',
                            'rgba(245, 124, 0, 0.6)',
                            'rgba(255, 224, 130, 0.6)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                usePointStyle: true,
                                pointStyle: 'circle',
                                font: {
                                    size: 10,
                                    weight: '100',
                                    family: 'Arial, sans-serif'
                                },
                            }
                        }
                    },
                    animation: {
                        onComplete: () => {
                            delayed = true;
                        },
                        delay: (context) => {
                            let delay = 0;
                            if (context.type === 'data' && context.mode === 'default' && !delayed) {
                                delay = context.dataIndex * 300 + context.datasetIndex * 100;
                            }
                            return delay;
                        },
                    },
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
                'No se pudieron obtener los datos meteorológicos'
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
                                borderColor: 'rgba(253, 206, 128, 1)',
                                backgroundColor: 'rgba(253, 206, 128, 0.2)',
                                fill: true,
                                tension: 0.1
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                x: {
                                    title: {
                                        display: true,
                                        text: 'Years'
                                    }
                                },
                                y: {
                                    title: {
                                        display: true,
                                        text: 'Promedio Anual'
                                    }
                                }
                            },
                            animation: {
                                onComplete: () => {
                                    delayed = true;
                                },
                                delay: (context) => {
                                    let delay = 0;
                                    if (context.type === 'data' && context.mode === 'default' && !delayed) {
                                        delay = context.dataIndex * 300 + context.datasetIndex * 100;
                                    }
                                    return delay;
                                },
                            },
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

    // Inicializar animación de números
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

    // Inicializar página
    initializePage().catch(error => {
        console.error('Error global al inicializar:', error);
        reportError(
            'Error de Inicialización',
            'Error inesperado al cargar la página'
        );
    });

});