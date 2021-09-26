using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class TitulosController : BaseController
    {
        // GET: Titulos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Titulos/List";
            var result = await CatalogsService.List<TituloViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Titulos/Find";
            var result = await CatalogsService.Find<TituloViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Create(TituloViewModel model)
        {
            if (model.Tit_Id == 0)
            {
                string url = "Titulos/Create";
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
                string url = "Titulos/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }        

        [HttpPost]
        public async Task<ActionResult> Exist(int? Tit_Id, string Tit_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Tit_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Titulos/Exist";
            var result = await CatalogsService.Exist<TituloViewModel>(url, Tit_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Tit_Id;
                return (firstValue == Tit_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(TituloViewModel model)
        {
            string url = "Titulos/Remove";
            bool result = await CatalogsService.Delete(url, model.Tit_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}