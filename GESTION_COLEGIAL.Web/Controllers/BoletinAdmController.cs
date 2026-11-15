using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Boletín de calificaciones")]
    public class BoletinAdmController : BaseController
    {
        private readonly BoletinAdmService _service = new BoletinAdmService();
        private readonly AlumnosService    _alumnos = new AlumnosService();
        private readonly EscalaNotasService _escala = new EscalaNotasService();

        public ActionResult Index() => View();

        // GET: BoletinAdm/EscalaAsync -> rangos de la escala para traducir el promedio a concepto.
        public async Task<ActionResult> EscalaAsync()
        {
            var result = await _escala.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> DropdownsAsync()
        {
            var alumnos = await _alumnos.ListAsync();
            return Json(new
            {
                alumnos = alumnos.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre })
            });
        }

        // GET: BoletinAdm/GetAsync?Alu_Id=&Anio=
        public async Task<ActionResult> GetAsync(int Alu_Id, int Anio)
        {
            var result = await _service.GetAsync(Alu_Id, Anio);
            return AjaxResult(result);
        }
    }
}
