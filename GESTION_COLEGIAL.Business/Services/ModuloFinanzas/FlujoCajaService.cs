using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener el flujo de caja proyectado por mes.
    /// </summary>
    public class FlujoCajaService
    {
        private const string _base = "FlujoCaja/";

        /// <summary>
        /// Obtiene el flujo de caja del año indicado.
        /// </summary>
        /// <param name="anio">Año a consultar.</param>
        /// <returns>Lista dinámica por mes con totales cobrados, pendientes y neto.</returns>
        public async Task<List<dynamic>> ListAsync(int anio)
        {
            string url = _base + $"ListAsync?Anio={anio}";
            return await SendHttpClient.GetAsync<dynamic>(url) ?? new List<dynamic>();
        }
    }
}
