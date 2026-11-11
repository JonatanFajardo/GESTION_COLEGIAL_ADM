using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Bitácora de actividad")]
    public class BitacoraController : BaseController
    {
        private readonly BitacoraService _service = new BitacoraService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(string Usu_Id = null, string Bit_Tabla = null, string Top = null)
        {
            var result = await _service.ListAsync(Usu_Id, Bit_Tabla, Top);
            return AjaxResult(result);
        }
    }
}
