using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Matriz Rol Pantallas")]
    public class MatrizRolPantallasController : BaseController
    {
        private readonly MatrizRolPantallasService _service = new MatrizRolPantallasService();

        public ActionResult Index() => View();

        public async Task<ActionResult> GetAsync()
        {
            var result = await _service.GetAsync();
            return AjaxResult(result);
        }
    }
}
