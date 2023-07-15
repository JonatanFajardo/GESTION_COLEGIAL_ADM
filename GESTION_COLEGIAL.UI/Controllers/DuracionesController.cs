using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de las duraciones.
    /// </summary>
    public class DuracionesController : BaseController
    {
        private readonly DuracionesService duracionesService = new DuracionesService();

        /// <summary>
        /// Acción para mostrar la vista principal de las duraciones.
        /// </summary>
        /// <returns>Vista principal de las duraciones.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de duraciones.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de duraciones.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await duracionesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una duración específica.
        /// </summary>
        /// <param name="id">ID de la duración.</param>
        /// <returns>Vista de creación de duración con los detalles de la duración.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await duracionesService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar una duración.
        /// </summary>
        /// <param name="model">Modelo de vista de la duración.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(DuracionViewModel model)
        {
            if (model.Dur_Id == 0)
            {
                bool result = await duracionesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await duracionesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si una duración ya existe.
        /// </summary>
        /// <param name="Dur_Id">ID de la duración.</param>
        /// <param name="Dur_Descripcion">Descripción de la duración.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Dur_Id, string Dur_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Dur_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await duracionesService.Exist(Dur_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Dur_Id;
                return (firstValue == Dur_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una duración.
        /// </summary>
        /// <param name="model">Modelo de vista de la duración.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(DuracionViewModel model)
        {
            bool result = await duracionesService.Delete(model.Dur_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
