using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Models;
using Microsoft.AspNetCore.Authorization;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Página pública del enlace de invitación al portal de padres (espejo de Skuul "invitations/{token}"):
    /// el encargado elige usuario y contraseña, la cuenta queda con rol Encargado y entra a su portal.
    /// El enlace sirve una sola vez y vence; la API guarda solo la huella del código.
    /// </summary>
    [AllowAnonymous]
    public class InvitacionController : Controller
    {
        private readonly PortalPadresService _service = new PortalPadresService();
        private readonly AccountService _account = new AccountService();

        [HttpGet("invitacion/{codigo}")]
        public async Task<IActionResult> Index(string codigo)
        {
            var info = await _service.InvitacionInfoAsync(codigo);
            return View(new InvitacionActivarViewModel { Codigo = codigo, Info = info, Usu_Name = info.UsuarioExistente });
        }

        [HttpPost("invitacion/{codigo}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string codigo, string usu_Name, string contrasena, string confirmar)
        {
            var modelo = new InvitacionActivarViewModel { Codigo = codigo, Usu_Name = usu_Name?.Trim() };
            modelo.Info = await _service.InvitacionInfoAsync(codigo);
            if (modelo.Info.Estado != "pendiente") return View(modelo);

            if (string.IsNullOrEmpty(contrasena) || contrasena != confirmar)
            {
                modelo.Error = "Las dos contraseñas no son iguales.";
                return View(modelo);
            }

            var r = await _service.ActivarAsync(new PortalActivarViewModel { Token = codigo, Usu_Name = modelo.Usu_Name, Contrasena = contrasena });
            if (!r.Ok)
            {
                modelo.Error = r.Mensaje ?? "No se pudo activar la cuenta.";
                return View(modelo);
            }

            // Igual que Skuul: con la cuenta lista, la persona entra de una vez.
            var login = await _account.LoginAsync(new LoginViewModel { Username = r.Estado, Password = contrasena }, AccountController.IpCliente(HttpContext));
            if (login != null && login.Success)
            {
                await AccountController.IniciarSesionAsync(HttpContext, login, false);
                TempData["PortalAviso"] = "Su cuenta está lista. Bienvenido al portal de padres.";
                return RedirectToAction("Index", "Portal");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
