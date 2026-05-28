using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Directorio de Alumnos")]
    public class AlumnosDirectorioController : BaseController
    {
        private readonly AlumnosDirectorioService _service = new AlumnosDirectorioService();
        private const string ApiBase = "http://localhost:5076/api/v1/";

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync(string Cur_Id = null, string Sec_Id = null)
        {
            string qs = "";
            if (!string.IsNullOrEmpty(Cur_Id)) qs += "&Cur_Id=" + Cur_Id;
            if (!string.IsNullOrEmpty(Sec_Id)) qs += "&Sec_Id=" + Sec_Id;
            string apiUrl = ApiBase + "AlumnosDirectorio/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);
                var raw = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "[]";
                return Content("{\"data\":" + raw + "}", "application/json");
            }
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
