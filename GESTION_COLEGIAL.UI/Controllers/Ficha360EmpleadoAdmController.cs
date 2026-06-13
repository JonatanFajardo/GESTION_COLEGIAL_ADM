using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Ficha 360 empleado")]
    public class Ficha360EmpleadoAdmController : BaseController
    {
        private readonly Ficha360EmpleadoAdmService _service   = new Ficha360EmpleadoAdmService();
        private readonly EmpleadosService           _empleados = new EmpleadosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> EmpleadosDropdownAsync()
        {
            var list = await _empleados.ListAsync();
            return Json(
                list.Select(e => new { id = e.Emp_Id, texto = e.Emp_Nombre }),
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAsync(int Emp_Id)
        {
            var result = await _service.GetAsync(Emp_Id);
            return AjaxResult(result, true);
        }
    }
}
