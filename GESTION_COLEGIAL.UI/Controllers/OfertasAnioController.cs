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
    /// Controlador de Ofertas de asignatura por año (Fase 5). Alimenta el paso 4
    /// del wizard de Año Lectivo ("Asignaturas para este año").
    /// </summary>
    [SessionManager("Ofertas de asignatura por año")]
    public class OfertasAnioController : BaseController
    {
        private readonly OfertasAnioService ofertasAnioService = new OfertasAnioService();

        /// <summary>Lista las ofertas de un año lectivo (id = Ani_Id).</summary>
        public async Task<ActionResult> ListAsync(int id)
        {
            var result = await ofertasAnioService.ListAsync(id);
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await ofertasAnioService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(OfertaAnioViewModel model)
        {
            if (model.Ofa_Id == 0)
            {
                bool result = await ofertasAnioService.Create(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await ofertasAnioService.Edit(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(OfertaAnioViewModel model)
        {
            bool result = await ofertasAnioService.Delete(model.Ofa_Id);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
