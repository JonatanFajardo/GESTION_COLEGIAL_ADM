using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Documentos alumno")]
    public class DocumentosAlumnoAdmController : BaseController
    {
        private readonly DocumentosAlumnoAdmService _service = new DocumentosAlumnoAdmService();
        private readonly AlumnosService             _alumnos = new AlumnosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> AlumnosDropdownAsync()
        {
            var list = await _alumnos.ListAsync();
            return Json(list.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre }));
        }

        public async Task<ActionResult> ListAsync(int Alu_Id)
        {
            var result = await _service.ListAsync(Alu_Id);
            return AjaxResult(result);
        }

        public async Task<ActionResult> PendientesAsync()
        {
            var result = await _service.PendientesAsync();
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAsync(int Doc_Id, string Doc_Estado, string Doc_Observacion)
        {
            bool err = await _service.UpdateAsync(new { Doc_Id, Doc_Estado, Doc_Observacion, UsuarioModifica = GetUsuarioId() });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
