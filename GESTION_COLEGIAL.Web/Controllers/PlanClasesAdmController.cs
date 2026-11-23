using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Plan de clases")]
    public class PlanClasesAdmController : BaseController
    {
        private readonly PlanClasesAdmService _service  = new PlanClasesAdmService();
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

        /// <summary>
        /// Temas del temario de la materia (mismo grado, curso, semestre y año del horario) para una semana:
        /// la ventana del plan los ofrece para llenar tema, objetivos y recursos. El alcance lo decide la API.
        /// </summary>
        public async Task<ActionResult> TemasDelTemarioAsync(int horId, int semana)
        {
            if (horId <= 0 || semana < 1 || semana > 30) return AjaxResult(new object[0]);
            int usu = GetUsuarioId();
            var temas = await new TemariosService().TemasDeHorarioAsync(horId, semana, usu);
            return AjaxResult(temas.Select(t => new { t.Tet_Id, t.Tem_Nombre, t.Tet_Titulo, t.Tet_Objetivos, t.Tet_Contenido, t.Tet_Recursos }));
        }

        public async Task<ActionResult> ListAsync(int Hor_Id)
        {
            var result = await _service.ListAsync(Hor_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int Hor_Id, short Pla_Semana, string Pla_Tema,
            string Pla_Objetivos, string Pla_Recursos)
        {
            bool err = await _service.CreateAsync(new {
                Hor_Id, Pla_Semana, Pla_Tema, Pla_Objetivos, Pla_Recursos,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        [HttpPost]
        public async Task<ActionResult> EditAsync(int Pla_Id, short Pla_Semana, string Pla_Tema,
            string Pla_Objetivos, string Pla_Recursos, string Pla_Estado)
        {
            bool err = await _service.EditAsync(new {
                Pla_Id, Pla_Semana, Pla_Tema, Pla_Objetivos, Pla_Recursos, Pla_Estado,
                UsuarioModifica = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int Pla_Id)
        {
            bool err = await _service.DeleteAsync(Pla_Id, GetUsuarioId());
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
