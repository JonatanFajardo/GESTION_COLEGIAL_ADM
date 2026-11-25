using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Compat;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Dashboard Docente")]
    public class DashboardDocenteController : BaseController
    {
        public ActionResult Index() => View();

        // Passthrough crudo del API (PascalCase). El empleado es el del usuario firmado (y la API lo
        // vuelve a poner por su cuenta); el día de la semana y el año son los de hoy en Honduras.
        public async Task<ActionResult> ResumenAsync()
        {
            DateTime hoy = HoraHonduras.Hoy;
            int diaId = hoy.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)hoy.DayOfWeek;   // 1 = lunes … 7 = domingo
            int empId = int.TryParse(Session[SesionUsuario.EmpId]?.ToString(), out int e) && e > 0 ? e : 0;
            var raw = await SendHttpClient.GetRawAsync($"DashboardDocente/ResumenAsync?Emp_Id={empId}&Dia_Id={diaId}&Anio={hoy.Year}");
            return Content(raw, "application/json");
        }
    }
}
