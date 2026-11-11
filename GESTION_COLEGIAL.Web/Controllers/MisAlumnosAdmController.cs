using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Mis alumnos")]
    public class MisAlumnosAdmController : BaseController
    {
        private readonly MisAlumnosAdmService _service  = new MisAlumnosAdmService();
        private readonly HorariosService      _horarios = new HorariosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var horarios = await _horarios.ListAsync();
            return Json(new
            {
                horarios = horarios.Select(h => new {
                    id    = h.Hor_Id,
                    texto = h.Dia_Descripcion + " " + h.Hor_HoraInicioDescripcion + " — " + h.Mat_Nombre + " (" + h.Sec_Descripcion + ")"
                })
            });
        }

        public async Task<ActionResult> ListAsync(int Hor_Id)
        {
            var result = await _service.ListAsync(Hor_Id);
            return AjaxResult(result);
        }
    }
}
