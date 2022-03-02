using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class MateriasController : BaseController
    {
        MateriasService materiasService = new MateriasService();
        // GET: Materias
        public ActionResult Index()
        {
            var model = new MateriaViewModel();
            return View();
        }
        public ActionResult Create()
        {
            return View("Create");
        }

        public async Task<ActionResult> List()
        {
            var result = await materiasService.List();
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(MateriaViewModel model)
        {
            if (model.Mat_Id == 0)
            {
                bool result = await materiasService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                
                //ModelState.Remove("Mat_EsActivo");
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await materiasService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                //ModelState.Remove("Mat_EsActivo");
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }

        }

        public async Task<ActionResult> Find(int id)
        {
            var result = await materiasService.Find(id);
            //ViewBag.Mat_EsActivo = result.Mat_EsActivo;
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Mat_Id, string Mat_Nombre)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Mat_Nombre;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await materiasService.Exist(Mat_Nombre);
            if (result != null)
            {
                int? firstValue = result.Mat_Id;
                return (firstValue == Mat_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(MateriaViewModel model)
        {
            bool result = await materiasService.Delete(model.Mat_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}