using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Análisis Ingresos vs Egresos")]
    public class AnalisisIngresosController : BaseController
    {
        private readonly AnalisisIngresosService _service = new AnalisisIngresosService();

        public ActionResult Index()
        {
            ViewBag.AnioActual = DateTime.Now.Year;
            return View();
        }

        public async Task<ActionResult> ListAsync(int? Anio = null)
        {
            int anio = Anio ?? DateTime.Now.Year;
            var result = await _service.ListAsync(anio);
            return AjaxResult(result);
        }
    }
}
