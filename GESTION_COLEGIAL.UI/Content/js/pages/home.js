$(document).ready(function () {

    async function HomeAndChartsList() {
        // 1. Define the AJAX request details
        const url = 'HomeAndCharts/HomeAndChartsList'; // Replace with the actual URL for your data source
        const dataType = 'json'; // Assuming your data is in JSON format

        try {
            // 2. Initiate the AJAX request
            const response = await $.ajax({
                url: url,
                dataType: dataType,
            });

            // 3. Handle successful data retrieval
            const table = $('#HomeAndCharts');

            // Clear existing table content (optional, adjust as needed)
            table.find('tbody').empty();

            // 4. Loop through data and build table rows dynamically
            $.each(response.data, function (index, course) {
                const row = $('<tr>');
                const test = 'test';

                // 5. Handle trend indicator conditionally
                let trendIndicator = '';
                if (course.PorcentajeDiferencia > 0) {
                    trendIndicator = '<i class="bi bi-caret-up-fill text-success"></i>';
                } else {
                    trendIndicator = '<i class="bi bi-caret-down-fill text-danger"></i>';
                }

                row.append(`<td>${trendIndicator} ${course.PorcentajeDiferencia}%</td>`);
                row.append(`<td>${course.NombreCurso}</td>`);

                // Agregar clase de animación o estilo para resaltar la fila mientras se carga
                row.addClass('loading-row');  // Puedes definir este estilo en tu CSS

                // Usar setTimeout para simular el retraso de carga
                setTimeout(function () {
                    table.find('tbody').append(row);
                    row.removeClass('loading-row');  // Remover clase de animación después de añadir la fila
                }, index * 100); // El retraso depende del índice, creando un efecto secuencial
            });

        } catch (error) {
            console.error('Error fetching data:', error);
            // Handle errors appropriately (e.g., display an error message)
        }


    }

    async function obtenerCantidadAlumnosPorCurso() {
        const url = 'HomeAndCharts/ObtenerCantidadAlumnosPorCursoList'; // La URL de tu servicio
        const ctx = document.getElementById('obtenerCantidadAlumnosPorCurso').getContext('2d'); // Contexto del canvas para el gráfico
        let delayed;

        try {
            // Realizamos la solicitud fetch para obtener los datos
            const response = await fetch(url, {
                method: 'GET', // o 'POST' si lo necesitas
                headers: {
                    'Content-Type': 'application/json', // Si necesitas cabeceras
                }
            });

            // Comprobamos si la respuesta fue exitosa (código de estado 200-299)
            if (!response.ok) {
                throw new Error('Error en la solicitud');
            }

            // Parseamos la respuesta JSON
            const data = await response.json();
            // Obtenemos las etiquetas y cantidades
            const labels = data.data.map(item => item.Cur_Nombre);
            const cantidades = data.data.map(item => parseInt(item.CantidadAlumnos));

            // Crear el gráfico con los datos obtenidos
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
                                usePointStyle: true, // Esto cambia la forma de la leyenda a círculos
                                pointStyle: 'circle', // Define el estilo como círculo
                                font: {
                                    size: 10, // Tamaño de la fuente
                                    weight: '100', // Cambia a light (peso ligero)
                                    family: 'Arial, sans-serif' // Define la familia de la fuente
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
            console.error('Error al obtener los datos:', error);
            // Mostrar mensaje de error al usuario
            alert("Error al cargar los datos. Por favor, inténtelo de nuevo más tarde.");
        }
    }

    // Llamada a la función




    //const apiKey = '565c6073a5c841bd81385441242012'; // Reemplaza con tu clave real
    const apiKey = '565c6073a5c841bd81385441242012'; // Reemplaza con tu clave real
    const query = 'Honduras'; // Puedes cambiar la ubicación aquí
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

    // Función asíncrona para obtener el clima usando AJAX
    async function obtenerDatosClima() { 
        try {
            // Realizamos la solicitud AJAX con await
            const data = await $.ajax({
                url: weatherUrl,
                dataType: 'json'
            });
 

            // Actualizamos el DOM con los datos del clima
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

            // Limpiamos el mensaje de error si la llamada fue exitosa
            errorMessage.text("");
        } catch (error) {
            // Si ocurre algún error, mostramos un mensaje en el DOM
            console.error('Error al obtener los datos del clima:', error);
            errorMessage.text("Error al obtener los datos del clima. Por favor, inténtelo de nuevo más tarde.");
            locationElement.text("Error");
        }
    }

    // Llamar a la función al cargar la página
    async function ObtenerPromedioCursoUltimosAnios() {
        const ctx = document.getElementById('ObtenerPromedioCursoUltimosAnios').getContext('2d');
        const mensajeErrorDiv = document.getElementById('mensajeError');
        let delayed;

        try {
            await $.ajax({
                url: 'HomeAndCharts/ObtenerPromedioCursoUltimosAnios',
                dataType: 'json',
                success: function (response) { 
                    // Procesar los datos
                    const anios = response.data.map(item => item.AnioCursado);
                    const promedios = response.data.map(item => item.PromedioAnual);

                    // Crear la gráfica con Chart.js
                    //const ctx = document.getElementById('graficaLineal').getContext('2d');
                    new Chart(ctx, {
                        type: 'line', // Tipo de gráfica: lineal
                        data: {
                            labels: anios, // Etiquetas del eje X (Años)
                            datasets: [{
                                label: 'Promedio Anual',
                                data: promedios, // Datos para el eje Y
                                borderColor: 'rgba(253, 206, 128, 1)', // Color de la línea rgb()
                                backgroundColor: 'rgba(253, 206, 128, 0.2)', // Color de fondo de los puntos
                                fill: true, // No llenar el área bajo la línea
                                tension: 0.1 // Curvatura de la línea
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
                }, // Cierre de la función success
                error: function (error) {
                    console.error("Error al obtener los datos", error);
                    mensajeErrorDiv.style.display = 'block'; // Mostrar mensaje de error si lo hay
                }
            }); // Cierre de la llamada AJAX
        } catch (error) {
            console.error("Error en la función async", error);
        }
    }




    // Función para animar números
    function animateNumbers() {
        const counters = document.querySelectorAll('.number');
        counters.forEach(counter => {
            const target = +counter.getAttribute('data-target'); // Obtiene el número final
            const increment = target / 200; // Ajusta este valor para la velocidad de incremento
            let current = 0;

            function updateCounter() {
                current += increment;
                if (current < target) {
                    counter.textContent = Math.floor(current);
                    requestAnimationFrame(updateCounter); // Continuar la animación
                } else {
                    counter.textContent = target; // Asegura que termina en el número exacto
                }
            }

            updateCounter();
        });
    }

    animateNumbers();

    async function LoadCards() {

        result = await $.ajax({
            url: 'HomeAndCharts/CardsInHomeList',
            dataType: 'json',
            success: function (response) {
                // Actualizar cards con los valores obtenidos
                $('#graduados').text(response.data[0].Graduados);
                $('#diferenciaPromedioAnual').text(response.data[0].DiferenciaPromedioAnual+"%");
                $('#actualPromedioAnual').text(response.data[0].ActualPromedioAnual);
                $('#diferenciaNuevoIngreso').text(response.data[0].DiferenciaNuevoIngreso + "%");
            },
            error: function () {
                console.log("Error al cargar los datos.");
            }
        });

    }

    //Llamando funciones
    HomeAndChartsList();
    obtenerCantidadAlumnosPorCurso();
    obtenerDatosClima();
    ObtenerPromedioCursoUltimosAnios();
    LoadCards();

});

