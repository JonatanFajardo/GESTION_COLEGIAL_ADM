using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ParcialesController : BaseController
    {
        // GET: Parciales
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Parciales/List";
            var result = await CatalogsService.List<ParcialViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ParcialViewModel model)
        {
            if (model.Pac_Id == 0)
            {
                string url = "Parciales/Create";
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
                string url = "Parciales/Edit";
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
            string url = "Parciales/Find";
            var result = await CatalogsService.Find<ParcialViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Pac_Id, string Pac_Descripcion)
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
            string url = "Parciales/Exist";
            var result = await CatalogsService.Exist<ParcialViewModel>(url, Pac_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Pac_Id;
                return (firstValue == Pac_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(ParcialViewModel model)
        {
            string url = "Parciales/Remove";
            bool result = await CatalogsService.Delete(url, model.Pac_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}