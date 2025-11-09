using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de las secciones.
    /// </summary>
    public class SeccionesController : BaseController
    {
        private readonly SeccionesService seccionesService = new SeccionesService();

        /// <summary>
        /// Acción para mostrar la vista principal de las secciones.
        /// </summary>
        /// <returns>Vista principal de las secciones.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de secciones.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de secciones.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await seccionesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una sección específica.
        /// </summary>
        /// <param name="id">ID de la sección.</param>
        /// <returns>Vista de creación de sección con los detalles de la sección.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await seccionesService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar una sección.
        /// </summary>
        /// <param name="model">Modelo de vista de la sección.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(SeccionViewModel model)
        {
            if (model.SeccionId == 0)
            {
                bool result = await seccionesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await seccionesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe una sección con la descripción proporcionada.
        /// </summary>
        /// <param name="SeccionId">ID de la sección.</param>
        /// <param name="DescripcionSeccion">Descripción de la sección.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? SeccionId, string DescripcionSeccion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionSeccion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await seccionesService.Exist(DescripcionSeccion);
            if (result != null)
            {
                int? firstValue = result.SeccionId;
                return (firstValue == SeccionId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una sección.
        /// </summary>
        /// <param name="model">Modelo de vista de la sección.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(SeccionViewModel model)
        {
            bool result = await seccionesService.Delete(model.SeccionId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

