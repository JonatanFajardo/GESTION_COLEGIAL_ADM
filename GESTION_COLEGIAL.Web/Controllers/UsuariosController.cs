using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Listado de usuarios")]
    public class UsuariosController : BaseController
    {
        private readonly UsuariosService usuariosService = new UsuariosService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAsync()
        {
            return View("CreateAsync");
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await usuariosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await usuariosService.Find(id);
            return AjaxResult(result, true);
        }

        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await usuariosService.Detail(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(UsuarioViewModel model)
        {
            // Misma regla que la API y los procedimientos (ver Business/Models/UsuarioViewModel.cs).
            model.Usu_Name = (model.Usu_Name ?? string.Empty).Trim();
            if (!UsuarioViewModel.NombreValido(model.Usu_Name))
            {
                return AjaxResult(false, false, AlertMessage.AlertMessageType.Error, UsuarioViewModel.MensajeNombre);
            }
            if (model.Usu_Id == 0 && string.IsNullOrEmpty(model.Usu_Contrasena))
            {
                return AjaxResult(false, false, AlertMessage.AlertMessageType.Error, "Escriba la contraseña inicial del usuario (al menos 8 caracteres).");
            }
            if (!string.IsNullOrEmpty(model.Usu_Contrasena)
                && (model.Usu_Contrasena.Length < 8 || string.Equals(model.Usu_Contrasena, model.Usu_Name, System.StringComparison.OrdinalIgnoreCase)))
            {
                return AjaxResult(false, false, AlertMessage.AlertMessageType.Error, "La contraseña debe tener al menos 8 caracteres y ser distinta del usuario.");
            }

            if (model.Usu_Id == 0)
            {
                bool result = await usuariosService.Create(model);

                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await usuariosService.Edit(model);

                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                // Cambió la contraseña, el rol o el estado: las sesiones abiertas de esa cuenta se revisan ya.
                if (model.Usu_Id == GetUsuarioId())
                {
                    await ValidadorSesion.RenovarAsync(HttpContext);
                }
                else
                {
                    ValidadorSesion.Olvidar(model.Usu_Id);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Usu_Id, string Usu_Name)
        {
            ValidationModal validationModal = new ValidationModal();
            if (!UsuarioViewModel.NombreValido((Usu_Name ?? string.Empty).Trim()))
            {
                return Json(UsuarioViewModel.MensajeNombre);
            }

            var result = await usuariosService.Exist(Usu_Name);

            if (result != null)
            {
                int? firstValue = result.Usu_Id;
                return (firstValue == Usu_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(UsuarioViewModel model)
        {
            bool result = await usuariosService.Delete(model.Usu_Id);

            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            ValidadorSesion.Olvidar(model.Usu_Id);
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        public async Task<ActionResult> RolesDropdownAsync()
        {
            var result = await usuariosService.RolesDropdownAsync();
            return AjaxResult(result);
        }
    }
}
