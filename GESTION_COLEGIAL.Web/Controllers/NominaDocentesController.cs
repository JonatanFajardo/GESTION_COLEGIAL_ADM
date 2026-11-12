using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    // ============================================================================
    //  HUECOS DE API PENDIENTES (NO implementados — requieren persistencia nueva
    //  en la API externa Gestion.Colegial.Api). Hoy la nómina se CALCULA en vivo
    //  desde los horarios; no se guarda ni se versiona ningún período.
    //
    //  Para completar el flujo de nómina falta que la API exponga:
    //    1. Cierre de período      → POST /nomina/cierre { anio, semestre } que
    //       congele el cálculo (snapshot) para que no cambie si luego se editan
    //       horarios. Sin esto, la planilla exportada/impresa es solo una foto
    //       del momento, no un registro oficial inmutable.
    //    2. Firma de conformidad   → persistir firma/aprobación del responsable
    //       (RRHH/Dirección) sobre el período cerrado (usuario, fecha, estado).
    //    3. Marcar como pagada     → PUT /nomina/{periodo}/pagar que registre
    //       fecha de pago y forma de pago por docente o por planilla completa.
    //
    //  Mientras no existan esos endpoints, esta pantalla solo ofrece exportación
    //  (CSV en cliente) e impresión/PDF (window.print sobre la vista Planilla).
    // ============================================================================
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

        /// <summary>
        /// Vista imprimible de la planilla de nómina (membrete + detalle + totales).
        /// Reutiliza el mismo cálculo que ListAsync; sin Layout, pensada para
        /// abrirse en una pestaña nueva e imprimirse con window.print().
        /// </summary>
        public async Task<ActionResult> PlanillaAsync(int? anio, int? semestre)
        {
            var a = anio ?? 2026;
            var s = semestre ?? 1;
            var result = await _service.ListMesAsync(a, s);
            ViewBag.Anio = a;
            ViewBag.Semestre = s;
            return View("Planilla", result);
        }
    }
}
