using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Horario semanal")]
    [BloquearRol("docente")] // Armar/editar horarios es tarea administrativa; el docente solo consulta (VisorHorarios).
    public class HorariosNuevoController : BaseController
    {
        private readonly HorariosService _service = new HorariosService();

        public ActionResult Index()
        {
            ViewBag.PuedeCrear    = TienePantalla("Crear horario");
            ViewBag.PuedeEditar   = TienePantalla("Editar horario");
            ViewBag.PuedeEliminar = TienePantalla("Eliminar horario");
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await _service.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        [SessionManager("Crear horario")]
        public async Task<ActionResult> CreateAsync(HorarioViewModel model)
        {
            model.Hor_UsuarioRegistra = GetUsuarioId();
            if (model.Hor_Id == 0)
            {
                bool err = await _service.Create(model);
                return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                           : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            model.Hor_UsuarioModifica = GetUsuarioId();
            bool err2 = await _service.Edit(model);
            return err2 ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                        : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }

        [HttpPost]
        [SessionManager("Eliminar horario")]
        public async Task<ActionResult> DeleteAsync(HorarioRemoveViewModel model)
        {
            bool err = await _service.Delete(model.Hor_Id);
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
