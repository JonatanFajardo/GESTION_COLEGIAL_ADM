using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los cursos niveles.
    /// </summary>
    public class CursosNivelesController : BaseController
    {
        private readonly CursosNivelesService cursosNivelesService = new CursosNivelesService();

        /// <summary>
        /// Acción para mostrar la vista principal de los cursos niveles.
        /// </summary>
        /// <returns>Vista principal de los cursos niveles.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de cursos niveles.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de cursos niveles.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await cursosNivelesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un curso nivel específico.
        /// </summary>
        /// <param name="id">ID del curso nivel.</param>
        /// <returns>Vista de creación de curso nivel con los detalles del curso nivel.</returns>
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await cursosNivelesService.Find(id);
			return AjaxResult(result, true);
		}

		/// <summary>
		/// Acción asincrónica para obtener el detalle de un curso nivel específico.
		/// </summary>
		/// <param name="id">ID del curso nivel.</param>
		/// <returns>Resultado con el detalle solicitado.</returns>
		public async Task<ActionResult> DetailAsync(int id)
		{
			var result = await cursosNivelesService.Detail(id);
			return AjaxResult(result, true);
		}

        /// <summary>
        /// Acción asincrónica para crear o editar un curso nivel.
        /// </summary>
        /// <param name="model">Modelo de vista del curso nivel.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(CursoNivelViewModel model)
        {
            if (model.Cun_Id == 0)
            {
                bool result = await cursosNivelesService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await cursosNivelesService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si un curso nivel ya existe.
        /// </summary>
        /// <param name="Cun_Id">ID del curso nivel.</param>
        /// <param name="Cun_Descripcion">Descripción del curso nivel.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Cun_Id, string Cun_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Cun_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await cursosNivelesService.Exist(Cun_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Cun_Id;
                return (firstValue == Cun_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un curso nivel.
        /// </summary>
        /// <param name="model">Modelo de vista del curso nivel.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(CursoNivelViewModel model)
        {
            bool result = await cursosNivelesService.Delete(model.Cun_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
