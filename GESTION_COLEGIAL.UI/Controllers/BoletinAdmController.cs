using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Boletín de calificaciones")]
    public class BoletinAdmController : BaseController
    {
        private readonly BoletinAdmService _service = new BoletinAdmService();
        private readonly AlumnosService    _alumnos = new AlumnosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var alumnos = await _alumnos.ListAsync();
            return Json(new
            {
                alumnos = alumnos.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre })
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: BoletinAdm/GetAsync?Alu_Id=&Anio=
        public async Task<ActionResult> GetAsync(int Alu_Id, int Anio)
        {
            var result = await _service.GetAsync(Alu_Id, Anio);
            return AjaxResult(result);
        }
    }
}
