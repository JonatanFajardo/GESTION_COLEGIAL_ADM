using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Búsqueda de Alumnos")]
    public class AlumnosBusquedaController : BaseController
    {
        private readonly AlumnosBusquedaService _service = new AlumnosBusquedaService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(string Termino = null)
        {
            var result = await _service.ListAsync(Termino);
            return AjaxResult(result);
        }
    }
}
