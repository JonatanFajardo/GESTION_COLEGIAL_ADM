using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Matrícula por Año + Promociones (capa multi-año, expediente/transcript).
    /// Pantalla "Inscripción y promociones": pestaña Inscripción (inscribir a los alumnos en el año)
    /// y pestaña Promover: elegir año/sección origen → ver alumnos →
    /// elegir año/sección destino → promover.
    /// </summary>
    [SessionManager("Promociones y expediente por año")]
    public class MatriculaAnioController : BaseController
    {
        private readonly MatriculaAnioService matriculaAnioService = new MatriculaAnioService();
        private readonly AniosLectivosService aniosLectivosService = new AniosLectivosService();
        private readonly SeccionesAnioService seccionesAnioService = new SeccionesAnioService();

        // GET: MatriculaAnio
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>Años lectivos para los dropdowns (origen/destino).</summary>
        public async Task<ActionResult> AniosAsync()
        {
            var result = await aniosLectivosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>Secciones vivas de un año lectivo (cascada ?aniId=).</summary>
        public async Task<ActionResult> SeccionesAsync(int aniId)
        {
            var result = await seccionesAnioService.ListAsync(aniId);
            return AjaxResult(result);
        }

        /// <summary>Alumnos matriculados en la sección-año seleccionada.</summary>
        public async Task<ActionResult> ListBySeccionAsync(int seaId)
        {
            var result = await matriculaAnioService.ListBySeccionAsync(seaId);
            return AjaxResult(result);
        }

        /// <summary>Expediente (historial por año) de un alumno.</summary>
        public async Task<ActionResult> ExpedienteAsync(int aluId)
        {
            var result = await matriculaAnioService.ExpedienteAsync(aluId);
            return AjaxResult(result);
        }

        /// <summary>Ejecuta la promoción de la sección-año origen a la destino.</summary>
        [HttpPost]
        public async Task<ActionResult> PromoverAsync(int anioOrigen, int seccionOrigen, int anioDestino, int seccionDestino)
        {
            if (anioOrigen <= 0 || seccionOrigen <= 0 || anioDestino <= 0 || seccionDestino <= 0)
            {
                return Json(new { success = false, message = "Seleccione año y sección de origen y destino." });
            }
            if (seccionOrigen == seccionDestino)
            {
                return Json(new { success = false, message = "La sección destino debe ser distinta a la de origen." });
            }

            int promovidos = await matriculaAnioService.Promover(anioOrigen, seccionOrigen, anioDestino, seccionDestino);
            return Json(new { success = true, promovidos = promovidos, message = promovidos + " alumno(s) promovido(s)." });
        }

        // ── Inscripción inicial del año ───────────────────────────────

        /// <summary>Resumen de la inscripción del año: grupos de ubicación de los alumnos vs secciones del año e inscritos.</summary>
        public async Task<ActionResult> ResumenInscripcionAsync(int? aniId)
        {
            var result = await matriculaAnioService.ResumenInscripcionAsync(aniId);
            return AjaxResult(result);
        }

        /// <summary>Crea las secciones del año que faltan según dónde están ubicados los alumnos.</summary>
        [HttpPost]
        public async Task<ActionResult> CrearSeccionesAsync(int aniId)
        {
            if (aniId <= 0)
            {
                return Json(new { success = false, message = "Seleccione un año lectivo." });
            }

            var r = await matriculaAnioService.CrearSeccionesDesdeUbicacionAsync(aniId, null, GetUsuarioId());
            if (r == null)
            {
                return Json(new { success = false, message = "No se pudieron crear las secciones del año." });
            }

            return Json(new { success = true, resultado = r, message = r.SeccionesCreadas + " sección(es) del año creada(s)." });
        }

        /// <summary>Inscribe en el año a todos sus alumnos; antes crea las secciones del año que falten.</summary>
        [HttpPost]
        public async Task<ActionResult> InscribirAsync(int aniId, bool activarEstado = true)
        {
            if (aniId <= 0)
            {
                return Json(new { success = false, message = "Seleccione un año lectivo." });
            }

            var r = await matriculaAnioService.InscribirDesdeUbicacionAsync(aniId, null, true, activarEstado, GetUsuarioId());
            if (r == null)
            {
                return Json(new { success = false, message = "No se pudo completar la inscripción." });
            }

            var partes = new List<string> { (r.Inscritos + r.Reactivados) + " alumno(s) inscrito(s)" };
            if (r.SeccionesCreadas > 0) partes.Add(r.SeccionesCreadas + " sección(es) del año creada(s)");
            if (r.Activados > 0) partes.Add(r.Activados + " pasado(s) a Activo");
            if (r.YaInscritos > 0) partes.Add(r.YaInscritos + " ya estaban inscritos");
            if (r.SinSeccion > 0) partes.Add(r.SinSeccion + " sin sección del año");

            return Json(new { success = true, resultado = r, message = string.Join(" · ", partes) + "." });
        }
    }
}
