using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Ficha 360 alumno")]
    public class Ficha360AlumnoAdmController : BaseController
    {
        private readonly Ficha360AlumnoAdmService _service = new Ficha360AlumnoAdmService();
        private readonly AlumnosService           _alumnos = new AlumnosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> AlumnosDropdownAsync()
        {
            var list = await _alumnos.ListAsync();
            return Json(
                list.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre }),
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAsync(int Alu_Id)
        {
            var result = await _service.GetAsync(Alu_Id);
            return AjaxResult(result, true);
        }
    }
}
