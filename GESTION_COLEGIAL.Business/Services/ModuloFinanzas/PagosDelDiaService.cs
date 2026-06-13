using GESTION_COLEGIAL.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener los pagos registrados en una fecha determinada.
    /// </summary>
    public class PagosDelDiaService
    {
        /// <summary>
        /// Obtiene la lista de pagos del día indicado.
        /// </summary>
        /// <param name="fecha">Fecha a consultar.</param>
        /// <returns>Lista dinámica de pagos.</returns>
        public async Task<List<dynamic>> ListAsync(DateTime fecha)
        {
            string url = $"PagosDelDia/ListAsync?Fecha={fecha:yyyy-MM-dd}";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
