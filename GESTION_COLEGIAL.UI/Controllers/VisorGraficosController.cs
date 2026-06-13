using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Visor de Gráficos")]
    public class VisorGraficosController : BaseController
    {
        private readonly DashboardFinancieroService _finanzas = new DashboardFinancieroService();
        private readonly DashboardAdminService      _admin    = new DashboardAdminService();

        public ActionResult Index()
        {
            ViewBag.AnioActual = DateTime.Now.Year;
            return View();
        }

        public async Task<ActionResult> FinancieroAsync()
        {
            var result = await _finanzas.Resumen();
            return AjaxResult(result);
        }

        public async Task<ActionResult> ResumenAsync()
        {
            var result = await _admin.Resumen();
            return AjaxResult(result);
        }
    }
}
