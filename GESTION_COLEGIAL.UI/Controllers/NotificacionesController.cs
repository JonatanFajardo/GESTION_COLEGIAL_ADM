using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Notificaciones")]
    public class NotificacionesController : BaseController
    {
        private readonly NotificacionesService _service = new NotificacionesService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(bool soloNoLeidas = false)
        {
            int usuId = GetUsuarioId();
            var result = await _service.ListAsync(usuId, soloNoLeidas);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> MarcarLeidaAsync(int notId)
        {
            var result = await _service.MarcarLeidaAsync(notId);
            return AjaxResult(result);
        }
    }
}
