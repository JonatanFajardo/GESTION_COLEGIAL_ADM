using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CursosController : BaseController
    {
        // GET: Cursos
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