using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pendientes por Cobrar")]
    public class PendientesCobrarController : BaseController
    {
        private readonly PendientesCobrarService _service = new PendientesCobrarService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }
    }
}
