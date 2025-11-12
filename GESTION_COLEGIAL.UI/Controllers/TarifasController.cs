using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de tarifas por nivel/curso.
    /// </summary>
    public class TarifasController : BaseController
    {
        private readonly TarifaService tarifaService = new TarifaService();

        /// <summary>
        /// Acción para mostrar la vista principal de tarifas.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de creación de tarifa.
        /// </summary>
        public ActionResult CreateAsync()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de edición de tarifa.
        /// </summary>
        public ActionResult FindAsync(int id)
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de tarifas.
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await tarifaService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de una tarifa específica.
        /// </summary>
        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await tarifaService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar una tarifa.
        /// </summary>
        /// <param name="model">Modelo de vista de la tarifa.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(TarifaViewModel model)
        {
            bool result = await tarifaService.Delete(model.Tar_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
