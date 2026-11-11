using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de cuentas por cobrar.
    /// </summary>
    [SessionManager("Listado de cuentas por cobrar")]
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
        public ActionResult EstadoCuenta(int? alumnoId = null, string identidad = null)
        {
            ViewBag.AlumnoId = alumnoId ?? 0;
            ViewBag.Identidad = identidad ?? string.Empty;
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de cargos pendientes.
        /// </summary>
        public ActionResult CargosPendientes()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de cargos masivos.
        /// </summary>
        public ActionResult CargosMasivos()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de histórico de pagos.
        /// </summary>
        public ActionResult HistoricoPagos()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de moratorias.
        /// </summary>
        public ActionResult Moratorias()
        {
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
        /// Acción asincrónica para obtener los detalles de una cuenta por cobrar específica.
        /// </summary>
        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await cuentasCobrarService.Find(id);
            return AjaxResult(result, true);
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
        /// Acción asincrónica para obtener lista de deudores (cuentas vencidas no pagadas).
        /// </summary>
        public async Task<ActionResult> ListDeudoresAsync()
        {
            var result = await cuentasCobrarService.ListDeudoresAsync();
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

        /// <summary>
        /// Genera mensualidades para un mes específico.
        /// Usa SP: finanza.PR_GenerarMensualidad
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GenerarMensualidadAsync(int mes, int anio)
        {
            try
            {
                var result = await cuentasCobrarService.GenerarMensualidad(mes, anio);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Genera mensualidades para un rango de meses.
        /// Usa SP: finanza.PR_GenerarMensualidadesRango
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GenerarMensualidadesRangoAsync(int mesInicio, int mesFin, int anio)
        {
            try
            {
                var result = await cuentasCobrarService.GenerarMensualidadesRango(mesInicio, mesFin, anio);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los meses pendientes de pago de un alumno.
        /// Usa SP: finanza.PR_MesesPendientesPorAlumno
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> MesesPendientesPorAlumnoAsync(int alumnoId, int? anio = null)
        {
            try
            {
                var result = await cuentasCobrarService.MesesPendientesPorAlumno(alumnoId, anio);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los cargos pendientes de un alumno.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ObtenerCargosPendientesAsync(int alumnoId)
        {
            try
            {
                var result = await cuentasCobrarService.ObtenerCargosPendientes(alumnoId);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Crea un cargo (CuentaCobrar) ad-hoc para un alumno desde el wizard de Nuevo Pago.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CrearCargoAlumnoAsync(CuentaCobrarFindViewModel model)
        {
            try
            {
                model.EstadoPagoId = model.EstadoPagoId == 0 ? 1 : model.EstadoPagoId;
                model.MontoTotal = model.MontoOriginal - model.MontoDescuento + model.MontoMora;
                model.MontoPendiente = model.MontoTotal;
                model.FechaEmision = model.FechaEmision == default ? System.DateTime.Today : model.FechaEmision;

                bool result = await cuentasCobrarService.Create(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el resumen financiero de un alumno.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ObtenerResumenFinancieroAsync(int alumnoId)
        {
            try
            {
                var result = await cuentasCobrarService.ObtenerResumenFinanciero(alumnoId);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el histórico de pagos de un alumno.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ObtenerHistoricoPagosAsync(int alumnoId)
        {
            try
            {
                var result = await cuentasCobrarService.ObtenerHistoricoPagos(alumnoId);
                return AjaxResult(result);
            }
            catch (System.Exception ex)
            {
                return AjaxResult(null, AlertMessage.AlertMessageType.Error, ex.Message);
            }
        }
    }
}