using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class HorasController : BaseController
    {
        private readonly HorasService horasService = new HorasService();
        // GET: Horas
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await horasService.ListAsync();
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(HoraViewModel model)
        {
            if (model.Hor_Id == 0)
            {
                bool result = await horasService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await horasService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }

        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await horasService.Find(id);
            return AjaxResult(result, true);
        }

        //[HttpPost]
        //public async Task<ActionResult> ExistAsync(int? Hor_Id, string Hor_Descripcion)
        //{
        //    //Validaciones.
        //    ValidationModal validationModal = new ValidationModal();
        //    validationModal.SendMessage = Hor_Descripcion;
        //    validationModal.BlankSpaces();
        //    validationModal.SpecialCharacters();
        //    if (validationModal.RequestMessage != null)
        //    {
        //        return Json(validationModal.RequestMessage);
        //    }

        //    //Envío de datos.
        //    string url = "Horas/ExistAsync";
        //    var result = await horasService.ExistAsync(url, Hor_Descripcion);
        //    if (result != null)
        //    {
        //        int? firstValue = result.First().Hor_Id;
        //        return (firstValue == Hor_Id) ? Json(true) : Json(msjExist);
        //    }
        //    return Json(true);
        //}

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(HoraViewModel model)
        {
            bool result = await horasService.Delete(model.Hor_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}