using System.Security.Claims;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using GESTION_COLEGIAL.UI.Filters;

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
                var result = await _accountService.LoginAsync(model, IpCliente(HttpContext));

                if (result != null && result.Success)
                {
                    await IniciarSesionAsync(HttpContext, result, model.RememberMe);

                    // El encargado (portal de padres) siempre entra a su portal.
                    if (SoloPortalEncargadoFilter.RolEncargado.Equals((result.User.Rol_Nombre ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Portal");
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                // Mensaje genérico de la API: no dice si el usuario existe.
                ModelState.AddModelError("", string.IsNullOrWhiteSpace(result?.Message) ? "Usuario o contraseña incorrectos." : result.Message);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "No se pudo conectar con el servidor. Intente de nuevo en un momento.");
                return View(model);
            }
        }

        /// <summary>
        /// Crea la cookie de login con los datos del usuario (también la usa la activación del portal de padres).
        /// </summary>
        internal static async Task IniciarSesionAsync(HttpContext contexto, LoginResponseViewModel result, bool recordar)
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
                new Claim(SesionUsuario.Pantallas, pantallas),
                new Claim(ValidadorSesion.ClaimSello, result.User.Sello ?? "")
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var propiedades = new AuthenticationProperties
            {
                // "Recordarme": 7 días aunque se cierre el navegador; si no, 60 minutos de inactividad.
                IsPersistent = recordar,
                ExpiresUtc = recordar ? DateTimeOffset.UtcNow.AddDays(7) : (DateTimeOffset?)null
            };

            // La API acaba de comprobar la cuenta: ese es el estado vigente (evita usar uno guardado de antes).
            ValidadorSesion.Recordar(result.User.Usu_Id, new EstadoSesionViewModel { Habilitado = 1, Sello = result.User.Sello });

            // Descarta datos de una sesión anterior (ej. "ver como rol").
            contexto.Session.Clear();
            await contexto.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad), propiedades);
        }

        /// <summary>IP del navegador (la que ve el ADM), para el límite de intentos de la API.</summary>
        internal static string IpCliente(HttpContext contexto)
        {
            var ip = contexto.Connection.RemoteIpAddress;
            if (ip == null) return null;
            return (ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip).ToString();
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
