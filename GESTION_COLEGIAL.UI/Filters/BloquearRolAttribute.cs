using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Filtro de autorización que BLOQUEA el acceso a un controller/acción a los roles
    /// cuyo nombre contenga alguno de los términos indicados (comparación case-insensitive,
    /// por coincidencia parcial). Redirige al rol bloqueado a una pantalla segura con un
    /// aviso en español (TempData["Aviso"]).
    ///
    /// Uso: [BloquearRol("docente")] sobre el controller o la acción.
    /// Pensado para impedir que un docente arme/edite horarios (tarea administrativa),
    /// dejándole solo la consulta en el visor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class BloquearRolAttribute : ActionFilterAttribute
    {
        private readonly string[] _rolesBloqueados;
        private readonly string _mensaje;
        private readonly string _controllerDestino;
        private readonly string _actionDestino;

        public BloquearRolAttribute(params string[] rolesBloqueados)
        {
            _rolesBloqueados = rolesBloqueados ?? new string[0];
            _mensaje = "No tienes permiso para armar horarios. Consulta tu horario en el visor.";
            _controllerDestino = "Home";
            _actionDestino = "Index";
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            string roleName = session?["RoleName"] as string ?? "";

            bool bloqueado = _rolesBloqueados.Any(r =>
                !string.IsNullOrEmpty(r) &&
                roleName.IndexOf(r, StringComparison.OrdinalIgnoreCase) >= 0);

            if (bloqueado)
            {
                filterContext.Controller.TempData["Aviso"] = _mensaje;
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = _controllerDestino, action = _actionDestino }));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
