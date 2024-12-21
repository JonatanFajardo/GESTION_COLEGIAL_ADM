using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HomeAndChartsService
    {
        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de alumnos.</returns>
        public async Task<IEnumerable<HomeAndChartsViewModel>> HomeAndCharts()
        {
            string url = "HomeAndCharts/HomeAndChartsList";
            IEnumerable<HomeAndChartsViewModel> apiUrl = await ApiRequests.ListAsync<HomeAndChartsViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de alumnos.</returns>
        public async Task<IEnumerable<ObtenerCantidadAlumnosPorCursoViewModel>> ObtenerCantidadAlumnosPorCursoList()
        {
            string url = "HomeAndCharts/ObtenerCantidadAlumnosPorCursoList";
            IEnumerable<ObtenerCantidadAlumnosPorCursoViewModel> apiUrl = await ApiRequests.ListAsync<ObtenerCantidadAlumnosPorCursoViewModel>(url);
            return apiUrl;
        }
    }
}
