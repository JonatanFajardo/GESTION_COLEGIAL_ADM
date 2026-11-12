using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Estado de Resultados")]
    public class EstadoResultadosController : BaseController
    {
        private readonly EstadoResultadosService _service = new EstadoResultadosService();

        public ActionResult Index()
        {
            ViewBag.AnioActual = DateTime.Now.Year;
            ViewBag.MesActual  = DateTime.Now.Month;
            return View();
        }

        public async Task<ActionResult> GetAsync(int? Anio = null, int? Mes = null)
        {
            int anio = Anio ?? DateTime.Now.Year;
            int mes  = Mes  ?? DateTime.Now.Month;
            var result = await _service.GetAsync(anio, mes);
            return AjaxResult(result);
        }
    }
}
