using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los parciales.
    /// </summary>
    public class ParcialesController : BaseController
    {
        private readonly ParcialesService parcialesService = new ParcialesService();

        /// <summary>
        /// Acción para mostrar la vista principal de los parciales.
        /// </summary>
        /// <returns>Vista principal de los parciales.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de parciales.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de parciales.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await parcialesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un parcial específico.
        /// </summary>
        /// <param name="id">ID del parcial.</param>
        /// <returns>Vista de creación de parcial con los detalles del parcial.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await parcialesService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un parcial.
        /// </summary>
        /// <param name="model">Modelo de vista del parcial.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(ParcialViewModel model)
        {
            if (model.ParcialId == 0)
            {
                bool result = await parcialesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await parcialesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si existe un parcial con la descripción proporcionada.
        /// </summary>
        /// <param name="ParcialId">ID del parcial.</param>
        /// <param name="DescripcionParcial">Descripción del parcial.</param>
        /// <returns>Resultado de la verificación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? ParcialId, string DescripcionParcial)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionParcial;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await parcialesService.Exist(DescripcionParcial);
            if (result != null)
            {
                int? firstValue = result.ParcialId;
                return (firstValue == ParcialId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un parcial.
        /// </summary>
        /// <param name="model">Modelo de vista del parcial.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ParcialViewModel model)
        {
            bool result = await parcialesService.Delete(model.ParcialId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

