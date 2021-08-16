using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class HorarioProfesoresController : Controller
    {
        // GET: HorarioProfesores
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("Create");
        }
    }
}