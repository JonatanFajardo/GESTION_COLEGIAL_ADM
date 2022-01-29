using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class PageError404Controller : Controller
    {
        [HandleError]
        // GET: PageError404
        public ActionResult Index()
        {
            return View();
        }
    }
}