using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CargosController : BaseController
    {
        private readonly CargosService cargosService = new CargosService();
        // GET: Cargos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View("CreateAsync");
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await cargosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await cargosService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CargoViewModel model)
        {
            if (model.Car_Id == 0)
            {
                bool result = await cargosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await cargosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }

        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Car_Id, string Car_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Car_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await cargosService.Exist(Car_Descripcion);

            //Validamos error
            if (result != null)
            {
                int? firstValue = result.Car_Id;
                return (firstValue == Car_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(CargoViewModel model)
        {
            bool result = await cargosService.Delete(model.Car_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}