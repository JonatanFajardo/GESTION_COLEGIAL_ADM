using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de las modalidades.
    /// </summary>
    public class ModalidadesController : BaseController
    {
        private readonly ModalidadesService modalidadesService = new ModalidadesService();

        /// <summary>
        /// Acción para mostrar la vista principal de las modalidades.
        /// </summary>
        /// <returns>Vista principal de las modalidades.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de modalidades.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de modalidades.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await modalidadesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una modalidad específica.
        /// </summary>
        /// <param name="id">ID de la modalidad.</param>
        /// <returns>Vista de creación de modalidad con los detalles de la modalidad.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await modalidadesService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar una modalidad.
        /// </summary>
        /// <param name="model">Modelo de vista de la modalidad.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(ModalidadViewModel model)
        {
            if (model.ModalidadId == 0)
            {
                bool result = await modalidadesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await modalidadesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe una modalidad con la descripción proporcionada.
        /// </summary>
        /// <param name="ModalidadId">ID de la modalidad.</param>
        /// <param name="DescripcionModalidad">Descripción de la modalidad.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? ModalidadId, string DescripcionModalidad)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionModalidad;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await modalidadesService.Exist(DescripcionModalidad);
            if (result != null)
            {
                int? firstValue = result.ModalidadId;
                return (firstValue == ModalidadId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una modalidad.
        /// </summary>
        /// <param name="model">Modelo de vista de la modalidad.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ModalidadViewModel model)
        {
            bool result = await modalidadesService.Delete(model.ModalidadId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

