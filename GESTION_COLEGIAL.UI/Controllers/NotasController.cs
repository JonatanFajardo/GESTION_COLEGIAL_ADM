using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class NotasController : Controller
    {
        // GET: Notas
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