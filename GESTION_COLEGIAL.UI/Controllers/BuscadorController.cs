using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Buscador Global")]
    public class BuscadorController : BaseController
    {
        private readonly BuscadorService _service = new BuscadorService();

        public ActionResult Index() => View();

        public async Task<ActionResult> SearchAsync(string q = null)
        {
            var result = await _service.SearchAsync(q);
            return AjaxResult(result);
        }
    }
}
