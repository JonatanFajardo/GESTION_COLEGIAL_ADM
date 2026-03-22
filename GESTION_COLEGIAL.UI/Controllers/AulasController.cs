using GESTION_COLEGIAL.UI.Filters;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Listado de aulas")]
    public class AulasController : BaseController
    {
        // GET: Aulas
        public ActionResult Index()
        {
            return View();
        }
    }
}