using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Directorio de alumnos graduados (snapshot historico).
    /// </summary>
    [SessionManager("Directorio de Graduados")]
    public class AlumnosGraduadosController : BaseController
    {
        private readonly AlumnosGraduadosService _service = new AlumnosGraduadosService();

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<ActionResult> ListAsync()
        {
            var data = await _service.ListAsync();
            return AjaxResult(data);
        }

        [HttpGet]
        public async Task<ActionResult> FindAsync(int id)
        {
            var data = await _service.FindAsync(id);
            return AjaxResult(data);
        }
    }
}
