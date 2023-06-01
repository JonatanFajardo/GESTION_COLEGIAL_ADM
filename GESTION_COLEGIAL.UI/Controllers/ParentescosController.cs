using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ParentescosController : BaseController
    {
        private readonly ParentescosService parentescosService = new ParentescosService();

        // GET: Parentescos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await parentescosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await parentescosService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(ParentescoViewModel model)
        {
            if (model.Par_Id == 0)
            {
                bool result = await parentescosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await parentescosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Par_Id, string Par_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Par_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await parentescosService.Exist(Par_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Par_Id;
                return (firstValue == Par_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ParentescoViewModel model)
        {
            bool result = await parentescosService.Delete(model.Par_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}