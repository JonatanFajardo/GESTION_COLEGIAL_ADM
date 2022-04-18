using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class HorarioProfesoresController : BaseController
    {
        // GET: HorarioProfesores
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