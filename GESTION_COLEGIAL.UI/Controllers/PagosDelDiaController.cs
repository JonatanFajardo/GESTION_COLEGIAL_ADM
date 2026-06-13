using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pagos del Día")]
    public class PagosDelDiaController : BaseController
    {
        private readonly PagosDelDiaService _service = new PagosDelDiaService();

        public ActionResult Index()
        {
            ViewBag.FechaDefault = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        public async Task<ActionResult> ListAsync(DateTime? fecha = null)
        {
            var result = await _service.ListAsync(fecha ?? DateTime.Today);
            return AjaxResult(result);
        }
    }
}
