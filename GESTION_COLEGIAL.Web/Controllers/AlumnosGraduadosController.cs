using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Directorio de alumnos graduados (snapshot histórico).
    /// Mezcla graduados vivos (API) con seed histórico en App_Data.
    /// </summary>
    [SessionManager("Directorio de Graduados")]
    public class AlumnosGraduadosController : BaseController
    {
        private readonly AlumnosGraduadosService _service = new AlumnosGraduadosService();

        private string HistoricosPath => RutasApp.MapPath("~/App_Data/graduados-historicos.json");

        /// <summary>Ruta legada: el directorio ahora vive como tab del hub de Graduación.</summary>
        public ActionResult Index() => RedirectToAction("Index", "GraduacionAlumnos", new { tab = "directorio" });

        [HttpGet]
        public async Task<ActionResult> ListAsync()
        {
            var data = await _service.ListAsync(HistoricosPath);
            return AjaxResult(data);
        }

        [HttpGet]
        public async Task<ActionResult> FindAsync(int id)
        {
            var data = await _service.FindAsync(id, HistoricosPath);
            return AjaxResult(data);
        }
    }
}
