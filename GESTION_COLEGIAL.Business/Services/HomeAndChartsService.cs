using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HomeAndChartsService
    {
        /// <summary>
        /// Recupera una lista de datos para la página principal y gráficos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea asincrónica que contiene una colección de datos para la página principal y gráficos.</returns>
        public async Task<IEnumerable<HomeAndChartsViewModel>> HomeAndCharts()
        {
            string url = "HomeAndCharts/HomeAndChartsList";
            IEnumerable<HomeAndChartsViewModel> apiUrl = await ApiRequests.ListAsync<HomeAndChartsViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene la cantidad de alumnos por curso de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea asincrónica que contiene una lista con el número de alumnos por curso.</returns>
        public async Task<IEnumerable<ObtenerCantidadAlumnosPorCursoViewModel>> ObtenerCantidadAlumnosPorCursoList()
        {
            string url = "HomeAndCharts/ObtenerCantidadAlumnosPorCursoList";
            IEnumerable<ObtenerCantidadAlumnosPorCursoViewModel> apiUrl = await ApiRequests.ListAsync<ObtenerCantidadAlumnosPorCursoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Recupera el promedio de calificaciones de los cursos en los últimos años de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea asincrónica que contiene una lista con los promedios de curso en los últimos años.</returns>
        public async Task<IEnumerable<ObtenerPromedioCursoUltimosAniosViewModel>> ObtenerPromedioCursoUltimosAnios()
        {
            string url = "HomeAndCharts/ObtenerPromedioCursoUltimosAnios";
            IEnumerable<ObtenerPromedioCursoUltimosAniosViewModel> apiUrl = await ApiRequests.ListAsync<ObtenerPromedioCursoUltimosAniosViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Recupera una lista de tarjetas con información destacada para la página principal de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea asincrónica que contiene una lista de tarjetas informativas para la página principal.</returns>
        public async Task<IEnumerable<CardsInHomeViewModel>> CardsInHomeList()
        {
            string url = "HomeAndCharts/CardsInHomeList";
            IEnumerable<CardsInHomeViewModel> apiUrl = await ApiRequests.ListAsync<CardsInHomeViewModel>(url);
            return apiUrl;
        }

    }
}
