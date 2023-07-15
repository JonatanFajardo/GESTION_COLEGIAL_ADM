using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los empleados.
    /// </summary>
    public class EmpleadosController : BaseController
    {
        private readonly EmpleadosService empleadosService = new EmpleadosService();

        /// <summary>
        /// Acción para mostrar la vista principal de los empleados.
        /// </summary>
        /// <returns>Vista principal de los empleados.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para mostrar la vista de creación de un nuevo empleado.
        /// </summary>
        /// <returns>Vista de creación de empleado.</returns>
        public async Task<ActionResult> CreateAsync()
        {
            var model = new EmpleadoViewModel();
            var load = await Load(model);
            return View(load);
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de empleados.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de empleados.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await empleadosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un empleado específico.
        /// </summary>
        /// <param name="id">ID del empleado.</param>
        /// <returns>Vista de creación de empleado con los detalles del empleado.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await empleadosService.Find(id);
            var load = await Load(result);

            return View("CreateAsync", load);
        }

        /// <summary>
        /// Acción asincrónica para guardar un empleado (crear o editar).
        /// </summary>
        /// <param name="model">Modelo de vista del empleado.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> Save(EmpleadoViewModel model)
        {
            if (model.Emp_Id == 0)
            {
                bool result = await empleadosService.Create(model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return RedirectToAction("Index");
            }
            else
            {
                bool result = await empleadosService.Edit(model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Acción asincrónica para eliminar un empleado.
        /// </summary>
        /// <param name="model">Modelo de vista del empleado.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(EmpleadoViewModel model)
        {
            bool result = await empleadosService.Delete(model.Emp_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        /// <summary>
        /// Carga la información necesaria para la vista de creación de empleado.
        /// </summary>
        /// <param name="model">Modelo de vista del empleado.</param>
        /// <returns>Modelo de vista del empleado cargado con los datos necesarios.</returns>
        public async Task<EmpleadoViewModel> Load(EmpleadoViewModel model)
        {
            return await empleadosService.Dropdown(model);
        }
    }
}
