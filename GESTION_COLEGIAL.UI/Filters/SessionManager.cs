using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Filtro de autorización que verifica si el usuario tiene acceso a una pantalla específica.
    /// Valida la sesión del usuario y sus permisos basados en el rol asignado.
    /// Se puede aplicar a nivel de clase (controller) y a nivel de acción (método).
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
                     && !TienePantalla(pantallas, _pantallaNombre))
            {
                // Usuario no tiene permiso para esta pantalla
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "SinAcceso" }));
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Verifica si el nombre de pantalla existe como elemento exacto en la lista separada por comas.
        /// Evita coincidencias parciales (ej: "Listado de cursos" no coincide con "Listado de cursos niveles").
        /// </summary>
        public static bool TienePantalla(string pantallas, string pantallaNombre)
        {
            if (string.IsNullOrEmpty(pantallas) || string.IsNullOrEmpty(pantallaNombre))
                return false;

            var lista = pantallas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return lista.Any(p => p.Trim().Equals(pantallaNombre, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtiene la lista de pantallas del usuario desde la sesión actual.
        /// </summary>
        public static string ObtenerPantallas()
        {
            return HttpContext.Current?.Session?["pantallas"] as string ?? "";
        }
    }
}
