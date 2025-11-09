using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los Parentescos.
    /// </summary>
    public class ParentescosController : BaseController
    {
        private readonly ParentescosService parentescosService = new ParentescosService();

        // GET: Parentescos
        /// <summary>
        /// Acción para mostrar la vista principal de los Parentescos.
        /// </summary>
        /// <returns>Vista principal de los Parentescos.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de Parentescos.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de Parentescos.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await parentescosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un Parentesco específico.
        /// </summary>
        /// <param name="id">ID del Parentesco.</param>
        /// <returns>Vista de los detalles del Parentesco.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await parentescosService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un Parentesco.
        /// </summary>
        /// <param name="model">Modelo de vista del Parentesco.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(ParentescoViewModel model)
        {
            if (model.ParentescoId == 0)
            {
                bool result = await parentescosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await parentescosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si un Parentesco ya existe.
        /// </summary>
        /// <param name="ParentescoId">ID del Parentesco.</param>
        /// <param name="DescripcionParentesco">Descripción del Parentesco.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? ParentescoId, string DescripcionParentesco)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionParentesco;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await parentescosService.Exist(DescripcionParentesco);
            if (result != null)
            {
                int? firstValue = result.ParentescoId;
                return (firstValue == ParentescoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un Parentesco.
        /// </summary>
        /// <param name="model">Modelo de vista del Parentesco.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ParentescoViewModel model)
        {
            bool result = await parentescosService.Delete(model.ParentescoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

