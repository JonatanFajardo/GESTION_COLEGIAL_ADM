using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Secciones por año + profesor guía (Fase 4 — Clases y profesores).
    /// Alimenta el paso 3 del wizard de Año Lectivo (Views/AniosLectivos/Setup.cshtml).
    /// </summary>
    [SessionManager("Secciones por año")]
    public class SeccionesAnioController : BaseController
    {
        private readonly SeccionesAnioService seccionesAnioService = new SeccionesAnioService();

        /// <summary>Lista las secciones del año lectivo indicado (para el DataTable del paso 3).</summary>
        public async Task<ActionResult> ListAsync(int aniId)
        {
            var result = await seccionesAnioService.ListAsync(aniId);
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await seccionesAnioService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(SeccionAnioViewModel model)
        {
            if (model.Sea_Id == 0)
            {
                bool result = await seccionesAnioService.Create(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await seccionesAnioService.Edit(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(SeccionAnioViewModel model)
        {
            bool result = await seccionesAnioService.Delete(model.Sea_Id);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
