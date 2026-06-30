using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Nomina Docentes")]
    public class NominaDocentesController : BaseController
    {
        private readonly NominaDocentesService _service = new NominaDocentesService();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync(int? anio, int? semestre)
        {
            var a = anio ?? 2026;
            var s = semestre ?? 1;
            var result = await _service.ListMesAsync(a, s);
            return AjaxResult(result);
        }

        public async Task<ActionResult> DetalleAsync(int empId, int? anio, int? semestre)
        {
            var a = anio ?? 2026;
            var s = semestre ?? 1;
            var result = await _service.DetalleAsync(empId, a, s);
            return AjaxResult(result, true);
        }
    }
}
