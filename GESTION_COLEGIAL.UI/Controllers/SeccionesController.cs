using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class SeccionesController : BaseController
    {
        // GET: Secciones
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Secciones/List";
            var result = await CatalogsService.List<SeccionViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SeccionViewModel model)
        {
            if (model.Sec_Id == 0)
            {
                string url = "Secciones/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false);
                }

                return AjaxResult(true);
            }
            else
            {
                string url = "Secciones/Edit";
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
            string url = "Secciones/Find";
            var result = await CatalogsService.Find<SeccionViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Sec_Id, string Sec_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Sec_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Secciones/Exist";
            var result = await CatalogsService.Exist<SeccionViewModel>(url, Sec_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Sec_Id;
                return (firstValue == Sec_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            string url = "Secciones/Remove";
            bool result = await CatalogsService.Delete(url, id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false);
            }
            return AjaxResult(true);
        }
    }
}