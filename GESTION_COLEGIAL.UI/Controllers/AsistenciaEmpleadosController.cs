using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Asistencia Empleados")]
    public class AsistenciaEmpleadosController : BaseController
    {
        private readonly AsistenciaEmpleadosService _service   = new AsistenciaEmpleadosService();
        private readonly EmpleadosService           _empleados = new EmpleadosService();

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

        public async Task<ActionResult> ListAsync(string Emp_Id = null, string Fecha = null)
        {
            string json = await _service.ListRawAsync(Emp_Id, Fecha);
            return Content(json, "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int Emp_Id, string Fecha, string Tipo)
        {
            bool err = await _service.CreateAsync(new
            {
                Emp_Id,
                Fecha,
                Tipo,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
