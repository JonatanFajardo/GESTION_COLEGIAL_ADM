using System.Security.Claims;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController()
        {
            _accountService = new AccountService();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            // Si ya está logueado (y su rol tiene pantallas), ir al inicio.
            // Sin pantallas se queda aquí para no entrar en un ciclo Login ↔ Home.
            var pantallas = new SesionUsuario(HttpContext)[SesionUsuario.Pantallas] as string;
            if (User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(pantallas))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _accountService.LoginAsync(model);

                if (result != null && result.Success)
                {
                    string pantallas = result.Pantallas != null
                        ? String.Join(",", result.Pantallas)
                        : "";

                    // Los datos del usuario viajan cifrados en la cookie de login
                    // (antes: Session + ticket de FormsAuthentication).
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.User.Usu_Name ?? ""),
                        new Claim(ClaimTypes.Role, result.User.Rol_Nombre ?? ""),
                        new Claim(SesionUsuario.UserId, result.User.Usu_Id.ToString()),
                        new Claim(SesionUsuario.EmpId, result.User.Emp_Id.ToString()),
                        new Claim(SesionUsuario.RolId, result.User.Rol_Id.ToString()),
                        new Claim(SesionUsuario.RoleName, result.User.Rol_Nombre ?? ""),
                        new Claim(SesionUsuario.Username, result.User.Usu_Name ?? ""),
                        new Claim(SesionUsuario.Token, result.Token ?? ""),
                        new Claim(SesionUsuario.Pantallas, pantallas)
                    };

                    var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var propiedades = new AuthenticationProperties
                    {
                        // "Recordarme": 7 días aunque se cierre el navegador; si no, 60 minutos de inactividad.
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : (DateTimeOffset?)null
                    };

                    // Descarta datos de una sesión anterior (ej. "ver como rol").
                    HttpContext.Session.Clear();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad), propiedades);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", result?.Message ?? "Usuario o contrasena incorrectos");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con el servidor: " + ex.Message);
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult SinAcceso()
        {
            return View();
        }

        /// <summary>
        /// Página de error genérica (antes: HandleErrorAttribute global de MVC 5).
        /// Vive aquí porque este controller no exige login.
        /// </summary>
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View("Error");
        }

        public async Task<IActionResult> Logout()
        {
            if (new SesionUsuario(HttpContext)[SesionUsuario.UserId] is int usuId)
            {
                await _accountService.LogoutAsync(usuId);
            }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }
    }
}
