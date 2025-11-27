using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de conceptos de pago.
    /// </summary>
    public class ConceptosPagoController : BaseController
    {
        private readonly ConceptosPagoService conceptosPagoService = new ConceptosPagoService();

        /// <summary>
        /// Acción para mostrar la vista principal de conceptos de pago.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de conceptos de pago.
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await conceptosPagoService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de conceptos de pago para dropdown.
        /// </summary>
        public async Task<ActionResult> DropdownAsync()
        {
            var result = await conceptosPagoService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un concepto de pago específico.
        /// </summary>
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await conceptosPagoService.Find(id);
			return AjaxResult(result, true);
		}

		/// <summary>
		/// Acción asincrónica para obtener el detalle de un concepto de pago específico.
		/// </summary>
		public async Task<ActionResult> DetailAsync(int id)
		{
			var result = await conceptosPagoService.Detail(id);
			return AjaxResult(result, true);
		}

        /// <summary>
        /// Acción asincrónica para crear o editar un concepto de pago.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(ConceptoPagoFindViewModel model)
        {
            if (model.ConceptoPagoId == 0)
            {
                bool result = await conceptosPagoService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await conceptosPagoService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe un concepto de pago con la descripción proporcionada.
        /// </summary>
        /// <param name="ConceptoPagoId">ID del concepto de pago.</param>
        /// <param name="Descripcion">Descripción del concepto de pago.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? ConceptoPagoId, string Descripcion)
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
            var result = await conceptosPagoService.Exist(Descripcion);
            if (result != null)
            {
                int? firstValue = result.ConceptoPagoId;
                return (firstValue == ConceptoPagoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un concepto de pago.
        /// </summary>
        /// <param name="model">Modelo de vista del concepto de pago.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ConceptoPagoFindViewModel model)
        {
            bool result = await conceptosPagoService.Delete(model.ConceptoPagoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
