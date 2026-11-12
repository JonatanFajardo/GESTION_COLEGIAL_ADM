using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Home")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var impersonando = Session["impersonando_rol"] as string;
            if (!string.IsNullOrEmpty(impersonando))
            {
                var controller = ResolveDashboardControllerByRoleName(impersonando);
                if (controller != null) return RedirectToAction("Index", controller);
            }

            if (Session["RolId"] != null && int.TryParse(Session["RolId"].ToString(), out int rolId))
            {
                switch (rolId)
                {
                    case 1:    return RedirectToAction("Index", "DashboardAdmin");
                    case 2:    return RedirectToAction("Index", "DashboardDocente");
                    case 1002: return RedirectToAction("Index", "DashboardDirector");
                    case 1003: return RedirectToAction("Index", "DashboardSecretaria");
                    case 1004: return RedirectToAction("Index", "DashboardRRHH");
                    case 1005: return RedirectToAction("Index", "DashboardCajero");
                    case 1006: return RedirectToAction("Index", "DashboardFinanciero");
                    case 1007: return RedirectToAction("Index", "DashboardConsulta");
                }
            }

            return View();
        }

        private static string ResolveDashboardControllerByRoleName(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return null;
            var r = roleName.Trim().ToLowerInvariant();

            if (r == "admin")                                  return "DashboardAdmin";
            if (r == "docente")                                return "DashboardDocente";
            if (r == "director")                               return "DashboardDirector";
            if (r == "secretaria" || r == "secretaría")        return "DashboardSecretaria";
            if (r == "recursos humanos" || r == "rrhh")        return "DashboardRRHH";
            if (r == "cajero" || r == "caja")                  return "DashboardCajero";
            if (r == "contador" || r == "financiero")          return "DashboardFinanciero";
            if (r == "consulta" || r == "consultas")           return "DashboardConsulta";
            return null;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
