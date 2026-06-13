using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener cuentas con saldo pendiente de cobro.
    /// </summary>
    public class PendientesCobrarService
    {
        /// <summary>
        /// Obtiene todos los registros con saldo pendiente.
        /// </summary>
        /// <returns>Lista dinámica de pendientes por cobrar.</returns>
        public async Task<List<dynamic>> ListAsync()
        {
            string url = "PendientesCobrar/ListAsync";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
