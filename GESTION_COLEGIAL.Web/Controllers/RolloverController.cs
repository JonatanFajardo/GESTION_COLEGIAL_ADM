using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Rollover de estructura del año lectivo: arrastra períodos, secciones y
    /// ofertas de un año origen al año destino (activo) como borrador.
    /// </summary>
    [SessionManager("Rollover de año lectivo")]
    public class RolloverController : BaseController
    {
        private readonly RolloverService rolloverService = new RolloverService();

        /// <summary>
        /// Ejecuta el rollover origen -> destino (años, Ani_Anio). Re-ejecutable:
        /// la API salta lo que ya exista en destino. Devuelve los conteos copiados.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RollForwardAsync(int origen, int destino)
        {
            var result = await rolloverService.RollForward(origen, destino);
            return AjaxResult(result, result != null);
        }
    }
}
