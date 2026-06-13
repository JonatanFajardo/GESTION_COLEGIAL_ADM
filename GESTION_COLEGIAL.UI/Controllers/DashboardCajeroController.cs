using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Caja")]
    public class DashboardCajeroController : BaseController
    {
        public ActionResult Index()
        {
            var usuIdSession = HttpContext.Session != null ? HttpContext.Session["UsuarioId"] : null;
            int usuId = 1;
            if (usuIdSession != null && int.TryParse(usuIdSession.ToString(), out var parsed)) usuId = parsed;
            ViewBag.UsuId = usuId;
            return View();
        }

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        public async Task<ActionResult> ResumenAsync(int? Usu_Id, DateTime? Fecha)
        {
            var usu = Usu_Id ?? 1;
            var fecha = (Fecha ?? DateTime.Today).ToString("yyyy-MM-dd");
            var url = $"DashboardCajero/ResumenAsync?Usu_Id={usu}&Fecha={fecha}";
            var raw = await SendHttpClient.GetRawAsync(url);
            return Content(raw, "application/json");
        }
    }
}
