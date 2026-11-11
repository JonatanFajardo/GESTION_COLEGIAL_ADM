using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Aulas")]
    public class DashboardAulasController : BaseController
    {
        private readonly DashboardAulasService _service = new DashboardAulasService();

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<ActionResult> AulasAsync()
        {
            var data = await _service.ObtenerAsync();
            return Json(data);
        }
    }
}
