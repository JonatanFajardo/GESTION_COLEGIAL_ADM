using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de pagos.
    /// </summary>
    [SessionManager("Listado de pagos")]
    public class PagosController : BaseController
    {
        private readonly PagosService pagosService = new PagosService();

        /// <summary>
        /// Acción para mostrar la vista principal de pagos.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.PuedeCrear = TienePantalla("Crear pagos");

            // Nombre del mes actual en español (es-HN) para títulos/subtítulos.
            var cultura = new System.Globalization.CultureInfo("es-HN");
            var ahora = DateTime.Now;
            string mesActual = cultura.TextInfo.ToTitleCase(ahora.ToString("MMMM 'de' yyyy", cultura));
            ViewBag.MesActual = mesActual;

            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de registro de pago.
        /// Acepta identidad (o alumnoId) para precargar el alumno al llegar
        /// desde Cartera/Estado de Cuenta.
        /// </summary>
        public ActionResult NuevoPago(string identidad = null, int alumnoId = 0)
        {
            ViewBag.Identidad = identidad ?? string.Empty;
            ViewBag.AlumnoId = alumnoId;
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de pagos del día.
        /// </summary>
        public ActionResult ListaDia()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de pagos.
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await pagosService.ListAsync();

            // El listado de esta pantalla muestra únicamente los pagos del mes en curso.
            // El histórico completo vive en CuentasCobrar/HistoricoPagos.
            var ahora = DateTime.Now;
            if (result != null)
            {
                result = result
                    .Where(p => p.FechaPago.Year == ahora.Year && p.FechaPago.Month == ahora.Month)
                    .ToList();
            }

            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener pagos de un alumno.
        /// </summary>
        public async Task<ActionResult> ListByAlumnoAsync(int alumnoId)
        {
            var result = await pagosService.ListByAlumnoAsync(alumnoId);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener pagos del día.
        /// </summary>
        public async Task<ActionResult> ListByFechaAsync(DateTime? fecha = null)
        {
            var result = await pagosService.ListByFechaAsync(fecha ?? DateTime.Today);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para crear un pago.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] PagoViewModel model)
        {
            if (model == null)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }

            // El wizard envía JSON ([FromBody]), que no pasa por el binder que recorta textos.
            // En MVC 5 sí pasaba: se replica aquí (sin espacios sobrantes y vacío = null).
            model.NumeroReferencia = string.IsNullOrWhiteSpace(model.NumeroReferencia) ? null : model.NumeroReferencia.Trim();
            model.Observaciones = string.IsNullOrWhiteSpace(model.Observaciones) ? null : model.Observaciones.Trim();

            // La fecha de pago la fija el servidor (reloj autoritativo). El wizard no la envía,
            // y sin ella FechaPago quedaba en DateTime.MinValue (0001-01-01), fuera del rango
            // de SQL Server datetime, provocando que el insert del API fallara.
            model.FechaPago = DateTime.Now;

            bool result = await pagosService.Create(model);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        /// <summary>
        /// Acción asincrónica para obtener el recibo de un pago.
        /// </summary>
        public async Task<ActionResult> GetReciboAsync(int pagoId)
        {
            var result = await pagosService.GetReciboAsync(pagoId);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción para mostrar la vista del recibo de pago en formato imprimible.
        /// </summary>
        public async Task<ActionResult> Recibo(int pagoId)
        {
            try
            {
                // Obtener información completa del recibo desde el endpoint específico
                var reciboInfo = await pagosService.GetReciboAsync(pagoId);

                if (reciboInfo == null)
                {
                    return NotFound("No se encontró el pago especificado.");
                }

                // Pasar directamente el PagoDetailViewModel que ya tiene toda la información
                return View(reciboInfo);
            }
            catch (Exception ex)
            {
                return Content("Error al cargar el recibo: " + ex.Message);
            }
        }
    }
}
