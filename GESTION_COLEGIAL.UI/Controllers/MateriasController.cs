using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de las Materias.
    /// </summary>
    public class MateriasController : BaseController
    {
        private readonly MateriasService materiasService = new MateriasService();

        // GET: Materias
        /// <summary>
        /// Acción para mostrar la vista principal de las Materias.
        /// </summary>
        /// <returns>Vista principal de las Materias.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de creación de Materias.
        /// </summary>
        /// <returns>Vista de creación de Materias.</returns>
        public ActionResult Create()
        {
            return View("CreateAsync");
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de Materias.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de Materias.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await materiasService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar una Materia.
        /// </summary>
        /// <param name="model">Modelo de vista de la Materia.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(MateriaViewModel model)
        {
            if (model.Mat_Id == 0)
            {
                bool result = await materiasService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await materiasService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una Materia específica.
        /// </summary>
        /// <param name="id">ID de la Materia.</param>
        /// <returns>Vista de creación de Materias con los detalles de la Materia.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await materiasService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para verificar si una Materia ya existe.
        /// </summary>
        /// <param name="Mat_Id">ID de la Materia.</param>
        /// <param name="Mat_Nombre">Nombre de la Materia.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Mat_Id, string Mat_Nombre)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Mat_Nombre;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await materiasService.Exist(Mat_Nombre);
            if (result != null)
            {
                int? firstValue = result.Mat_Id;
                return (firstValue == Mat_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una Materia.
        /// </summary>
        /// <param name="model">Modelo de vista de la Materia.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(MateriaViewModel model)
        {
            bool result = await materiasService.Delete(model.Mat_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
