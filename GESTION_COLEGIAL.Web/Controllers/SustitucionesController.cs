using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Sustitución de docente (espejo de Skuul "cover"): Dirección ve cada mañana qué docentes faltan,
    /// qué clases quedan sin profesor y asigna un suplente libre a esa hora; RRHH consulta las horas
    /// que cubrió cada suplente; el docente ve las clases que le toca cubrir y quién cubre las suyas.
    /// Permisos:
    ///   "Sustituciones de docentes"    ver la pantalla. Sin "Ver todas" solo se ve lo propio (donde el
    ///                                  empleado de la sesión es el ausente o el suplente), sin acciones.
    ///   "Ver todas las sustituciones"  todos los ausentes, todas las clases y el historial completo.
    ///   "Asignar sustituciones"        marcar/quitar ausencias, asignar y cancelar suplentes.
    /// La API recibe el alcance y el permiso, y su procedimiento vuelve a validar todo.
    /// </summary>
    [SessionManager("Sustituciones de docentes")]
    public class SustitucionesController : BaseController
    {
        public const string PantallaVerTodas = "Ver todas las sustituciones";
        public const string PantallaAsignar = "Asignar sustituciones";

        private readonly SustitucionesService _service = new SustitucionesService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeAsignar() => EsAdmin() || TienePantalla(PantallaAsignar);
        private bool VerTodas() => PuedeAsignar() || TienePantalla(PantallaVerTodas);

        private int? UsuId() =>
            int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        private int? EmpId() =>
            int.TryParse(Session[SesionUsuario.EmpId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        /// <summary>Alcance de lectura: null = todo; si no, el empleado de la sesión (-1 si no tiene: no ve nada).</summary>
        private int? Alcance() => VerTodas() ? (int?)null : (EmpId() ?? -1);

        private static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        private static DateTime? LeerFecha(string valor) =>
            DateTime.TryParseExact((valor ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)
            && d.Year >= 2000 && d.Year <= 2100 ? d.Date : (DateTime?)null;

        private static object Error(string mensaje) => new { success = false, message = mensaje };

        private T Escritor<T>(T model) where T : SustitucionEscritorViewModel
        {
            model.Usuario = UsuId();
            model.PuedeAsignar = PuedeAsignar();
            return model;
        }

        // ───────────────────────────── Pantalla ─────────────────────────────

        /// <summary>/sustituciones?fecha=2026-11-23 (hoy por defecto)</summary>
        public ActionResult Index(string fecha)
        {
            DateTime hoy = HoraHonduras.Ahora.Date;
            DateTime dia = LeerFecha(fecha) ?? hoy;
            return View(new SustitucionesIndexViewModel
            {
                PuedeAsignar = PuedeAsignar(),
                VerTodas = VerTodas(),
                TieneEmpleado = EmpId().HasValue,
                Hoy = hoy.ToString("yyyy-MM-dd"),
                Fecha = dia.ToString("yyyy-MM-dd"),
                InicioMes = new DateTime(hoy.Year, hoy.Month, 1).ToString("yyyy-MM-dd")
            });
        }

        // ───────────────────────────── Lecturas ─────────────────────────────

        /// <summary>
        /// Todo lo de un día: ausentes con sus clases, indicadores del mes y, para quien solo ve lo
        /// suyo, las clases que cubre ese día y las próximas dos semanas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> DiaAsync(string fecha)
        {
            DateTime? dia = LeerFecha(fecha);
            if (!dia.HasValue) return Json(Error("La fecha no es válida."));
            DateTime hoy = HoraHonduras.Ahora.Date;
            int? alcance = Alcance();
            bool todas = alcance == null;

            var filasTask = _service.DiaAsync(dia.Value, alcance);
            var mesTask = _service.MesAsync(dia.Value, alcance);
            Task<List<SustitucionHistorialViewModel>> propiasTask = todas
                ? Task.FromResult(new List<SustitucionHistorialViewModel>())
                : _service.HistorialAsync(dia.Value < hoy ? dia.Value : hoy, (dia.Value > hoy ? dia.Value : hoy).AddDays(14), alcance);
            await Task.WhenAll(filasTask, mesTask, propiasTask);

            List<SustitucionHistorialViewModel> propias = propiasTask.Result
                .Where(s => s.Sus_Estado == "asignada" && s.Emp_IdSuplente == alcance).ToList();

            return Json(new
            {
                success = true,
                fecha = dia.Value.ToString("yyyy-MM-dd"),
                titulo = Cultura.TextInfo.ToTitleCase(dia.Value.ToString("dddd", Cultura)) + " " + dia.Value.ToString("d 'de' MMMM 'de' yyyy", Cultura),
                mesNombre = dia.Value.ToString("MMMM", Cultura),
                esFinDeSemana = dia.Value.DayOfWeek == DayOfWeek.Saturday || dia.Value.DayOfWeek == DayOfWeek.Sunday,
                esPasado = dia.Value < hoy,
                filas = filasTask.Result,
                mes = mesTask.Result,
                cubro = propias.Where(s => s.Sus_Fecha.Date == dia.Value).OrderBy(s => s.MinInicio),
                proximas = propias.Where(s => s.Sus_Fecha.Date >= hoy && s.Sus_Fecha.Date != dia.Value)
                                  .OrderBy(s => s.Sus_Fecha).ThenBy(s => s.MinInicio).Take(20)
            });
        }

        /// <summary>Suplentes posibles para una clase en una fecha (solo quien asigna).</summary>
        [HttpGet]
        public async Task<ActionResult> SugerenciasAsync(int horId, string fecha)
        {
            DateTime? dia = LeerFecha(fecha);
            if (!PuedeAsignar() || horId <= 0 || !dia.HasValue) return AjaxResult(new object[0]);
            return AjaxResult(await _service.SugerenciasAsync(horId, dia.Value));
        }

        /// <summary>Docentes para "Marcar docente ausente" (solo quien asigna).</summary>
        [HttpGet]
        public async Task<ActionResult> DocentesAsync(string fecha)
        {
            DateTime? dia = LeerFecha(fecha);
            if (!PuedeAsignar() || !dia.HasValue) return AjaxResult(new object[0]);
            return AjaxResult(await _service.DocentesAsync(dia.Value));
        }

        /// <summary>Historial de un rango (hasta 400 días), con el alcance de la sesión.</summary>
        [HttpGet]
        public async Task<ActionResult> HistorialAsync(string desde, string hasta)
        {
            DateTime? d = LeerFecha(desde), h = LeerFecha(hasta);
            if (!d.HasValue || !h.HasValue || h.Value < d.Value || (h.Value - d.Value).TotalDays > 400)
                return AjaxResult(new object[0]);
            return AjaxResult(await _service.HistorialAsync(d.Value, h.Value, Alcance()));
        }

        // ───────────────────────────── Escrituras ─────────────────────────────

        private ActionResult Respuesta(SustitucionResultadoViewModel r, string general, string aviso) =>
            r == null || !r.Ok ? Json(Error(r?.Mensaje ?? general)) : Json(new { success = true, id = r.Id, message = aviso });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AsignarAsync([FromBody] SustitucionAsignarViewModel model)
        {
            if (!PuedeAsignar()) return Json(Error("No tiene permiso para asignar sustituciones."));
            if (model == null || model.Hor_Id <= 0) return Json(Error("Elija la clase."));
            if (!model.Sus_Fecha.HasValue) return Json(Error("Indique la fecha de la clase."));
            if (model.Emp_IdSuplente <= 0) return Json(Error("Elija al suplente."));
            if ((model.Sus_Notas ?? "").Trim().Length > 500) return Json(Error("Las notas admiten hasta 500 caracteres."));
            return Respuesta(await _service.AsignarAsync(Escritor(model)), "No se pudo asignar el suplente.", "Se asignó el suplente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelarAsync([FromBody] SustitucionCancelarViewModel model)
        {
            if (!PuedeAsignar()) return Json(Error("No tiene permiso para cancelar sustituciones."));
            if (model == null || model.Sus_Id <= 0) return Json(Error("No llegó la sustitución."));
            if ((model.Motivo ?? "").Trim().Length > 300) return Json(Error("El motivo admite hasta 300 caracteres."));
            return Respuesta(await _service.CancelarAsync(Escritor(model)), "No se pudo cancelar la sustitución.", "Se canceló la sustitución.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarcarAusenciaAsync([FromBody] AusenciaDocenteMarcarViewModel model)
        {
            if (!PuedeAsignar()) return Json(Error("No tiene permiso para marcar ausencias de docentes."));
            if (model == null || model.Emp_Id <= 0) return Json(Error("Elija al docente."));
            if (!model.Aud_Fecha.HasValue) return Json(Error("Indique la fecha de la ausencia."));
            if (string.IsNullOrWhiteSpace(model.Aud_Motivo)) return Json(Error("Escriba el motivo de la ausencia."));
            if (model.Aud_Motivo.Trim().Length > 300) return Json(Error("El motivo admite hasta 300 caracteres."));
            return Respuesta(await _service.MarcarAusenciaAsync(Escritor(model)), "No se pudo marcar la ausencia.", "Se marcó al docente ausente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarAusenciaAsync([FromBody] AusenciaDocenteQuitarViewModel model)
        {
            if (!PuedeAsignar()) return Json(Error("No tiene permiso para quitar ausencias de docentes."));
            if (model == null || model.Aud_Id <= 0) return Json(Error("No llegó la ausencia."));
            return Respuesta(await _service.QuitarAusenciaAsync(Escritor(model)), "No se pudo quitar la ausencia.", "Se quitó la ausencia.");
        }
    }
}
