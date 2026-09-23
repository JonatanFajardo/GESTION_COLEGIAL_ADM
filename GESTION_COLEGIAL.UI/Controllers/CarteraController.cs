using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub de Cartera. Consolida en una sola pantalla con tabs las vistas de deuda
    /// que antes vivían dispersas (Aging, Morosidad por Nivel, Top Deudores,
    /// Pendientes por Cobrar y Deudores/Morosos).
    ///
    /// Este controlador SOLO orquesta la vista. Los datos NO se duplican en backend:
    /// cada tab consume vía AJAX el endpoint existente correspondiente
    ///   - /AgingCuentas/ListAsync
    ///   - /MorosidadPorNivel/ListAsync
    ///   - /TopDeudores/ListAsync
    ///   - /PendientesCobrar/ListAsync
    /// </summary>
    [SessionManager("Cartera")]
    public class CarteraController : BaseController
    {
        private readonly PendientesCobrarService _pendientes = new PendientesCobrarService();

        /// <summary>
        /// Vista principal del hub de cartera (tabs + KPIs).
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Endpoint de datos propio del hub (gateado por "Cartera"), para no
        /// depender de los permisos de las pantallas de deuda que se consolidan
        /// aquí. Todas las pestañas y KPIs se calculan en cliente desde estos
        /// datos (cuentas pendientes con alumno, curso, sección, monto y días).
        /// </summary>
        public async Task<ActionResult> PendientesAsync()
        {
            var result = await _pendientes.ListAsync();
            return AjaxResult(result);
        }
    }
}
