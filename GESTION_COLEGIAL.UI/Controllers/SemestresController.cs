using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class SemestresController : BaseController
    {
        private readonly SemestresService semestresService = new SemestresService();

        // GET: Semestres
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await semestresService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await semestresService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(SemestreViewModel model)
        {
            if (model.Sem_Id == 0)
            {
                bool result = await semestresService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await semestresService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Sem_Id, string Sem_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Sem_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await semestresService.Exist(Sem_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Sem_Id;
                return (firstValue == Sem_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(SemestreViewModel model)
        {
            bool result = await semestresService.Delete(model.Sem_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}