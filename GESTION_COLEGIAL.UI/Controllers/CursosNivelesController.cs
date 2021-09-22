using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CursosNivelesController : BaseController
    {
        // GET: CursosNiveles
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "CursosNiveles/List";
            var result = await CatalogsService.List<CursoNivelViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CursoNivelViewModel model)
        {
            if (model.Cun_Id == 0)
            {
                string url = "CursosNiveles/Create";
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
                string url = "CursosNiveles/Edit";
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
            string url = "CursosNiveles/Find";
            var result = await CatalogsService.Find<CursoNivelViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Cun_Id, string Cun_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Cun_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "CursosNiveles/Exist";
            var result = await CatalogsService.Exist<CursoNivelViewModel>(url, Cun_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Cun_Id;
                return (firstValue == Cun_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/Remove";
            bool result = await CatalogsService.Delete(url, model.Cun_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}