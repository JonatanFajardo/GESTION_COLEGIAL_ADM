using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Matrícula por Año + Promociones (capa multi-año, expediente/transcript).
    /// Pantalla de Promociones: elegir año/sección origen → ver alumnos →
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
    }
}
