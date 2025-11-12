using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de formas de pago.
    /// </summary>
    public class FormasPagoController : BaseController
    {
        private readonly FormasPagoService formasPagoService = new FormasPagoService();

        /// <summary>
        /// Acción para mostrar la vista principal de formas de pago.
        /// </summary>
        /// <returns>Vista principal de formas de pago.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de formas de pago.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de formas de pago.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await formasPagoService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una forma de pago específica.
        /// </summary>
        /// <param name="id">ID de la forma de pago.</param>
        /// <returns>Resultado con los detalles de la forma de pago.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await formasPagoService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar una forma de pago.
        /// </summary>
        /// <param name="model">Modelo de vista de la forma de pago.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(FormaPagoFindViewModel model)
        {
            if (model.FormaPagoId == 0)
            {
                bool result = await formasPagoService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await formasPagoService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe una forma de pago con la descripción proporcionada.
        /// </summary>
        /// <param name="FormaPagoId">ID de la forma de pago.</param>
        /// <param name="Descripcion">Descripción de la forma de pago.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? FormaPagoId, string Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await formasPagoService.Exist(Descripcion);
            if (result != null)
            {
                int? firstValue = result.FormaPagoId;
                return (firstValue == FormaPagoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una forma de pago.
        /// </summary>
        /// <param name="model">Modelo de vista de la forma de pago.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(FormaPagoFindViewModel model)
        {
            bool result = await formasPagoService.Delete(model.FormaPagoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
