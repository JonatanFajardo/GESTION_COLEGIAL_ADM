using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Cumpleaños Empleados")]
    public class EmpleadosCumpleanosController : BaseController
    {
        private readonly EmpleadosCumpleanosService _service = new EmpleadosCumpleanosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(int? Mes = null)
        {
            int mes = Mes ?? DateTime.Now.Month;
            var result = await _service.ListAsync(mes);
            return AjaxResult(result);
        }
    }
}
