using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Pantallas del flujo de graduacion (candidatos + backfill).
    /// </summary>
    [SessionManager("Graduación de Alumnos")]
    public class GraduacionAlumnosController : BaseController
    {
        private readonly GraduacionService _service = new GraduacionService();
        private readonly ConfiguracionService _configuracion = new ConfiguracionService();

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<ActionResult> NotaMinimaAsync()
        {
            decimal valor = await _configuracion.GetNotaMinimaAprobacionAsync();
            return Json(new { valor }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> NotaMinimaAsync(decimal valor)
        {
            if (valor < 0 || valor > 100)
                return AjaxResult(new { mensaje = "El valor debe estar entre 0 y 100." }, false);
            int usuarioId = GetUsuarioId();
            bool error = await _configuracion.SetNotaMinimaAprobacionAsync(valor, usuarioId);
            return AjaxResult(new { valor }, !error);
        }

        [HttpGet]
        public async Task<ActionResult> ListCandidatosAsync()
        {
            var data = await _service.ListCandidatosAsync();
            return AjaxResult(data);
        }

        [HttpPost]
        public async Task<ActionResult> GraduarAsync(int alumnoId)
        {
            int usuarioId = GetUsuarioId();
            var resultado = await _service.GraduarAsync(alumnoId, usuarioId);
            return AjaxResult(resultado, resultado.Exitoso);
        }

        public ActionResult BackfillIndex() => View();

        [HttpPost]
        public async Task<ActionResult> BackfillAsync(int? anio, int? topN, bool? dryRun)
        {
            var resultado = await _service.BackfillAsync(anio, topN ?? 50, dryRun ?? true);
            return AjaxResult(resultado);
        }
    }
}
