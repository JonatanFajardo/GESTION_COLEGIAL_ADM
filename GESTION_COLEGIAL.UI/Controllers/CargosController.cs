using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los cargos.
    /// </summary>
    public class CargosController : BaseController
    {
        private readonly CargosService cargosService = new CargosService();

        /// <summary>
        /// Acción para mostrar la vista principal de los cargos.
        /// </summary>
        /// <returns>Vista principal de los cargos.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de creación de un nuevo cargo.
        /// </summary>
        /// <returns>Vista de creación de cargo.</returns>
        public ActionResult CreateAsync()
        {
            return View("CreateAsync");
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de cargos.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de cargos.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await cargosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un cargo específico.
        /// </summary>
        /// <param name="id">ID del cargo.</param>
        /// <returns>Resultado de la operación con los detalles del cargo.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await cargosService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un cargo.
        /// </summary>
        /// <param name="model">Modelo de vista del cargo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(CargoViewModel model)
        {
            if (model.CargoId == 0)
            {
                bool result = await cargosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await cargosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si ya existe un cargo con la misma descripción.
        /// </summary>
        /// <param name="CargoId">ID del cargo (opcional).</param>
        /// <param name="DescripcionCargo">Descripción del cargo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? CargoId, string DescripcionCargo)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionCargo;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await cargosService.Exist(DescripcionCargo);

            //Validamos error
            if (result != null)
            {
                int? firstValue = result.CargoId;
                return (firstValue == CargoId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un cargo.
        /// </summary>
        /// <param name="model">Modelo de vista del cargo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(CargoViewModel model)
        {
            bool result = await cargosService.Delete(model.CargoId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

