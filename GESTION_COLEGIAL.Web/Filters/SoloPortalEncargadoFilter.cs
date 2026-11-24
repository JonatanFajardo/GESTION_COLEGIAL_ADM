using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Filtro global: un usuario con rol Encargado (portal de padres) solo puede abrir el portal,
    /// la activación de su cuenta y cerrar sesión. Cualquier otra pantalla o llamada administrativa
    /// lo regresa al portal (403 si es una llamada AJAX), aunque esa pantalla solo pida login.
    /// </summary>
    public sealed class SoloPortalEncargadoFilter : IActionFilter
    {
        public const string RolEncargado = "Encargado";

        private static readonly HashSet<string> Permitidos =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Portal", "Account", "Invitacion" };

        /// <summary>True si la sesión es de un encargado (rol "Encargado").</summary>
        public static bool EsEncargado(HttpContext contexto)
        {
            if (contexto?.User?.Identity?.IsAuthenticated != true) return false;
            var rol = new SesionUsuario(contexto)[SesionUsuario.RoleName] as string ?? "";
            return rol.Trim().Equals(RolEncargado, StringComparison.OrdinalIgnoreCase);
        }

        public void OnActionExecuting(ActionExecutingContext contexto)
        {
            if (!EsEncargado(contexto.HttpContext)) return;

            var controller = (contexto.ActionDescriptor as ControllerActionDescriptor)?.ControllerName;
            if (controller != null && Permitidos.Contains(controller)) return;

            var peticion = contexto.HttpContext.Request;
            bool esAjax = string.Equals(peticion.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                          || (peticion.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase));
            contexto.Result = esAjax
                ? new ObjectResult(new { success = false, message = "No tiene permiso para esta acción." }) { StatusCode = StatusCodes.Status403Forbidden }
                : new RedirectToActionResult("Index", "Portal", null);
        }

        public void OnActionExecuted(ActionExecutedContext contexto) { }
    }
}
