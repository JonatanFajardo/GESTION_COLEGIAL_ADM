using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("KPIs Financieros")]
    public class KPIsFinancierosController : BaseController
    {
        private readonly KPIsFinancierosService _service = new KPIsFinancierosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> FindAsync(int? Anio, int? Mes)
        {
            int anio = Anio ?? DateTime.Now.Year;
            var result = await _service.FindAsync(anio, Mes);
            return AjaxResult(result);
        }
    }
}
