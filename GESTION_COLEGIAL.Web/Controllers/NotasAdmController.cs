using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Cuaderno de notas")]
    public class NotasAdmController : BaseController
    {
        private readonly NotasAdmService  _service   = new NotasAdmService();
        private readonly HorariosService  _horarios  = new HorariosService();
        private readonly ParcialesService _parciales = new ParcialesService();
        private readonly UsuariosService  _usuarios  = new UsuariosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var horariosTask  = _horarios.ListAsync();
            var parcialesTask = _parciales.ListAsync();
            // La sesión solo guarda Usu_Id/Rol/Token (ver AccountController.Login), no el Emp_Id
            // del docente. Se resuelve aquí vía Usuarios/FindAsync para poder filtrar sin tocar
            // AccountController/BaseController (fuera del alcance de este cambio).
            var usuarioTask   = _usuarios.Find(GetUsuarioId());

            await Task.WhenAll(horariosTask, parcialesTask, usuarioTask);

            var horarios  = horariosTask.Result;
            var parciales = parcialesTask.Result;
            var usuario   = usuarioTask.Result;

            // Si el usuario logueado tiene un Empleado (docente) asociado, mostrar solo sus horarios.
            // Si no tiene Emp_Id (p.ej. un admin sin vínculo a empleado), se deja el listado completo
            // para no romper el acceso de otros roles que también usan esta pantalla.
            if (usuario != null && usuario.Emp_Id > 0)
                horarios = horarios.Where(h => h.Emp_Id == usuario.Emp_Id);

            return Json(new
            {
                horarios  = horarios.Select(h  => new { id = h.Hor_Id, texto = h.Dia_Descripcion + " " + h.Hor_HoraInicioDescripcion + " — " + h.Mat_Nombre + " (" + h.Sec_Descripcion + ")" }),
                parciales = parciales.Select(p => new { id = p.Pac_Id, texto = p.Pac_Descripcion }),
            });
        }

        // GET: NotasAdm/ListAsync?Hor_Id=&Par_Id=
        public async Task<ActionResult> ListAsync(int Hor_Id, int Par_Id)
        {
            var result = await _service.ListAsync(Hor_Id, Par_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarNota(int? Not_Id, int Alu_Id, int Sec_Id,
            int Mat_Id, int Sem_Id, int Par_Id, decimal Not_Nota, string Not_Año)
        {
            bool err;
            if (Not_Id == null || Not_Id == 0)
                err = await _service.CreateAsync(new { Alu_Id, Sec_Id, Mat_Id, Sem_Id, Par_Id, Not_Nota, Not_Año, UsuarioRegistra = GetUsuarioId() });
            else
                err = await _service.EditAsync(new { Not_Id, Not_Nota, UsuarioModifica = GetUsuarioId() });

            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
