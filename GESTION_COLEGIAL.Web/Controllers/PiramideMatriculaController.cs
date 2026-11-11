using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pirámide de Matrícula")]
    public class PiramideMatriculaController : BaseController
    {
        private readonly PiramideMatriculaService _service = new PiramideMatriculaService();

        public ActionResult Index()
        {
            ViewBag.AnioActual = DateTime.Now.Year;
            return View();
        }

        public async Task<ActionResult> ListAsync(string Anio = null)
        {
            var result = await _service.ListAsync(Anio);
            return Content(JsonConvert.SerializeObject(new { data = result }), "application/json");
        }
    }
}
