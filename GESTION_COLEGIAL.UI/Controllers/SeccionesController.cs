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

        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await seccionesService.Detail(id);
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
            if (model.Sec_Id == 0)
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
        /// <param name="Sec_Id">ID de la sección.</param>
        /// <param name="Sec_Descripcion">Descripción de la sección.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Sec_Id, string Sec_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Sec_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await seccionesService.Exist(Sec_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Sec_Id;
                return (firstValue == Sec_Id) ? Json(true) : Json(msjExist);
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
            bool result = await seccionesService.Delete(model.Sec_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
