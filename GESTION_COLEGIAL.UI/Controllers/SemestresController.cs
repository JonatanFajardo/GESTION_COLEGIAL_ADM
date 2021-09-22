using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class SemestresController : BaseController
    {
        // GET: Semestres
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Semestres/List";
            var result = await CatalogsService.List<SemestreViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SemestreViewModel model)
        {
            if (model.Sem_Id == 0)
            {
                string url = "Semestres/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                string url = "Semestres/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }

        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Semestres/Find";
            var result = await CatalogsService.Find<SemestreViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Sem_Id, string Sem_Descripcion)
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
            string url = "Semestres/Exist";
            var result = await CatalogsService.Exist<SemestreViewModel>(url, Sem_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Sem_Id;
                return (firstValue == Sem_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(SemestreViewModel model)
        {
            string url = "Semestres/Remove";
            bool result = await CatalogsService.Delete(url, model.Sem_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}