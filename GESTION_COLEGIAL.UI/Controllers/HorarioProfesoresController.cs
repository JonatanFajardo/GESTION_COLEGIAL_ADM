using GESTION_COLEGIAL.UI.Filters;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Listado de horarios")]
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