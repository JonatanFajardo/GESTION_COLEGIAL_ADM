using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Conciliación Rápida")]
    public class ConciliacionController : BaseController
    {
        private readonly PagosDelDiaService      _pagos      = new PagosDelDiaService();
        private readonly PendientesCobrarService _pendientes = new PendientesCobrarService();

        public ActionResult Index()
        {
            ViewBag.FechaDefault = DateTime.Today.ToString("yyyy-MM-dd");
            return View();
        }

        public async Task<ActionResult> PagosAsync(DateTime? fecha = null)
        {
            var result = await _pagos.ListAsync(fecha ?? DateTime.Today);
            return AjaxResult(result);
        }

        public async Task<ActionResult> PendientesAsync()
        {
            var result = await _pendientes.ListAsync();
            return AjaxResult(result);
        }
    }
}
