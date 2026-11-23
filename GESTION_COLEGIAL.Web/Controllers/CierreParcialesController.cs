using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Cierre y publicación de parciales (espejo de Skuul exams.active / publish_result). Un período =
    /// (año, semestre, parcial). Dirección ve qué secciones, materias y docentes tienen notas faltantes
    /// para exigirlas, cierra el parcial para que nadie cambie notas, publica los resultados y, con motivo,
    /// lo reabre si hay una corrección autorizada.
    /// Permisos:
    ///   "Cierre de parciales"          ver estado, faltantes e historial (admin, director, secretaria).
    ///   "Cerrar y publicar parciales"  cerrar, publicar, retirar publicación, reabrir y fechas (admin, director).
    /// La API recibe el permiso y su procedimiento lo vuelve a validar (nunca solo se ocultan botones).
    /// </summary>
    [SessionManager("Cierre de parciales")]
    public class CierreParcialesController : BaseController
    {
        public const string PantallaCerrar = "Cerrar y publicar parciales";

        private readonly CierresParcialService _service = new CierresParcialService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeCerrar() => EsAdmin() || TienePantalla(PantallaCerrar);

        private int? UsuId() =>
            int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        private static object Error(string mensaje) => new { success = false, message = mensaje };

        /// <summary>/cierreparciales?anio=2026 (sin año: el año lectivo activo).</summary>
        public async Task<ActionResult> Index(int? anio)
        {
            var anios = await _service.AniosAsync();
            int activo = anios.FirstOrDefault(a => a.EsActivo)?.Anio ?? DateTime.Today.Year;
            int elegido = anio.HasValue && anios.Any(a => a.Anio == anio.Value) ? anio.Value : activo;
            if (!anios.Any(a => a.Anio == elegido))
                anios.Insert(0, new CierreParcialAnioViewModel { Anio = elegido, EsActivo = elegido == activo });

            return View(new CierreParcialesIndexViewModel
            {
                Anios = anios,
                Anio = elegido,
                AnioActivo = activo,
                PuedeCerrar = PuedeCerrar()
            });
        }

        [HttpGet]
        public async Task<ActionResult> EstadoAsync(int anio) => AjaxResult(await _service.EstadoAsync(anio));

        [HttpGet]
        public async Task<ActionResult> FaltantesAsync(int anio, int semId, int pacId) =>
            AjaxResult(await _service.FaltantesAsync(anio, semId, pacId));

        [HttpGet]
        public async Task<ActionResult> HistorialAsync(int anio) => AjaxResult(await _service.HistorialAsync(anio));

        // ───────────────────────────── Acciones ─────────────────────────────

        /// <summary>Permiso, datos y motivo; quién escribe siempre desde la sesión.</summary>
        private object Preparar(CierreParcialAccionViewModel model, bool motivoObligatorio, string faltaMotivo)
        {
            if (!PuedeCerrar()) return Error("No tiene permiso para cerrar ni publicar parciales.");
            if (model == null) return Error("No llegaron datos.");
            if (model.Anio < 2000 || model.Anio > 2100 || model.Sem_Id <= 0 || model.Pac_Id <= 0)
                return Error("Elija un parcial válido.");
            model.Motivo = string.IsNullOrWhiteSpace(model.Motivo) ? null : model.Motivo.Trim();
            if (motivoObligatorio && model.Motivo == null) return Error(faltaMotivo);
            if (model.Motivo != null && model.Motivo.Length > 500) return Error("El motivo admite hasta 500 caracteres.");
            model.Usuario = UsuId();
            model.PuedeCerrar = true;
            return null;
        }

        private ActionResult Responder(CierreParcialResultadoViewModel r, string general, string exito) =>
            r == null || !r.Ok
                ? Json(Error(r?.Mensaje ?? general))
                : Json(new { success = true, message = exito, estado = r.Estado, publicado = r.Publicado });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CerrarAsync([FromBody] CierreParcialAccionViewModel model)
        {
            var problema = Preparar(model, false, null);
            if (problema != null) return Json(problema);
            if (model.ConfirmarFaltantes && model.Motivo == null)
                return Json(Error("Escriba por qué cierra el parcial con notas faltantes."));
            return Responder(await _service.CerrarAsync(model), "No se pudo cerrar el parcial.",
                "Parcial cerrado. Ya nadie puede registrar ni cambiar sus notas.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PublicarAsync([FromBody] CierreParcialAccionViewModel model)
        {
            var problema = Preparar(model, false, null);
            if (problema != null) return Json(problema);
            model.Motivo = null;
            return Responder(await _service.PublicarAsync(model), "No se pudieron publicar los resultados.",
                "Resultados publicados. El boletín ya no los muestra como provisionales.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DespublicarAsync([FromBody] CierreParcialAccionViewModel model)
        {
            var problema = Preparar(model, true, "Escriba el motivo para retirar la publicación.");
            if (problema != null) return Json(problema);
            return Responder(await _service.DespublicarAsync(model), "No se pudo retirar la publicación.",
                "Se retiró la publicación. El parcial sigue cerrado.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReabrirAsync([FromBody] CierreParcialAccionViewModel model)
        {
            var problema = Preparar(model, true, "Escriba el motivo para reabrir el parcial (qué corrección se autorizó).");
            if (problema != null) return Json(problema);
            return Responder(await _service.ReabrirAsync(model), "No se pudo reabrir el parcial.",
                "Parcial reabierto. Los docentes ya pueden corregir sus notas.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FechasAsync([FromBody] CierreParcialAccionViewModel model)
        {
            var problema = Preparar(model, false, null);
            if (problema != null) return Json(problema);
            model.Motivo = null;
            if (model.FechaInicio.HasValue && model.FechaLimite.HasValue && model.FechaInicio.Value.Date > model.FechaLimite.Value.Date)
                return Json(Error("La fecha de inicio no puede ser posterior a la fecha límite."));
            return Responder(await _service.FechasAsync(model), "No se pudieron guardar las fechas.",
                "Se guardaron las fechas de carga de notas.");
        }
    }
}
