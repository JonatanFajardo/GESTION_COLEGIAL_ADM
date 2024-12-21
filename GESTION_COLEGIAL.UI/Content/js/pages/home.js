$(document).ready(function () {
    console.log('aksjdnkajsbdkb');



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
            console.log(response.data);

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
                table.find('tbody').append(row);
            });
        } catch (error) {
            console.error('Error fetching data:', error);
            // Handle errors appropriately (e.g., display an error message)
        }


    }

    HomeAndChartsList();
    async function obtenerCantidadAlumnosPorCurso() {
        const url = 'HomeAndCharts/ObtenerCantidadAlumnosPorCursoList'; // La URL de tu servicio
        const ctx = document.getElementById('obtenerCantidadAlumnosPorCurso').getContext('2d'); // Contexto del canvas para el gráfico

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
                            'rgba(255, 99, 132, 0.6)',
                            'rgba(54, 162, 235, 0.6)',
                            'rgba(255, 206, 86, 0.6)',
                            'rgba(75, 192, 192, 0.6)',
                            'rgba(153, 102, 255, 0.6)',
                            'rgba(255, 159, 64, 0.6)',
                            'rgba(201, 203, 207, 0.6)',
                            'rgba(0, 123, 255, 0.6)',
                            'rgba(128, 0, 128, 0.6)'
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
                            position: 'right'
                        }
                    }
                }
            });
        } catch (error) {
            console.error('Error al obtener los datos:', error);
            // Mostrar mensaje de error al usuario
            alert("Error al cargar los datos. Por favor, inténtelo de nuevo más tarde.");
        }
    }

    // Llamada a la función
    obtenerCantidadAlumnosPorCurso();




    //const apiKey = '565c6073a5c841bd81385441242012'; // Reemplaza con tu clave real
    //const query = 'Honduras'; // You can change the query here

    //const weatherUrl = `http://api.weatherapi.com/v1/current.json?key=${apiKey}&q=${query}&aqi=no`;

    //const locationElement = $('#location');
    //const weatherIcon = $('#weather-icon');
    //const tempC = $('#temp-c');
    //const weatherCondition = $('#weather-condition');
    //const feelsLike = $('#feels-like');
    //const humidity = $('#humidity');
    //const windSpeed = $('#wind-speed');
    //const windDirection = $('#wind-direction');
    //const visibility = $('#visibility');
    //const errorMessage = $('#error-message');

    //$.ajax({
    //    url: weatherUrl,
    //    dataType: 'json',
    //    success: function (data) {
    //        locationElement.text(`${data.location.name}, ${data.location.country}`);
    //        weatherIcon.attr('src', data.current.condition.icon);
    //        weatherIcon.attr('alt', data.current.condition.text);
    //        tempC.text(data.current.temp_c);
    //        weatherCondition.text(data.current.condition.text);
    //        feelsLike.text(data.current.feelslike_c);
    //        humidity.text(data.current.humidity);
    //        windSpeed.text(data.current.wind_kph);
    //        windDirection.text(data.current.wind_dir);
    //        visibility.text(data.current.vis_km);
    //        errorMessage.text("");
    //    },
    //    error: function (jqXHR, textStatus, errorThrown) {
    //        console.error('Error fetching data:', textStatus, errorThrown);
    //        errorMessage.text("Error al obtener los datos del clima. Por favor, inténtelo de nuevo más tarde.");
    //        locationElement.text("Error");
    //    }
    //});


});

