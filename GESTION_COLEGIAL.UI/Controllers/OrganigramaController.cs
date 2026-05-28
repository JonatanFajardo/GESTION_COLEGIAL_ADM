using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Organigrama")]
    public class OrganigramaController : BaseController
    {
        private readonly OrganigramaService _service = new OrganigramaService();

        public ActionResult Index() => View();

        public async Task<ActionResult> GetAsync()
        {
            var result = await _service.GetAsync();
            return Content(JsonConvert.SerializeObject(new { data = result }), "application/json");
        }
    }
}
