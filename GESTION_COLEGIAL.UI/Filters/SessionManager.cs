using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Filtro de autorización que verifica si el usuario tiene acceso a una pantalla específica.
    /// Valida la sesión del usuario y sus permisos basados en el rol asignado.
    /// </summary>
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
            var session = filterContext.HttpContext.Session;
            string pantallas = session["pantallas"] as string;

            if (string.IsNullOrEmpty(pantallas))
            {
                // Sesión expirada o no autenticado → redirigir al Login
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
            else if (!string.IsNullOrEmpty(_pantallaNombre)
                     && _pantallaNombre != "Home"
                     && !pantallas.Contains(_pantallaNombre))
            {
                // Usuario no tiene permiso para esta pantalla
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "SinAcceso" }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
