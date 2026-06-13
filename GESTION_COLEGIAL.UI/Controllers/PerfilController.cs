using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Mi Perfil")]
    public class PerfilController : BaseController
    {
        private readonly PerfilService _service = new PerfilService();

        public async Task<ActionResult> Index()
        {
            int usuId = GetUsuarioId();
            var perfil = await _service.FindAsync(usuId);
            return View(perfil);
        }
    }
}
