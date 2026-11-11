using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Secretaría")]
    public class DashboardSecretariaController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        public async Task<ActionResult> ResumenAsync(int? Anio)
        {
            var url = "DashboardSecretaria/ResumenAsync";
            if (Anio.HasValue) url += $"?Anio={Anio.Value}";

            var raw = await SendHttpClient.GetRawAsync(url);
            return Content(raw, "application/json");
        }
    }
}
