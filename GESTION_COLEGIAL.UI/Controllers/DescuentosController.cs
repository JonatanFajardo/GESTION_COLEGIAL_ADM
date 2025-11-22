using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de descuentos.
    /// </summary>
    public class DescuentosController : BaseController
    {
        private readonly DescuentosService descuentosService = new DescuentosService();

        /// <summary>
        /// Acción para mostrar la vista principal de descuentos.
        /// </summary>
        /// <returns>Vista principal de descuentos.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de descuentos.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de descuentos.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await descuentosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un descuento específico.
        /// </summary>
        /// <param name="id">ID del descuento.</param>
        /// <returns>Resultado con los detalles del descuento.</returns>
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await descuentosService.Find(id);
			return AjaxResult(result, true);
		}

		/// <summary>
		/// Acción asincrónica para obtener el detalle de un descuento específico.
		/// </summary>
		/// <param name="id">ID del descuento.</param>
		/// <returns>Resultado con el detalle del descuento.</returns>
		public async Task<ActionResult> DetailAsync(int id)
		{
			var result = await descuentosService.Detail(id);
			return AjaxResult(result, true);
		}

        /// <summary>
        /// Acción asincrónica para crear o editar un descuento.
        /// </summary>
        /// <param name="model">Modelo de vista del descuento.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(DescuentoFindViewModel model)
        {
            if (model.DescuentoId == 0)
            {
                bool result = await descuentosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await descuentosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe un descuento con la descripción proporcionada.
        /// </summary>
        /// <param name="DescuentoId">ID del descuento.</param>
        /// <param name="Descripcion">Descripción del descuento.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? DescuentoId, string Descripcion)
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
            var result = await descuentosService.Exist(Descripcion);
            if (result != null)
            {
                int? firstValue = result.DescuentoId;
                return (firstValue == DescuentoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un descuento.
        /// </summary>
        /// <param name="model">Modelo de vista del descuento.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(DescuentoFindViewModel model)
        {
            bool result = await descuentosService.Delete(model.DescuentoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
