using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de cuentas por cobrar.
    /// </summary>
    public class CuentasCobrarController : BaseController
    {
        private readonly CuentasCobrarService cuentasCobrarService = new CuentasCobrarService();

        /// <summary>
        /// Acción para mostrar la vista principal de cuentas por cobrar.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de deudores.
        /// </summary>
        public ActionResult Deudores()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar el estado de cuenta de un alumno.
        /// </summary>
        public ActionResult EstadoCuenta(int alumnoId)
        {
            ViewBag.AlumnoId = alumnoId;
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de cuentas por cobrar.
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await cuentasCobrarService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener cuentas por cobrar de un alumno.
        /// </summary>
        public async Task<ActionResult> ListByAlumnoAsync(int alumnoId)
        {
            var result = await cuentasCobrarService.ListByAlumnoAsync(alumnoId);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener cuentas pendientes.
        /// </summary>
        public async Task<ActionResult> ListPendientesAsync()
        {
            var result = await cuentasCobrarService.ListPendientesAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener cuentas vencidas.
        /// </summary>
        public async Task<ActionResult> ListVencidasAsync()
        {
            var result = await cuentasCobrarService.ListVencidasAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para generar cargos de un alumno.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GenerarCargosAlumnoAsync(int alumnoId, int anio)
        {
            bool result = await cuentasCobrarService.GenerarCargosAlumno(alumnoId, anio);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        /// <summary>
        /// Acción asincrónica para aplicar descuento.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AplicarDescuentoAsync(int cuentaCobrarId, int descuentoId, decimal monto, string justificacion)
        {
            bool result = await cuentasCobrarService.AplicarDescuento(cuentaCobrarId, descuentoId, monto, justificacion);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}