using GESTION_COLEGIAL.Business.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con reportes financieros.
    /// </summary>
    public class ReportesFinancierosService
    {
        /// <summary>
        /// Obtiene los ingresos por mes de forma asíncrona.
        /// </summary>
        /// <param name="anio">El año a consultar.</param>
        /// <param name="mes">El mes a consultar.</param>
        /// <returns>Una colección con los datos de ingresos.</returns>
        public async Task<IEnumerable<dynamic>> IngresosPorMesAsync(int anio, int mes)
        {
            string url = $"ReportesFinancieros/IngresosPorMesAsync?anio={anio}&mes={mes}";
            IEnumerable<dynamic> apiUrl = await ApiRequests.ListAsync<dynamic>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene la proyección de cobros de forma asíncrona.
        /// </summary>
        /// <param name="anio">El año a consultar.</param>
        /// <param name="mes">El mes a consultar.</param>
        /// <returns>Una colección con los datos de proyección.</returns>
        public async Task<IEnumerable<dynamic>> ProyeccionCobrosAsync(int anio, int mes)
        {
            string url = $"ReportesFinancieros/ProyeccionCobrosAsync?anio={anio}&mes={mes}";
            IEnumerable<dynamic> apiUrl = await ApiRequests.ListAsync<dynamic>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene el listado de morosos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección con los datos de morosos.</returns>
        public async Task<IEnumerable<dynamic>> ListadoMorososAsync()
        {
            string url = "ReportesFinancieros/ListadoMorososAsync";
            IEnumerable<dynamic> apiUrl = await ApiRequests.ListAsync<dynamic>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene el estado de cuenta de un alumno de forma asíncrona.
        /// </summary>
        /// <param name="alumnoId">El identificador del alumno.</param>
        /// <returns>Una colección con los datos del estado de cuenta.</returns>
        public async Task<IEnumerable<dynamic>> EstadoCuentaAlumnoAsync(int alumnoId)
        {
            string url = $"ReportesFinancieros/EstadoCuentaAlumnoAsync?alumnoId={alumnoId}";
            IEnumerable<dynamic> apiUrl = await ApiRequests.ListAsync<dynamic>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene la comparativa anual de ingresos de forma asíncrona.
        /// </summary>
        /// <param name="anioInicio">El año de inicio.</param>
        /// <param name="anioFin">El año de fin.</param>
        /// <returns>Una colección con los datos comparativos.</returns>
        public async Task<IEnumerable<dynamic>> ComparativaAnualAsync(int anioInicio, int anioFin)
        {
            string url = $"ReportesFinancieros/ComparativaAnualAsync?anioInicio={anioInicio}&anioFin={anioFin}";
            IEnumerable<dynamic> apiUrl = await ApiRequests.ListAsync<dynamic>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene el resumen del dashboard financiero de forma asíncrona.
        /// </summary>
        /// <returns>Un objeto con los datos del dashboard.</returns>
        public async Task<dynamic> DashboardFinancieroAsync()
        {
            string url = "ReportesFinancieros/DashboardFinancieroAsync";
            dynamic apiUrl = await ApiRequests.FindAsync<dynamic>(url, 0);
            return apiUrl;
        }
    }
}
