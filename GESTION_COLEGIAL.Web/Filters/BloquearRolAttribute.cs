using Microsoft.AspNetCore.Mvc.Filters;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Bloquea una acción para ciertos roles (por nombre, coincidencia parcial)
    /// y los regresa al inicio con un aviso.
    /// Uso: [BloquearRol("docente")]
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
            var sesion = new SesionUsuario(filterContext.HttpContext);
            string roleName = sesion[SesionUsuario.RoleName] as string ?? "";

            bool bloqueado = _rolesBloqueados.Any(r =>
                !string.IsNullOrEmpty(r) &&
                roleName.IndexOf(r, StringComparison.OrdinalIgnoreCase) >= 0);

            if (bloqueado)
            {
                if (filterContext.Controller is Controller controller)
                {
                    controller.TempData["Aviso"] = _mensaje;
                }

                filterContext.Result = new RedirectToActionResult(_actionDestino, _controllerDestino, null);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
