using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Directorio de Alumnos")]
    public class AlumnosDirectorioController : BaseController
    {
        private readonly AlumnosDirectorioService _service = new AlumnosDirectorioService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(string Cur_Id = null, string Sec_Id = null)
        {
            string qs = "";
            if (!string.IsNullOrEmpty(Cur_Id)) qs += "&Cur_Id=" + Cur_Id;
            if (!string.IsNullOrEmpty(Sec_Id)) qs += "&Sec_Id=" + Sec_Id;
            string apiUrl = "AlumnosDirectorio/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            var raw = await SendHttpClient.GetRawAsync(apiUrl);
            return Content("{\"data\":" + raw + "}", "application/json");
        }

        [HttpGet]
        public async Task<ActionResult> CursosDropdown()
        {
            var result = await _service.CursosDropdown();
            return Content(JsonConvert.SerializeObject(new { data = result }), "application/json");
        }

        [HttpGet]
        public async Task<ActionResult> SeccionesDropdown(string Cur_Id = null)
        {
            var result = await _service.SeccionesDropdown(Cur_Id);
            return Content(JsonConvert.SerializeObject(new { data = result }), "application/json");
        }
    }
}
