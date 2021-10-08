using System.Threading.Tasks;
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

        //public async Task<ActionResult> List()
        //{
        //    string url = "NivelesEducativos/List";
        //    var result = await CatalogsService.List<NivelEducativoViewModel>(url);
        //    return AjaxResult(result);
        //}

        public ActionResult Create()
        {
            return View("Create");
        }
    }
}