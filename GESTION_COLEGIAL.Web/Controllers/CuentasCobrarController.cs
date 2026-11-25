using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Linq;
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
        private readonly PagosService pagosService = new PagosService();
        private readonly SaldoFavorService saldoFavorService = new SaldoFavorService();
        private readonly AlumnosService alumnosService = new AlumnosService();

        /// <summary>Permiso para anular pagos, perdonar cargos y mover el saldo a favor.</summary>
        public const string PermisoCorregir = "Corregir cuentas";

        /// <summary>
        /// Cuentas por cobrar: una sola lista con filtro Pendientes / Vencidas / Pagadas / Todas.
        /// Reemplaza a Deudores, Cargos pendientes, Moratorias y el reporte de Morosos.
        /// </summary>
        /// <param name="estado">Filtro inicial: pendiente, vencida, pagada o todas.</param>
        public ActionResult Index(string estado = null)
        {
            var validos = new[] { "pendiente", "vencida", "pagada", "todas" };
            var e = (estado ?? "").ToLowerInvariant();
            ViewBag.Estado = validos.Contains(e) ? e : "pendiente";
            return View();
        }

        /// <summary>Antes: Deudores. Ahora es el filtro "Vencidas" de Cuentas por cobrar.</summary>
        public ActionResult Deudores() => RedirectToAction("Index", new { estado = "vencida" });

        /// <summary>Antes: Cargos pendientes. Ahora es el filtro "Pendientes" de Cuentas por cobrar.</summary>
        public ActionResult CargosPendientes() => RedirectToAction("Index", new { estado = "pendiente" });

        /// <summary>Antes: Intereses por mora. Ahora es el filtro "Vencidas" (con la columna Mora).</summary>
        public ActionResult Moratorias() => RedirectToAction("Index", new { estado = "vencida" });

        /// <summary>Antes: Historial de pagos. Ahora vive en Movimientos de caja.</summary>
        public ActionResult HistoricoPagos() => RedirectToAction("Index", "Pagos", new { periodo = "anio" });

        /// <summary>
        /// Cuenta del alumno: cargos, pagos, saldo a favor y correcciones.
        /// Se abre con alumnoId o con la identidad del alumno.
        /// </summary>
        public ActionResult EstadoCuenta(int? alumnoId = null, string identidad = null)
        {
            ViewBag.AlumnoId = alumnoId ?? 0;
            ViewBag.Identidad = identidad ?? string.Empty;
            ViewBag.PuedeCorregir = PuedeCorregir();
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de cargos masivos.
        /// </summary>
        [SessionManager("Crear pagos|Crear cuentas por cobrar")]
        public ActionResult CargosMasivos()
        {
            return View();
        }

        /// <summary>
        /// Alumnos (id, nombre, identidad y ubicación) para el buscador de la cuenta del alumno.
        /// </summary>
        public async Task<ActionResult> AlumnosBuscarAsync()
        {
            var lista = await alumnosService.ListAsync() ?? Enumerable.Empty<AlumnoViewModel>();
            var result = lista.Select(a => new
            {
                a.Alu_Id,
                a.Alu_Nombre,
                a.Per_Identidad,
                a.Niv_Descripcion,
                Grado = (a.Cno_Descripcion ?? "").Trim(),
                a.Cur_Descripcion,
                a.Sec_Descripcion
            }).ToList();
            return Json(result);
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
        [SessionManager("Crear pagos|Crear cuentas por cobrar")]
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
        /// Genera mensualidades para un mes específico.
        /// Usa SP: finanza.PR_GenerarMensualidad
        /// </summary>
        [HttpPost]
        [SessionManager("Crear pagos|Crear cuentas por cobrar")]
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
        [SessionManager("Crear pagos|Crear cuentas por cobrar")]
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
        [SessionManager("Crear pagos|Crear cuentas por cobrar")]
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

        #region Correcciones de la cuenta del alumno

        /// <summary>
        /// Saldo a favor del alumno.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> SaldoFavorAsync(int alumnoId)
        {
            var result = await saldoFavorService.Obtener(alumnoId);
            return AjaxResult(result ?? new SaldoFavorViewModel { AlumnoId = alumnoId, Saldo = 0 });
        }

        /// <summary>
        /// Registro de correcciones de la cuenta del alumno.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> CorreccionesAsync(int alumnoId)
        {
            var result = await saldoFavorService.Correcciones(alumnoId);
            return AjaxResult(result ?? new System.Collections.Generic.List<CorreccionCuentaViewModel>());
        }

        /// <summary>
        /// Anula un pago. Destino: "Devuelto" (se devolvió el dinero) o "SaldoFavor".
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AnularPagoAsync(int pagoId, string motivo, string destino)
        {
            if (!PuedeCorregir()) return SinPermisoCorregir();
            if (pagoId <= 0) return Rechazo("Seleccione el pago a anular.");
            if (string.IsNullOrWhiteSpace(motivo)) return Rechazo("El motivo es obligatorio.");
            if (destino != "Devuelto" && destino != "SaldoFavor") return Rechazo("Indique qué pasó con el dinero.");

            var r = await pagosService.Anular(new AnularPagoViewModel { PagoId = pagoId, Motivo = motivo.Trim(), Destino = destino, UsuarioId = GetUsuarioId() });
            return Respuesta(r, "Pago anulado.");
        }

        /// <summary>
        /// Perdona (rebaja sin cobrar) parte o todo un cargo.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PerdonarAsync(int cuentaCobrarId, decimal monto, string tipo, string motivo)
        {
            if (!PuedeCorregir()) return SinPermisoCorregir();
            if (cuentaCobrarId <= 0) return Rechazo("Seleccione el cargo.");
            if (monto <= 0) return Rechazo("El monto debe ser mayor que cero.");
            if (tipo != "Exoneración" && tipo != "Beca" && tipo != "Ajuste") return Rechazo("Seleccione el tipo.");
            if (string.IsNullOrWhiteSpace(motivo)) return Rechazo("El motivo es obligatorio.");

            var r = await cuentasCobrarService.Perdonar(new PerdonarCargoViewModel { CuentaCobrarId = cuentaCobrarId, Monto = monto, Tipo = tipo, Motivo = motivo.Trim(), UsuarioId = GetUsuarioId() });
            return Respuesta(r, "Cargo rebajado.");
        }

        /// <summary>
        /// Devuelve dinero del saldo a favor al encargado.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DevolverSaldoAsync(int alumnoId, decimal monto, string metodo, string motivo)
        {
            if (!PuedeCorregir()) return SinPermisoCorregir();
            if (alumnoId <= 0) return Rechazo("Seleccione el alumno.");
            if (monto <= 0) return Rechazo("El monto debe ser mayor que cero.");
            if (metodo != "Efectivo" && metodo != "Transferencia" && metodo != "Cheque") return Rechazo("Indique cómo se devuelve el dinero.");
            if (string.IsNullOrWhiteSpace(motivo)) return Rechazo("El motivo es obligatorio.");

            var r = await saldoFavorService.Devolver(new DevolverSaldoViewModel { AlumnoId = alumnoId, Monto = monto, Metodo = metodo, Motivo = motivo.Trim(), UsuarioId = GetUsuarioId() });
            return Respuesta(r, "Devolución registrada.");
        }

        /// <summary>
        /// Aplica el saldo a favor a los cargos pendientes del alumno.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AplicarSaldoAsync(int alumnoId, decimal monto, string motivo)
        {
            if (!PuedeCorregir()) return SinPermisoCorregir();
            if (alumnoId <= 0) return Rechazo("Seleccione el alumno.");
            if (monto <= 0) return Rechazo("El monto debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(motivo)) return Rechazo("El motivo es obligatorio.");

            var r = await saldoFavorService.Aplicar(new AplicarSaldoViewModel { AlumnoId = alumnoId, Monto = monto, Motivo = motivo.Trim(), UsuarioId = GetUsuarioId() });
            return Respuesta(r, "Saldo a favor aplicado.");
        }

        private bool PuedeCorregir() =>
            string.Equals(Session[SesionUsuario.RoleName] as string, "admin", System.StringComparison.OrdinalIgnoreCase)
            || TienePantalla(PermisoCorregir);

        private ActionResult SinPermisoCorregir()
        {
            Response.StatusCode = 403;
            return Json(new { success = false, message = "No tiene permiso para corregir cuentas." });
        }

        private ActionResult Rechazo(string mensaje)
        {
            Response.StatusCode = 400;
            return Json(new { success = false, message = mensaje });
        }

        private ActionResult Respuesta(RespuestaApi r, string exito)
        {
            if (r == null || r.Error)
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = string.IsNullOrWhiteSpace(r?.Mensaje) ? "No se pudo completar la operación." : r.Mensaje });
            }
            return Json(new { success = true, message = string.IsNullOrWhiteSpace(r.Mensaje) ? exito : r.Mensaje });
        }

        #endregion
    }
}