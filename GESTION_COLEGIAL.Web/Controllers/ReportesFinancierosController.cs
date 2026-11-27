using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para reportes financieros.
    /// </summary>
    [SessionManager("Reportes financieros")]
    public class ReportesFinancierosController : BaseController
    {
        private readonly ReportesFinancierosService reportesService = new ReportesFinancierosService();

        /// <summary>
        /// Acción para mostrar el dashboard financiero.
        /// </summary>
        public ActionResult DashboardFinanciero()
        {
            return View();
        }

        /// <summary>
        /// Antes: reporte de morosos. Ahora es el filtro "Vencidas" de Cuentas por cobrar.
        /// </summary>
        public ActionResult Morosos() => RedirectToAction("Index", "CuentasCobrar", new { estado = "vencida" });

        // Ingresos del mes, Proyección de cobros y Comparativa anual viven en el Centro de reportes
        // (las pantallas viejas nunca funcionaron: su API no existía). Los enlaces viejos llevan allá.
        public ActionResult IngresosMes() => RedirectToAction("Index", "CentroReportes", new { r = "ingresos-concepto" });
        public ActionResult ProyeccionCobros() => RedirectToAction("Index", "CentroReportes", new { r = "proyeccion-cobros" });
        public ActionResult ComparativaAnual() => RedirectToAction("Index", "CentroReportes", new { r = "comparativo-cobros" });



        /// <summary>
        /// Acción asincrónica para obtener listado de morosos.
        /// </summary>
        public async Task<ActionResult> ListadoMorososAsync()
        {
            var result = await reportesService.ListadoMorososAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener estado de cuenta de un alumno.
        /// </summary>
        public async Task<ActionResult> EstadoCuentaAlumnoAsync(int alumnoId)
        {
            var result = await reportesService.EstadoCuentaAlumnoAsync(alumnoId);
            return AjaxResult(result);
        }


        /// <summary>
        /// Acción asincrónica para obtener datos del dashboard financiero.
        /// </summary>
        public async Task<ActionResult> DashboardFinancieroAsync()
        {
            var result = await reportesService.DashboardFinancieroAsync();
            return AjaxResult(result);
        }
    }
}
