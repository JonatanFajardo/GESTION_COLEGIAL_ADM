using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los cursos.
    /// </summary>
    public class CursosController : BaseController
    {
        private readonly CursosService cursosService = new CursosService();

        /// <summary>
        /// Acción para mostrar la vista principal de los cursos.
        /// </summary>
        /// <returns>Vista principal de los cursos.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de cursos.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de cursos.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await cursosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para mostrar la vista de creación de un nuevo curso.
        /// </summary>
        /// <returns>Vista de creación de curso.</returns>
        public async Task<ActionResult> CreateAsync()
        {
            var model = new CursoViewModel();
            var load = await Load(model);
            return View(load);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un curso específico.
        /// </summary>
        /// <param name="id">ID del curso.</param>
        /// <returns>Vista de creación de curso con los detalles del curso.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await cursosService.Find(id);
            return View("CreateAsync", result);
        }

        /// <summary>
        /// Acción para guardar un curso.
        /// </summary>
        /// <param name="model">Modelo de vista del curso.</param>
        /// <returns>Redirecciona a la vista principal de los cursos.</returns>
        [HttpPost]
        public async Task<ActionResult> Save(CursoViewModel model)
        {
            if (model.CursoId == 0)
            {
                Boolean createResult = await cursosService.Create(model);
                //Validamos error
                if (createResult)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return RedirectToAction("Index");
            }
            else
            {
                Boolean createResult = await cursosService.Create(model);
                //Validamos error
                if (createResult)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Acción asincrónica para eliminar un curso.
        /// </summary>
        /// <param name="model">Modelo de vista del curso.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(CursoViewModel model)
        {
            bool result = await cursosService.Delete(model.CursoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        /// <summary>
        /// Carga información para la vista de creación de curso.
        /// </summary>
        /// <param name="model">Modelo de vista del curso.</param>
        /// <returns>Modelo de vista del curso con la información cargada.</returns>
        public async Task<CursoViewModel> Load(CursoViewModel model)
        {
            var result = await cursosService.Load(model);
            return result;
        }
    }
}

