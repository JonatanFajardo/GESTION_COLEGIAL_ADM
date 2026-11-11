using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Comparativo de Años")]
    public class ComparativoAniosController : BaseController
    {
        private readonly ComparativoAniosService _service = new ComparativoAniosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(int? AnioBase, int? AnioComparacion)
        {
            int anioBase       = AnioBase       ?? DateTime.Now.Year - 1;
            int anioComparacion = AnioComparacion ?? DateTime.Now.Year;
            var result = await _service.ListAsync(anioBase, anioComparacion);
            return AjaxResult(result);
        }
    }
}
