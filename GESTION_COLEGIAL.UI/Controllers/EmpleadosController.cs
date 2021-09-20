using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EmpleadosController : BaseController
    {
        // GET: Empleados
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