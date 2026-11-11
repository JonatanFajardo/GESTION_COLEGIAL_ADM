using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Morosidad por Nivel")]
    public class MorosidadPorNivelController : BaseController
    {
        private readonly MorosidadPorNivelService _service = new MorosidadPorNivelService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }
    }
}
