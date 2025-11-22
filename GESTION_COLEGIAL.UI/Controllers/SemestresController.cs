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

        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await semestresService.Detail(id);
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
            if (model.Sem_Id == 0)
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
        /// <param name="Sem_Id">ID del Semestre.</param>
        /// <param name="Sem_Descripcion">Descripción del Semestre.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Sem_Id, string Sem_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Sem_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await semestresService.Exist(Sem_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Sem_Id;
                return (firstValue == Sem_Id) ? Json(true) : Json(msjExist);
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
            bool result = await semestresService.Delete(model.Sem_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
