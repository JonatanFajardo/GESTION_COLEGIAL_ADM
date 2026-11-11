using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Top Alumnos con Deuda")]
    public class TopDeudoresController : BaseController
    {
        private readonly TopDeudoresService _service = new TopDeudoresService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }
    }
}
