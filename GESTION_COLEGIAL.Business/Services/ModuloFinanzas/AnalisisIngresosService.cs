using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener el análisis de ingresos vs egresos por concepto.
    /// </summary>
    public class AnalisisIngresosService
    {
        private const string _base = "AnalisisIngresos/";

        /// <summary>
        /// Obtiene el análisis de ingresos del año indicado.
        /// </summary>
        /// <param name="anio">Año a consultar.</param>
        /// <returns>Lista dinámica con cobrado, pendiente y porcentaje de recuperación.</returns>
        public async Task<List<dynamic>> ListAsync(int anio)
        {
            string url = _base + $"ListAsync?Anio={anio}";
            return await SendHttpClient.GetAsync<dynamic>(url) ?? new List<dynamic>();
        }
    }
}
