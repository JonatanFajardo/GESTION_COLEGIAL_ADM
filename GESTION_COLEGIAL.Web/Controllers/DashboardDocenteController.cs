using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Docente")]
    public class DashboardDocenteController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API para evitar el bug JObject + JavaScriptSerializer.
        // El API devuelve PascalCase y se pasa tal cual al navegador.
        // El SP requiere Emp_Id, Dia_Id y Anio (no Anio/Mes como otros dashboards).
        public async Task<ActionResult> ResumenAsync(int? Emp_Id, int? Dia_Id, int? Anio)
        {
            var url = "DashboardDocente/ResumenAsync";
            if (Emp_Id.HasValue && Dia_Id.HasValue && Anio.HasValue)
                url += $"?Emp_Id={Emp_Id.Value}&Dia_Id={Dia_Id.Value}&Anio={Anio.Value}";

            var raw = await SendHttpClient.GetRawAsync(url);
            return Content(raw, "application/json");
        }
    }
}
