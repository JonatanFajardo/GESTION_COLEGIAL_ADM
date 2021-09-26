using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EstadosController : BaseController
    {
        // GET: Estados
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// 
        /// 
        /// En proceso de revision.
        /// 
        /// 
        /// 
        /// 
        /// </summary>


        public async Task<ActionResult> List()
        {
            string url = "Estados/List";
            var result = await CatalogsService.List<EstadoViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Estados/Find";
            var result = await CatalogsService.Find<EstadoViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Create(EstadoViewModel model)
        {
            if (model.Est_Id == 0)
            {
                string url = "Estados/Create";
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
                string url = "Estados/Edit";
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
        public async Task<ActionResult> Exist(int? Est_Id, string Est_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Est_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Estados/Exist";
            var result = await CatalogsService.Exist<EstadoViewModel>(url, Est_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Est_Id;
                return (firstValue == Est_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(EstadoViewModel model)
        {
            string url = "Estados/Remove";
            bool result = await CatalogsService.Delete(url, model.Est_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}