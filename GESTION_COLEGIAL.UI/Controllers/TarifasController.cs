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
        /// Acción asincrónica para mostrar la vista de creación de tarifa.
        /// </summary>
        public async Task<ActionResult> CreateAsync()
        {
            var model = new TarifaViewModel();
            var load = await Load(model);
            return View(load);
        }

        /// <summary>
        /// Acción asincrónica para mostrar la vista de edición de tarifa.
        /// </summary>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await tarifaService.Find(id);
            var tarifaViewModel = MapToTarifaViewModel(result);
            var load = await Load(tarifaViewModel);

            return View("CreateAsync", load);
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
        /// Acción asincrónica para guardar una tarifa (crear o editar).
        /// </summary>
        /// <param name="model">Modelo de vista de la tarifa.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> Save(TarifaViewModel model)
        {
            var tarifaFind = MapToTarifaFindViewModel(model);

            if (model.Tar_Id == 0)
            {
                bool result = await tarifaService.Create(tarifaFind);

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
                bool result = await tarifaService.Edit(tarifaFind);

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

        /// <summary>
        /// Carga la información necesaria para la vista de creación de tarifa.
        /// </summary>
        /// <param name="model">Modelo de vista de la tarifa.</param>
        /// <returns>Modelo de vista de la tarifa cargado con los datos necesarios.</returns>
        public async Task<TarifaViewModel> Load(TarifaViewModel model)
        {
            return await tarifaService.Dropdown(model);
        }

        /// <summary>
        /// Mapea TarifaFindViewModel a TarifaViewModel.
        /// </summary>
        private TarifaViewModel MapToTarifaViewModel(TarifaFindViewModel source)
        {
            return new TarifaViewModel
            {
                Tar_Id = source.TarifaId,
                ConceptoPago_Id = source.ConceptoPagoId,
                Nivel_Id = source.NivelId,
                CursoNivel_Id = source.CursoNivelId,
                Tar_Monto = source.Monto,
                Tar_AnioVigencia = source.AnioVigencia,
                Tar_EsEliminado = source.EsEliminado,
                Usu_RegistraId = source.UsuarioRegistraId,
                Tar_FechaRegistro = source.FechaRegistro,
                Usu_ModificaId = source.UsuarioModificaId,
                Tar_FechaModifica = source.FechaModifica
            };
        }

        /// <summary>
        /// Mapea TarifaViewModel a TarifaFindViewModel.
        /// </summary>
        private TarifaFindViewModel MapToTarifaFindViewModel(TarifaViewModel source)
        {
            return new TarifaFindViewModel
            {
                TarifaId = source.Tar_Id,
                ConceptoPagoId = source.ConceptoPago_Id,
                NivelId = source.Nivel_Id,
                CursoNivelId = source.CursoNivel_Id,
                Monto = source.Tar_Monto,
                AnioVigencia = source.Tar_AnioVigencia,
                EsEliminado = source.Tar_EsEliminado,
                UsuarioRegistraId = source.Usu_RegistraId,
                FechaRegistro = source.Tar_FechaRegistro,
                UsuarioModificaId = source.Usu_ModificaId,
                FechaModifica = source.Tar_FechaModifica
            };
        }
    }
}
