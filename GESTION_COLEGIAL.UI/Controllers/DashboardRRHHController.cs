using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Recursos Humanos")]
    public class DashboardRRHHController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        public async Task<ActionResult> ResumenAsync(int? Anio, int? Mes)
        {
            var anio = Anio ?? DateTime.Now.Year;
            var mes = Mes ?? DateTime.Now.Month;

            var url = $"DashboardRRHH/ResumenAsync?Anio={anio}&Mes={mes}";

            var raw = await SendHttpClient.GetRawAsync(url);
            return Content(raw, "application/json");
        }
    }
}
