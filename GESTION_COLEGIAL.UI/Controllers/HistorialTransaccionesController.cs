using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Historial de Transacciones")]
    public class HistorialTransaccionesController : BaseController
    {
        private readonly HistorialTransaccionesService _service = new HistorialTransaccionesService();

        public ActionResult Index()
        {
            var hoy = DateTime.Today;
            ViewBag.FechaDesdeDefault = hoy.AddDays(-30).ToString("yyyy-MM-dd");
            ViewBag.FechaHastaDefault = hoy.ToString("yyyy-MM-dd");
            return View();
        }

        public async Task<ActionResult> ListAsync(int? aluId = null, string fechaDesde = null, string fechaHasta = null)
        {
            DateTime? desde = null;
            DateTime? hasta = null;

            if (!string.IsNullOrWhiteSpace(fechaDesde) && DateTime.TryParse(fechaDesde, out DateTime dDesde))
                desde = dDesde;

            if (!string.IsNullOrWhiteSpace(fechaHasta) && DateTime.TryParse(fechaHasta, out DateTime dHasta))
                hasta = dHasta;

            var result = await _service.ListAsync(aluId, desde, hasta);
            return AjaxResult(result);
        }
    }
}
