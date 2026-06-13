using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Control de vacaciones")]
    public class VacacionesAdmController : BaseController
    {
        private readonly VacacionesAdmService _service   = new VacacionesAdmService();
        private readonly EmpleadosService     _empleados = new EmpleadosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var empleados = await _empleados.ListAsync();
            return Json(new
            {
                empleados = empleados.Select(e => new {
                    id    = e.Emp_Id,
                    texto = e.Emp_Nombre
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ListAsync(string Emp_Id = null)
        {
            var result = await _service.ListAsync(Emp_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int Emp_Id, string Vac_FechaInicio, string Vac_FechaFin, string Vac_Tipo, string Vac_Motivo)
        {
            bool err = await _service.CreateAsync(new { Emp_Id, Vac_FechaInicio, Vac_FechaFin, Vac_Tipo, Vac_Motivo, UsuarioRegistra = GetUsuarioId() });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        [HttpPost]
        public async Task<ActionResult> ApproveAsync(int Vac_Id, string Vac_Estado)
        {
            bool err = await _service.ApproveAsync(new { Vac_Id, Vac_Estado, UsuarioModifica = GetUsuarioId() });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
