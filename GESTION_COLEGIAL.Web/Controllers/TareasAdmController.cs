using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Tareas y entregas")]
    public class TareasAdmController : BaseController
    {
        private readonly TareasAdmService _service  = new TareasAdmService();
        private readonly HorariosService  _horarios = new HorariosService();

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

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int Hor_Id, string Tar_Titulo, string Tar_Descripcion,
            string Tar_FechaEntrega, decimal Tar_Punteo)
        {
            bool err = await _service.CreateAsync(new {
                Hor_Id, Tar_Titulo, Tar_Descripcion, Tar_FechaEntrega, Tar_Punteo,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        [HttpPost]
        public async Task<ActionResult> EditAsync(int Tar_Id, string Tar_Titulo, string Tar_Descripcion,
            string Tar_FechaEntrega, decimal Tar_Punteo, string Tar_Estado)
        {
            bool err = await _service.EditAsync(new {
                Tar_Id, Tar_Titulo, Tar_Descripcion, Tar_FechaEntrega, Tar_Punteo, Tar_Estado,
                UsuarioModifica = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int Tar_Id)
        {
            bool err = await _service.DeleteAsync(Tar_Id, GetUsuarioId());
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
