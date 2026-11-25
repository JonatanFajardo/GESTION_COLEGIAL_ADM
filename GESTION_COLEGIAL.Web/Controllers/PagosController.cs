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
        private readonly DescuentosService descuentosService = new DescuentosService();
        private readonly TarifaService tarifaService = new TarifaService();

        /// <summary>
        /// Descuentos disponibles para la pantalla de cobro (quien cobra no necesita el permiso del catálogo).
        /// </summary>
        [SessionManager("Crear pagos")]
        public async Task<ActionResult> DescuentosDelCobroAsync()
        {
            var lista = await descuentosService.ListAsync() ?? Enumerable.Empty<DescuentoListViewModel>();
            return AjaxResult(lista.Where(d => d.EsActivo && !d.EsEliminado)
                                   .Select(d => new { d.DescuentoId, d.Descripcion, d.EsActivo, d.EsEliminado }).ToList());
        }

        /// <summary>
        /// Tarifas vigentes para sugerir el monto de un cargo desde la pantalla de cobro.
        /// </summary>
        [SessionManager("Crear pagos")]
        public async Task<ActionResult> TarifasDelCobroAsync()
        {
            var lista = await tarifaService.ListAsync() ?? Enumerable.Empty<TarifaListViewModel>();
            return AjaxResult(lista.Where(t => t.EsActivo && !t.EsEliminado)
                                   .Select(t => new { t.ConceptoPagoId, t.Monto, t.AnioVigencia, t.EsEliminado }).ToList());
        }

        /// <summary>
        /// Movimientos de caja: los cobros de hoy o de cualquier rango de fechas.
        /// Reemplaza a "Pagos del mes", "Pagos del día" e "Historial de pagos".
        /// </summary>
        /// <param name="periodo">Atajo inicial: hoy, semana, mes o anio.</param>
        public ActionResult Index(string periodo = null)
        {
            var validos = new[] { "hoy", "semana", "mes", "anio" };
            var p = (periodo ?? "").ToLowerInvariant();
            ViewBag.Periodo = validos.Contains(p) ? p : "hoy";
            ViewBag.PuedeCrear = TienePantalla("Crear pagos")
                || string.Equals(Session[SesionUsuario.RoleName] as string, "admin", StringComparison.OrdinalIgnoreCase);
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de registro de pago.
        /// Acepta identidad (o alumnoId) para precargar el alumno al llegar
        /// desde Cartera/Estado de Cuenta.
        /// </summary>
        [SessionManager("Crear pagos")]
        public ActionResult NuevoPago(string identidad = null, int alumnoId = 0)
        {
            ViewBag.Identidad = identidad ?? string.Empty;
            ViewBag.AlumnoId = alumnoId;
            return View();
        }

        /// <summary>Antes: Pagos del día. Ahora vive en Movimientos de caja.</summary>
        public ActionResult ListaDia() => RedirectToAction("Index", new { periodo = "hoy" });

        /// <summary>
        /// Acción asincrónica para obtener la lista de pagos (todos).
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await pagosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Pagos entre dos fechas (ambas incluidas), anulados incluidos, del más reciente al más viejo.
        /// </summary>
        public async Task<ActionResult> ListByRangoAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var d = (desde ?? DateTime.Today).Date;
            var h = (hasta ?? DateTime.Today).Date;
            if (h < d) { var x = d; d = h; h = x; }
            var result = (await pagosService.ListByRangoFechasAsync(d, h) ?? Enumerable.Empty<PagoListViewModel>())
                .OrderByDescending(p => p.FechaPago)
                .ToList();
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
        [SessionManager("Crear pagos")]
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

            // Quién cobra lo fija la sesión, nunca el navegador.
            model.UsuarioId = GetUsuarioId();

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
