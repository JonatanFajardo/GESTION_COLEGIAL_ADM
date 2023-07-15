using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los Encargados.
    /// </summary>
    public class EncargadosController : BaseController
    {
        public string count;
        private readonly EncargadosService encargadosService = new EncargadosService();


        // GET: Encargados
        /// <summary>
        /// Acción para mostrar la vista principal de los Encargados.
        /// </summary>
        /// <returns>Vista principal de los Encargados.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de creación de Encargados.
        /// </summary>
        /// <returns>Vista de creación de Encargados.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de Encargados.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de Encargados.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await encargadosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un Encargado específico.
        /// </summary>
        /// <param name="id">ID del Encargado.</param>
        /// <returns>Vista de creación de Encargados con los detalles del Encargado.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await encargadosService.Find(id);
            return View("CreateAsync", result);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un Encargado.
        /// </summary>
        /// <param name="model">Modelo de vista del Encargado.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> Save(EncargadoViewModel model)
        {
            if (model.Enc_Id == 0)
            {
                bool result = await encargadosService.Create(model);

                //Validamos error
                if (result)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            else
            {
                bool result = await encargadosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Acción asincrónica para eliminar un Encargado.
        /// </summary>
        /// <param name="model">Modelo de vista del Encargado.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(EncargadoViewModel model)
        {
            bool result = await encargadosService.Delete(model.Enc_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
