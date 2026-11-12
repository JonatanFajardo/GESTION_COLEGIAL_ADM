using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio de Rollover (consume la API externa Rollover/RollForwardAsync).
    /// Arrastra la estructura de un año lectivo a otro como borrador.
    /// </summary>
    public class RolloverService
    {
        /// <summary>
        /// Copia la estructura del año <paramref name="origen"/> al año <paramref name="destino"/> (ambos = Ani_Anio).
        /// La API expone un POST con los parámetros en la querystring; se envía un
        /// cuerpo vacío. Devuelve los conteos, o null si la API respondió error.
        /// </summary>
        public async Task<RolloverResultViewModel> RollForward(int origen, int destino)
        {
            string url = $"Rollover/RollForwardAsync?origen={origen}&destino={destino}";
            return await ApiRequests.PostAsyncWithResponse<RolloverResultViewModel>(url, new { });
        }
    }
}
