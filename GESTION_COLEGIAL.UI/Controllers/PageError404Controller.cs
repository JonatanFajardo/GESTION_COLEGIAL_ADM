using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class PageError404Controller : Controller
    {
        [HandleError]
        public ActionResult Index()
        {
            return View();
        }
    }
}