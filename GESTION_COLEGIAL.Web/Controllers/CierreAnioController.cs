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
    /// Cierre del año escolar (espejo de Skuul: Abierto → Cerrando → Cerrado → Archivado, con la revisión
    /// guardada al cerrar y reapertura con motivo; y de RosarioSIS: calificación final y "siguiente grado /
    /// repetir" por alumno). Asistente de 4 pasos para Dirección: 1 Revisión · 2 Resultados finales ·
    /// 3 Decisiones de promoción · 4 Cerrar el año.
    /// Permisos:
    ///   "Cierre del año"          ver el asistente, resultados, decisiones e historial (admin, director, secretaria).
    ///   "Cerrar el año"           iniciar el cierre, calcular, reglas, recuperaciones, decidir, confirmar, cerrar y archivar (admin, director).
    ///   "Reabrir el año escolar"  reabrir un año cerrado, con motivo (admin; se asigna a otro rol a propósito).
    /// La API recibe los permisos y el procedimiento los vuelve a validar (nunca solo se ocultan botones).
    /// </summary>
    [SessionManager("Cierre del año|Cerrar el año")]
    public class CierreAnioController : BaseController
    {
        public const string PantallaCerrar = "Cerrar el año";
        public const string PantallaReabrir = "Reabrir el año escolar";

        private readonly CierreAnioService _service = new CierreAnioService();
        private readonly AniosLectivosService _anios = new AniosLectivosService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeCerrar() => EsAdmin() || TienePantalla(PantallaCerrar);
        private bool PuedeReabrir() => EsAdmin() || TienePantalla(PantallaReabrir);

        private int? UsuId() =>
            int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        private static object Error(string mensaje) => new { success = false, message = mensaje };

        /// <summary>/cierreanio?id=5&amp;paso=2 (sin id: el año de trabajo si ya está abierto o en cierre).</summary>
        public async Task<ActionResult> Index(int? id, int paso = 1)
        {
            var anios = ((await _anios.ListAsync(incluirArchivados: true)) ?? Enumerable.Empty<AnioLectivoViewModel>())
                .Where(a => a.Ani_Estado >= 3)
                .OrderByDescending(a => a.Ani_Anio)
                .ToList();

            int elegido = id.HasValue && anios.Any(a => a.Ani_Id == id.Value)
                ? id.Value
                : (anios.FirstOrDefault(a => a.Ani_EsActual)?.Ani_Id
                   ?? anios.FirstOrDefault(a => a.Ani_Estado == 3 || a.Ani_Estado == 4)?.Ani_Id
                   ?? anios.FirstOrDefault()?.Ani_Id ?? 0);

            var resumen = elegido > 0 ? await _service.ResumenAsync(elegido) : null;

            // Avance: con el año abierto solo se ve la revisión; en cierre, cerrado o archivado, los 4 pasos.
            int maximo = resumen == null ? 1 : (resumen.Ani_Estado >= 4 ? 4 : 1);
            if (paso < 1) paso = 1;
            if (paso > maximo) paso = maximo;

            return View(new CierreAnioIndexViewModel
            {
                Anios = anios,
                Resumen = resumen,
                Paso = paso,
                PasoMaximo = maximo,
                PuedeCerrar = PuedeCerrar(),
                PuedeReabrir = PuedeReabrir(),
                AnioNoEncontrado = id.HasValue && !anios.Any(a => a.Ani_Id == id.Value),
                VeParciales = EsAdmin() || TienePantalla("Cierre de parciales"),
                VeInscripcion = EsAdmin() || TienePantalla("Promociones y expediente por año"),
                VeAnios = EsAdmin() || TienePantalla("Listado de años lectivos")
            });
        }

        [HttpGet] public async Task<ActionResult> RevisionAsync(int id) => AjaxResult(await _service.RevisionAsync(id));
        [HttpGet] public async Task<ActionResult> ReglasAsync(int id) => AjaxResult(await _service.ReglasAsync(id));
        [HttpGet] public async Task<ActionResult> SeccionesAsync(int id) => AjaxResult(await _service.SeccionesAsync(id));
        [HttpGet] public async Task<ActionResult> ResultadosAsync(int id, int seaId) => AjaxResult(await _service.ResultadosAsync(id, seaId));
        [HttpGet] public async Task<ActionResult> DecisionesAsync(int id, int seaId) => AjaxResult(await _service.DecisionesAsync(id, seaId));
        [HttpGet] public async Task<ActionResult> HistorialAsync(int id) => AjaxResult(await _service.HistorialAsync(id));

        // ───────────────────────────── Acciones ─────────────────────────────

        /// <summary>Permiso, datos y motivo; quién escribe y los permisos siempre desde la sesión.</summary>
        private object Preparar(CierreAnioAccionViewModel model, bool reabrir = false)
        {
            if (model == null || model.Ani_Id <= 0) return Error("Elija un año lectivo.");
            if (reabrir ? !PuedeReabrir() : !PuedeCerrar())
                return Error(reabrir
                    ? "Reabrir un año cerrado requiere el permiso especial «Reabrir el año escolar»."
                    : "No tiene permiso para cerrar el año escolar.");
            model.Motivo = string.IsNullOrWhiteSpace(model.Motivo) ? null : model.Motivo.Trim();
            if (model.Motivo != null && model.Motivo.Length > 500) return Error("El motivo admite hasta 500 caracteres.");
            model.Usuario = UsuId();
            model.PuedeCerrar = PuedeCerrar();
            model.PuedeReabrir = PuedeReabrir();
            return null;
        }

        private ActionResult Responder(CierreAnioRespuestaViewModel r, string general, string exito) =>
            r == null || !r.Ok
                ? Json(Error(r?.Mensaje ?? general))
                : Json(new { success = true, message = exito, datos = r.Datos });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EstadoAsync([FromBody] CierreAnioAccionViewModel model)
        {
            string accion = (model?.Accion ?? "").Trim().ToLowerInvariant();
            var problema = Preparar(model, accion == "reabrir");
            if (problema != null) return Json(problema);
            if (accion == "reabrir" && model.Motivo == null)
                return Json(Error("Escriba el motivo para reabrir el año (qué corrección se autorizó)."));
            model.Accion = accion;
            string exito = accion switch
            {
                "iniciar" => "El año pasó a «Cerrando». Ya puede calcular los resultados finales.",
                "cancelar" => "Se canceló el cierre: el año vuelve a estar abierto.",
                "cerrar" => "Año cerrado. Sus notas, actividades, asistencia y tareas quedaron en solo lectura.",
                "reabrir" => "Año reabierto: vuelve a «Cerrando» para hacer la corrección autorizada.",
                "archivar" => "Año archivado. Queda como historia y ya no sale en los selectores de año.",
                _ => "Listo."
            };
            var r = await _service.EstadoAsync(model);

            // Al cerrar el año de trabajo, si se pidió: el año siguiente pasa a ser el de trabajo con la MISMA
            // activación de Años lectivos (API AniosLectivos/SetActivoAsync), no con un cambio propio.
            if (r != null && r.Ok && accion == "cerrar" && model.PasarActivo)
            {
                var resumen = await _service.ResumenAsync(model.Ani_Id);
                if (resumen != null && resumen.EsActivo && resumen.SiguienteId.HasValue && (resumen.SiguienteEstado ?? 0) < 5)
                {
                    bool error = await _anios.SetActivo(resumen.SiguienteId.Value);   // true = error (booleano invertido)
                    exito += error
                        ? " No se pudo cambiar el año de trabajo: hágalo en Años lectivos."
                        : $" El año de trabajo pasó a {resumen.SiguienteAnio}.";
                }
            }
            return Responder(r, "No se pudo cambiar el estado del año.", exito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CalcularAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            return Responder(await _service.CalcularAsync(model), "No se pudieron calcular los resultados.", "Resultados calculados.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReglaAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            if (!(model.Niv_Id > 0)) return Json(Error("Elija un nivel válido."));
            if (model.NotaMinima == null || model.NotaMinima < 1 || model.NotaMinima > 100)
                return Json(Error("La nota mínima de aprobación debe estar entre 1 y 100."));
            if (model.MaxRecuperacion == null || model.MaxRecuperacion < 0 || model.MaxRecuperacion > 20)
                return Json(Error("El máximo de materias en recuperación debe estar entre 0 y 20."));
            return Responder(await _service.ReglaAsync(model), "No se pudo guardar la regla.", "Regla guardada. Los resultados se volvieron a clasificar.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecuperacionAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            if (!(model.Rea_Id > 0)) return Json(Error("Elija la materia."));
            if (model.Nota.HasValue && (model.Nota < 0 || model.Nota > 100 || model.Nota != decimal.Truncate(model.Nota.Value)))
                return Json(Error("La nota de recuperación debe ser un número entero entre 0 y 100."));
            return Responder(await _service.RecuperacionAsync(model), "No se pudo guardar la nota de recuperación.",
                model.Nota.HasValue ? "Nota de recuperación guardada." : "Se quitó la nota de recuperación.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DecisionAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            if (!(model.Pra_Id > 0)) return Json(Error("Elija el alumno."));
            if (model.Motivo != null && model.Motivo.Length > 300) return Json(Error("El motivo admite hasta 300 caracteres."));
            return Responder(await _service.DecisionAsync(model), "No se pudo guardar la decisión.", "Decisión guardada.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarSeccionAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            return Responder(await _service.ConfirmarSeccionAsync(model), "No se pudieron confirmar las decisiones.", "Decisiones confirmadas.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeshacerSeccionAsync([FromBody] CierreAnioAccionViewModel model)
        {
            var problema = Preparar(model);
            if (problema != null) return Json(problema);
            if (model.Motivo == null) return Json(Error("Escriba el motivo para deshacer la confirmación."));
            return Responder(await _service.DeshacerSeccionAsync(model), "No se pudo deshacer la confirmación.",
                "Confirmación deshecha: las decisiones de la sección vuelven a revisión.");
        }
    }
}
