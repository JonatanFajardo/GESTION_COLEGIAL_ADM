using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class NotasController : BaseController
    {
        // GET: Notas
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