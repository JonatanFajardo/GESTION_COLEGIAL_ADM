using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EstadosController : BaseController
    {
        EstadosService estadosService = new EstadosService();


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
            var result = await estadosService.List();
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            var result = await estadosService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Create(EstadoViewModel model)
        {
            if (model.Est_Id == 0)
            {
                bool result = await estadosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await estadosService.Edit(model);

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
            var result = await estadosService.Exist(Est_Descripcion);
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
            bool result = await estadosService.Delete(model.Est_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}