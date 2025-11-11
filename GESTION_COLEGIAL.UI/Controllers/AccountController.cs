using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
                    Session["RoleName"] = result.User.Rol_Nombre;
                    Session["Token"] = result.Token;

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

        public async Task<ActionResult> Logout()
        {
            if (Session["UserId"] != null)
            {
                int usuId = (int)Session["UserId"];
                await _accountService.LogoutAsync(usuId);
            }

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }
    }
}
