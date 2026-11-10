using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        public ActionResult Login()
        {
            // Si ya está autenticado, redirigir al Home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
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
                    // Guardar datos del usuario en sesión
                    Session["UserId"] = result.User.Usu_Id;
                    Session["Username"] = result.User.Usu_Name;
                    Session["EmpId"] = result.User.Emp_Id;
                    Session["RolId"] = result.User.Rol_Id;
                    Session["RoleName"] = result.User.Rol_Nombre;
                    Session["Token"] = result.Token;

                    // Guardar pantallas/permisos del rol en sesión
                    string pantallas = result.Pantallas != null
                        ? String.Join(",", result.Pantallas)
                        : "";
                    Session["pantallas"] = pantallas;

                    // Crear ticket de autenticación con datos del usuario embebidos
                    string userData = JsonConvert.SerializeObject(new
                    {
                        UserId = result.User.Usu_Id,
                        EmpId = result.User.Emp_Id,
                        RolId = result.User.Rol_Id,
                        RoleName = result.User.Rol_Nombre,
                        Username = result.User.Usu_Name,
                        Token = result.Token,
                        Pantallas = pantallas
                    });

                    DateTime expiracion = model.RememberMe
                        ? DateTime.Now.AddDays(7)
                        : DateTime.Now.AddMinutes(60);

                    var ticket = new FormsAuthenticationTicket(
                        1,
                        result.User.Usu_Name,
                        DateTime.Now,
                        expiracion,
                        model.RememberMe,
                        userData
                    );

                    string ticketEncriptado = FormsAuthentication.Encrypt(ticket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncriptado);
                    if (model.RememberMe)
                    {
                        authCookie.Expires = expiracion;
                    }
                    Response.Cookies.Add(authCookie);

                    // Redirigir
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", result?.Message ?? "Usuario o contrasena incorrectos");
                    return View(model);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con el servidor: " + ex.Message);
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult SinAcceso()
        {
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (Session["UserId"] != null)
            {
                int usuId = (int)Session["UserId"];
                await _accountService.LogoutAsync(usuId);
            }

            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }
    }
}
