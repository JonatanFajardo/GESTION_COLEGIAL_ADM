using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class MateriasController : Controller
    {
        // GET: Materias
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