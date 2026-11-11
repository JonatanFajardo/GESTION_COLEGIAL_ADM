using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Listado de personas")]
    public class PersonasController : BaseController
    {
        // GET: Personas
        public ActionResult Index()
        {
            return View();
        }
    }
}