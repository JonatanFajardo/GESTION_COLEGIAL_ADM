using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador de Períodos del año lectivo (Fase 2). Alimenta el editor de
    /// períodos embebido en el paso 1 del wizard de Año Lectivo.
    /// </summary>
    [SessionManager("Períodos del año lectivo")]
    public class PeriodosAnioController : BaseController
    {
        private readonly PeriodosAnioService periodosAnioService = new PeriodosAnioService();

        /// <summary>Lista los períodos de un año lectivo (para el editor del paso 1).</summary>
        public async Task<ActionResult> ListAsync(int anioId)
        {
            var result = await periodosAnioService.ListAsync(anioId);
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await periodosAnioService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(PeriodoAnioViewModel model)
        {
            if (model.Pan_Estado == 0) model.Pan_Estado = 1; // Borrador por defecto

            if (model.Pan_Id == 0)
            {
                bool result = await periodosAnioService.Create(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await periodosAnioService.Edit(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(PeriodoAnioViewModel model)
        {
            bool result = await periodosAnioService.Delete(model.Pan_Id);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
