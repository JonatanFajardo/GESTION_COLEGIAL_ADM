using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Alumnos en Riesgo Académico")]
    public class AlumnosEnRiesgoController : BaseController
    {
        private readonly AlumnosEnRiesgoService _service = new AlumnosEnRiesgoService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(string PromedioMinimo = null, string Anio = null)
        {
            var result = await _service.ListAsync(PromedioMinimo, Anio);
            return AjaxResult(result);
        }
    }
}
