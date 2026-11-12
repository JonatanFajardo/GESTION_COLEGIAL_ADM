using Microsoft.AspNetCore.Mvc.Filters;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Exige login y, si se indica, permiso a una pantalla concreta.
    /// Uso: [SessionManager("Listado de alumnos")] en el controller o la acción.
    /// El admin entra a todo; "Home" solo exige login.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SessionManager : ActionFilterAttribute
    {
        private readonly string _pantallaNombre;

        public SessionManager() { }

        public SessionManager(string pantallaNombre)
        {
            _pantallaNombre = pantallaNombre;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = new SesionUsuario(filterContext.HttpContext);
            string pantallas = sesion[SesionUsuario.Pantallas] as string;

            if (string.IsNullOrEmpty(pantallas))
            {
                // Sin login o cookie vencida → Login.
                // (En MVC 5 aquí se restauraba la sesión desde la cookie; ahora los datos ya viven en la cookie.)
                filterContext.Result = new RedirectToActionResult("Login", "Account", null);
            }
            else if (!string.IsNullOrEmpty(_pantallaNombre)
                     && _pantallaNombre != "Home"
                     && !EsAdmin(sesion)
                     && !TienePantalla(pantallas, _pantallaNombre))
            {
                // Usuario sin permiso para esta pantalla.
                filterContext.Result = new RedirectToActionResult("SinAcceso", "Account", null);
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Verifica si una pantalla está en la lista de pantallas (separadas por coma).
        /// </summary>
        public static bool TienePantalla(string pantallas, string pantallaNombre)
        {
            if (string.IsNullOrEmpty(pantallas) || string.IsNullOrEmpty(pantallaNombre))
                return false;

            var lista = pantallas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return lista.Any(p => p.Trim().Equals(pantallaNombre, StringComparison.OrdinalIgnoreCase));
        }

        private static bool EsAdmin(SesionUsuario sesion)
        {
            var role = sesion[SesionUsuario.RoleName] as string ?? "";
            return role.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
