using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pase de lista")]
    public class AsistenciaAdmController : BaseController
    {
        private readonly AsistenciaAdmService _service  = new AsistenciaAdmService();
        private readonly HorariosService      _horarios = new HorariosService();
        private readonly UsuariosService      _usuarios = new UsuariosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var horariosTask = _horarios.ListAsync();
            // La sesión solo guarda Usu_Id/Rol/Token (ver AccountController.Login), no el Emp_Id
            // del docente. Se resuelve aquí vía Usuarios/FindAsync para poder filtrar sin tocar
            // AccountController/BaseController (fuera del alcance de este cambio).
            var usuarioTask  = _usuarios.Find(GetUsuarioId());

            await Task.WhenAll(horariosTask, usuarioTask);

            var horarios = horariosTask.Result;
            var usuario  = usuarioTask.Result;

            // Si el usuario logueado tiene un Empleado (docente) asociado, mostrar solo sus horarios.
            // Si no tiene Emp_Id (p.ej. un admin sin vínculo a empleado), se deja el listado completo
            // para no romper el acceso de otros roles (Director también usa Pase de Lista).
            if (usuario != null && usuario.Emp_Id > 0)
                horarios = horarios.Where(h => h.Emp_Id == usuario.Emp_Id);

            return Json(new
            {
                horarios = horarios.Select(h => new {
                    id    = h.Hor_Id,
                    texto = h.Dia_Descripcion + " " + h.Hor_HoraInicioDescripcion + " — " + h.Mat_Nombre + " (" + h.Sec_Descripcion + ")"
                })
            });
        }

        public async Task<ActionResult> ListAsync(int Hor_Id, string Fecha)
        {
            var result = await _service.ListAsync(Hor_Id, Fecha);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> MarcarAsync(int? Asi_Id, int Hor_Id, int Alu_Id,
            string Asi_Fecha, string Asi_Estado, string Asi_Observacion)
        {
            bool err;
            if (Asi_Id == null || Asi_Id == 0)
                err = await _service.CreateAsync(new { Hor_Id, Alu_Id, Asi_Fecha, Asi_Estado, Asi_Observacion, UsuarioRegistra = GetUsuarioId() });
            else
                err = await _service.EditAsync(new { Asi_Id, Asi_Estado, Asi_Observacion, UsuarioModifica = GetUsuarioId() });

            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
