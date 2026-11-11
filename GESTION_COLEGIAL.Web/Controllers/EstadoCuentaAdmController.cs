using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Estado de cuenta alumno")]
    public class EstadoCuentaAdmController : BaseController
    {
        private readonly EstadoCuentaAdmService _service = new EstadoCuentaAdmService();
        private readonly AlumnosService         _alumnos = new AlumnosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> AlumnosDropdownAsync()
        {
            var list = await _alumnos.ListAsync();
            return Json(list.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre }));
        }

        public async Task<ActionResult> GetAsync(int Alu_Id, int? Anio = null)
        {
            var result = await _service.GetAsync(Alu_Id, Anio);
            return AjaxResult(result, true);
        }
    }
}
