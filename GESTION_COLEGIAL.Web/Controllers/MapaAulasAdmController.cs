using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Mapa de aulas")]
    public class MapaAulasAdmController : BaseController
    {
        private readonly MapaAulasAdmService _service = new MapaAulasAdmService();

        public ActionResult Index() => View();

        public async Task<ActionResult> GetAsync()
        {
            var result = await _service.GetAsync();
            return AjaxResult(result);
        }
    }
}
