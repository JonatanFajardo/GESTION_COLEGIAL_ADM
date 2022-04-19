using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ParcialesController : BaseController
    {
        ParcialesService parcialesService = new ParcialesService();
        // GET: Parciales
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await parcialesService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await parcialesService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(ParcialViewModel model)
        {
            if (model.Pac_Id == 0)
            {
                bool result = await parcialesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await parcialesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Pac_Id, string Pac_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Pac_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await parcialesService.Exist(Pac_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Pac_Id;
                return (firstValue == Pac_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ParcialViewModel model)
        {
            bool result = await parcialesService.Delete(model.Pac_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}