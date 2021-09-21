using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ModalidadesController : BaseController
    {
        // GET: Modalidades
        public ActionResult Index()
        {
           

            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Modalidades/List";
            var result = await CatalogsService.List<ModalidadViewModel>(url);
            Show(AlertMessageType.Error, "error");
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ModalidadViewModel model)
        {
            if (model.Mda_Id == 0)
            {
                string url = "Modalidades/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    //ShowController(AlertMessageCustomType.Error);
                    //Show(AlertMessageType.Error, "error");
                    return AjaxResult(false);
                }
                //ShowController(AlertMessageCustomType.SuccessInsert);
                //Show(AlertMessageType.Success, "success");
                return AjaxResult(true);
            }
            else
            {
                string url = "Modalidades/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false);
                }

                return AjaxResult(true);
            }

        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Modalidades/Find";
            var result = await CatalogsService.Find<ModalidadViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Mda_Id, string Mda_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Mda_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Modalidades/Exist";
            var result = await CatalogsService.Exist<ModalidadViewModel>(url, Mda_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Mda_Id;
                return (firstValue == Mda_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(ModalidadViewModel model)
        {
            string url = "Modalidades/Remove";
            bool result = await CatalogsService.Delete(url, model.Mda_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false);
            }
            return AjaxResult(true);
        }
    }
}
