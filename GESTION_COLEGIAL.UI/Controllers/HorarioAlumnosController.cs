using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class HorarioAlumnosController : BaseController
    {
        // GET: HorarioAlumnos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View("CreateAsync");
        }
    }
}