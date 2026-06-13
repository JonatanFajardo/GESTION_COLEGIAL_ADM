using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Administrador")]
    public class DashboardAdminController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        public async Task<ActionResult> ResumenAsync()
        {
            var raw = await SendHttpClient.GetRawAsync("DashboardAdmin/ResumenAsync");
            return Content(raw, "application/json");
        }
    }
}
