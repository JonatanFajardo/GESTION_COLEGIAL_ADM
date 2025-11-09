using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los Niveles Educativos.
    /// </summary>
    public class NivelesEducativosController : BaseController
    {
        private readonly NivelesEducativosService nivelesEducativosService = new NivelesEducativosService();

        // GET: NivelesEducativos
        /// <summary>
        /// Acción para mostrar la vista principal de los Niveles Educativos.
        /// </summary>
        /// <returns>Vista principal de los Niveles Educativos.</returns>
        public ActionResult Index()
        {
            NivelEducativoViewModel model = new NivelEducativoViewModel();
            return View(model);
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de Niveles Educativos.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de Niveles Educativos.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await nivelesEducativosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un Nivel Educativo específico.
        /// </summary>
        /// <param name="id">ID del Nivel Educativo.</param>
        /// <returns>Vista de los detalles del Nivel Educativo.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await nivelesEducativosService.Find(id);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción asincrónica para crear o editar un Nivel Educativo.
        /// </summary>
        /// <param name="model">Modelo de vista del Nivel Educativo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(NivelEducativoViewModel model)
        {
            if (model.NivelId == 0)
            {
                bool result = await nivelesEducativosService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await nivelesEducativosService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        /// <summary>
        /// Acción asincrónica para verificar si un Nivel Educativo ya existe.
        /// </summary>
        /// <param name="NivelId">ID del Nivel Educativo.</param>
        /// <param name="DescripcionNivel">Descripción del Nivel Educativo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? NivelId, string DescripcionNivel)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = DescripcionNivel;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await nivelesEducativosService.Exist(DescripcionNivel);
            if (result != null)
            {
                int? firstValue = result.NivelId;
                return (firstValue == NivelId) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un Nivel Educativo.
        /// </summary>
        /// <param name="model">Modelo de vista del Nivel Educativo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(NivelEducativoViewModel model)
        {
            bool result = await nivelesEducativosService.Delete(model.NivelId);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}

