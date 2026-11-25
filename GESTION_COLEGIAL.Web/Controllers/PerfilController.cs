using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Mi Perfil")]
    public class PerfilController : BaseController
    {
        private readonly PerfilService _service = new PerfilService();
        private readonly AccountService _cuenta = new AccountService();

        public async Task<ActionResult> Index()
        {
            int usuId = GetUsuarioId();
            var perfil = await _service.FindAsync(usuId);
            return View(perfil);
        }

        /// <summary>
        /// Cambia la contraseña del usuario en sesión. La API vuelve a validar todo (contraseña actual,
        /// reglas, bloqueo por intentos); aquí solo se revisa lo básico para responder rápido.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarContrasena(string contrasenaActual, string contrasenaNueva, string confirmarContrasena)
        {
            if (string.IsNullOrEmpty(contrasenaActual)) return Rechazo("Escriba su contraseña actual.");
            if (string.IsNullOrEmpty(contrasenaNueva) || contrasenaNueva.Length < 8) return Rechazo("La contraseña nueva debe tener al menos 8 caracteres.");
            if (contrasenaNueva != confirmarContrasena) return Rechazo("La confirmación no coincide con la contraseña nueva.");

            var r = await _cuenta.CambiarContrasenaAsync(GetUsuarioId(), contrasenaActual, contrasenaNueva, AccountController.IpCliente(HttpContext));
            if (r == null || r.Error)
            {
                return Rechazo(string.IsNullOrWhiteSpace(r?.Mensaje) ? "No se pudo cambiar la contraseña." : r.Mensaje);
            }
            // Esta sesión sigue abierta; las demás sesiones de la cuenta se cierran.
            await ValidadorSesion.RenovarAsync(HttpContext);
            return Json(new { success = true, message = "Contraseña actualizada. Úsela la próxima vez que inicie sesión." });
        }

        private ActionResult Rechazo(string mensaje)
        {
            Response.StatusCode = 400;
            return Json(new { success = false, message = mensaje });
        }
    }
}
