using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dev Tools")]
    public class DevToolsController : BaseController
    {
        private readonly DevToolsService _service = new DevToolsService();

        private bool IsAdmin() => Session["RolId"] != null && (int)Session["RolId"] == 1;

        private ActionResult RequireAdmin()
        {
            return new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "SinAcceso" }));
        }

        public ActionResult Index()
        {
            if (!IsAdmin()) return RequireAdmin();
            return View();
        }

        public async Task<ActionResult> DetectarAsync(int? AnioObjetivo = null)
        {
            if (!IsAdmin()) return RequireAdmin();
            var data = await _service.DetectarAsync(AnioObjetivo);
            return AjaxResult(data);
        }

        [HttpPost]
        public async Task<ActionResult> PasoAsync(string Tabla, int Delta)
        {
            if (!IsAdmin()) return RequireAdmin();
            var data = await _service.PasoAsync(Tabla, Delta);
            return AjaxResult(data);
        }

        public async Task<ActionResult> EstadoBDAsync()
        {
            if (!IsAdmin()) return RequireAdmin();
            var data = await _service.EstadoBDAsync();
            return AjaxResult(data);
        }

        public async Task<ActionResult> RolesAsync()
        {
            if (!IsAdmin()) return RequireAdmin();
            var data = await _service.RolesAsync();
            return AjaxResult(data);
        }
    }
}
