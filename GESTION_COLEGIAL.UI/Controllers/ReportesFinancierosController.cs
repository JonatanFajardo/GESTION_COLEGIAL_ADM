using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para reportes financieros.
    /// </summary>
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
        /// Acción para mostrar el reporte de morosos.
        /// </summary>
        public ActionResult Morosos()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar el reporte de ingresos por mes.
        /// </summary>
        public ActionResult IngresosMes()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar el reporte de proyección de cobros.
        /// </summary>
        public ActionResult ProyeccionCobros()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar el reporte comparativo anual.
        /// </summary>
        public ActionResult ComparativaAnual()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener ingresos por mes.
        /// </summary>
        public async Task<ActionResult> IngresosPorMesAsync(int anio, int mes)
        {
            var result = await reportesService.IngresosPorMesAsync(anio, mes);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener proyección de cobros.
        /// </summary>
        public async Task<ActionResult> ProyeccionCobrosAsync(int anio, int mes)
        {
            var result = await reportesService.ProyeccionCobrosAsync(anio, mes);
            return AjaxResult(result);
        }

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
        /// Acción asincrónica para obtener comparativa anual.
        /// </summary>
        public async Task<ActionResult> ComparativaAnualAsync(int anioInicio, int anioFin)
        {
            var result = await reportesService.ComparativaAnualAsync(anioInicio, anioFin);
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
