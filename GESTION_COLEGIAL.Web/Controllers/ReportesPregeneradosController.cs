using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Reportes pre-generados")]
    public class ReportesPregeneradosController : BaseController
    {
        public ActionResult Index() => View();
    }
}
