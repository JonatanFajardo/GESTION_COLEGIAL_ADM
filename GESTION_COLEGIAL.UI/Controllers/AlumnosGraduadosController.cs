using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

        private string HistoricosPath => HttpContext.Server.MapPath("~/App_Data/graduados-historicos.json");

        public ActionResult Index() => View();

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
