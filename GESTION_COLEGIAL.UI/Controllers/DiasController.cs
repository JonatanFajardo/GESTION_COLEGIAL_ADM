using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los días.
    /// </summary>
    public class DiasController : BaseController
    {
        private readonly DiasService diasService = new DiasService();

        /// <summary>
        /// Acción para mostrar la vista principal de los días.
        /// </summary>
        /// <returns>Vista principal de los días.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de días.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de días.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await diasService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un día específico.
        /// </summary>
        /// <param name="id">ID del día.</param>
        /// <returns>Vista de creación de día con los detalles del día.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await diasService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un día.
        /// </summary>
        /// <param name="model">Modelo de vista del día.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(DiaViewModel model)
        {
            if (model.DiaId == 0)
            {
                bool result = await diasService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await diasService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si un día ya existe.
        /// </summary>
        /// <param name="DiaId">ID del día.</param>
        /// <param name="DescripcionDia">Descripción del día.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? DiaId, string DescripcionDia)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionDia;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await diasService.Exist(DescripcionDia);
            if (result != null)
            {
                int? firstValue = result.DiaId;
                return (firstValue == DiaId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un día.
        /// </summary>
        /// <param name="model">Modelo de vista del día.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(DiaViewModel model)
        {
            bool result = await diasService.Delete(model.DiaId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

