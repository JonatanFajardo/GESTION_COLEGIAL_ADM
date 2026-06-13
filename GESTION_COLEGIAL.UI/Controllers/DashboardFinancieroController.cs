using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Financiero")]
    public class DashboardFinancieroController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        public async Task<ActionResult> ResumenAsync(int? Anio, int? Mes)
        {
            var url = "DashboardFinanciero/ResumenAsync";
            if (Anio.HasValue && Mes.HasValue)
                url += $"?Anio={Anio.Value}&Mes={Mes.Value}";

            var raw = await SendHttpClient.GetRawAsync(url);
            return Content(raw, "application/json");
        }
    }
}
