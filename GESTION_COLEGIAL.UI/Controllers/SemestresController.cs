using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los Semestres.
    /// </summary>
    public class SemestresController : BaseController
    {
        private readonly SemestresService semestresService = new SemestresService();

        // GET: Semestres
        /// <summary>
        /// Acción para mostrar la vista principal de los Semestres.
        /// </summary>
        /// <returns>Vista principal de los Semestres.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de Semestres.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de Semestres.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await semestresService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un Semestre específico.
        /// </summary>
        /// <param name="id">ID del Semestre.</param>
        /// <returns>Vista de los detalles del Semestre.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await semestresService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un Semestre.
        /// </summary>
        /// <param name="model">Modelo de vista del Semestre.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(SemestreViewModel model)
        {
            if (model.SemestreId == 0)
            {
                bool result = await semestresService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await semestresService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si un Semestre ya existe.
        /// </summary>
        /// <param name="SemestreId">ID del Semestre.</param>
        /// <param name="DescripcionSemestre">Descripción del Semestre.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? SemestreId, string DescripcionSemestre)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionSemestre;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await semestresService.Exist(DescripcionSemestre);
            if (result != null)
            {
                int? firstValue = result.SemestreId;
                return (firstValue == SemestreId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un Semestre.
        /// </summary>
        /// <param name="model">Modelo de vista del Semestre.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(SemestreViewModel model)
        {
            bool result = await semestresService.Delete(model.SemestreId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

