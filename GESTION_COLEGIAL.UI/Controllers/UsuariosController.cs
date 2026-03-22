using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

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

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Usu_Id, string Usu_Name)
        {
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Usu_Name;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
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
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        public async Task<ActionResult> RolesDropdownAsync()
        {
            var result = await usuariosService.RolesDropdownAsync();
            return AjaxResult(result);
        }
    }
}
