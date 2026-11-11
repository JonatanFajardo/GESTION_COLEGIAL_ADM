using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Aging de Cuentas por Cobrar")]
    public class AgingCuentasController : BaseController
    {
        private readonly AgingCuentasService _service = new AgingCuentasService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }
    }
}
