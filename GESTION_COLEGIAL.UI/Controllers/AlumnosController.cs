using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class AlumnosController : BaseController
    {
        // GET: Alumnos
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