using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class NivelesEducativosController : BaseController
    {
        // GET: NivelesEducativos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "NivelesEducativos/List";
            var result = await CatalogsService.List<NivelEducativoViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(NivelEducativoViewModel model)
        {
            if (model.Niv_Id == 0)
            {
                string url = "NivelesEducativos/Create";
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
                string url = "NivelesEducativos/Edit";
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
            string url = "NivelesEducativos/Find";
            var result = await CatalogsService.Find<NivelEducativoViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Niv_Id, string Niv_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Niv_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "NivelesEducativos/Exist";
            var result = await CatalogsService.Exist<NivelEducativoViewModel>(url, Niv_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Niv_Id;
                return (firstValue == Niv_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/Remove";
            bool result = await CatalogsService.Delete(url, model.Niv_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}