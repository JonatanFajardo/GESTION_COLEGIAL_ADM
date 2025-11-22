using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de estados de pago.
    /// </summary>
    public class EstadosPagoController : BaseController
    {
        private readonly EstadosPagoService estadosPagoService = new EstadosPagoService();

        /// <summary>
        /// Acción para mostrar la vista principal de estados de pago.
        /// </summary>
        /// <returns>Vista principal de estados de pago.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de estados de pago.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de estados de pago.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await estadosPagoService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un estado de pago específico.
        /// </summary>
        /// <param name="id">ID del estado de pago.</param>
        /// <returns>Resultado con los detalles del estado de pago.</returns>
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await estadosPagoService.Find(id);
			return AjaxResult(result, true);
		}

		/// <summary>
		/// Acción asincrónica para obtener el detalle de un estado de pago específico.
		/// </summary>
		/// <param name="id">ID del estado de pago.</param>
		/// <returns>Resultado con la información detallada.</returns>
		public async Task<ActionResult> DetailAsync(int id)
		{
			var result = await estadosPagoService.Detail(id);
			return AjaxResult(result, true);
		}

        /// <summary>
        /// Acción asincrónica para crear o editar un estado de pago.
        /// </summary>
        /// <param name="model">Modelo de vista del estado de pago.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(EstadoPagoFindViewModel model)
        {
            if (model.EstadoPagoId == 0)
            {
                bool result = await estadosPagoService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await estadosPagoService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe un estado de pago con la descripción proporcionada.
        /// </summary>
        /// <param name="EstadoPagoId">ID del estado de pago.</param>
        /// <param name="Descripcion">Descripción del estado de pago.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? EstadoPagoId, string Descripcion)
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
            var result = await estadosPagoService.Exist(Descripcion);
            if (result != null)
            {
                int? firstValue = result.EstadoPagoId;
                return (firstValue == EstadoPagoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un estado de pago.
        /// </summary>
        /// <param name="model">Modelo de vista del estado de pago.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(EstadoPagoFindViewModel model)
        {
            bool result = await estadosPagoService.Delete(model.EstadoPagoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
