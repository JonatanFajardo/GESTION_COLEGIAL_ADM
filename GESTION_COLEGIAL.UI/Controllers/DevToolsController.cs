using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public async Task<ActionResult> ImpersonarAsync(int rolId)
        {
            if (!IsAdmin()) return RequireAdmin();

            var rolesService = new RolesService();
            var rol = await rolesService.Find(rolId);
            if (rol == null)
            {
                TempData["ImpError"] = "Rol no encontrado (id=" + rolId + ")";
                return RedirectToAction("Index");
            }

            var todasPantallas = (await rolesService.PantallasListAsync()) ?? new List<PantallaItemViewModel>();
            var pantallasIds   = (await rolesService.PantallasByRolAsync(rolId)) ?? new List<PantallaIdViewModel>();
            var ids = new HashSet<int>(pantallasIds.Select(p => p.Pan_Id));
            var nombres = todasPantallas.Where(p => ids.Contains(p.Pan_Id)).Select(p => p.Pan_Descripcion).ToList();

            // Guardia critica: si el rol no tiene pantallas, NO impersonar (evita logout en reload)
            if (nombres.Count == 0)
            {
                TempData["ImpError"] = "El rol '" + rol.Rol_Descripcion + "' no tiene pantallas asignadas.";
                return RedirectToAction("Index");
            }

            // Siempre incluir 'Home' para que el dashboard cargue
            if (!nombres.Any(n => n.Equals("Home", System.StringComparison.OrdinalIgnoreCase)))
                nombres.Insert(0, "Home");

            var pantallasStr = string.Join(",", nombres);

            // Guardar las pantallas originales del admin la PRIMERA vez que se impersona
            if (Session["_admin_pantallas"] == null)
                Session["_admin_pantallas"] = Session["pantallas"];

            // OJO: NO cambiar Session["RoleName"] ni Session["RolId"] para conservar acceso a DevTools
            Session["pantallas"] = pantallasStr;
            Session["impersonando_rol"] = rol.Rol_Descripcion;

            TempData["ImpOk"] = "Ahora viendo como: " + rol.Rol_Descripcion;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult RestaurarAsync()
        {
            if (!IsAdmin()) return RequireAdmin();

            if (Session["_admin_pantallas"] != null)
            {
                Session["pantallas"] = Session["_admin_pantallas"];
                Session.Remove("_admin_pantallas");
            }
            Session.Remove("impersonando_rol");

            return RedirectToAction("Index");
        }
    }
}
