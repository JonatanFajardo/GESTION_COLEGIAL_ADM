using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EncargadosController : BaseController
    {
        // GET: Encargados
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