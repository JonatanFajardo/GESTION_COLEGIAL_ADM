using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Lista de años lectivos (solo lectura) para el selector "Año de trabajo" del encabezado, que ve
    /// todo usuario con sesión. Cambiar el año de trabajo sigue siendo de quien tiene la pantalla
    /// "Listado de años lectivos" (AniosLectivosController.SetActivoAsync).
    /// </summary>
    [SessionManager("Home")]
    public class AnioTrabajoController : BaseController
    {
        private readonly AniosLectivosService aniosLectivosService = new AniosLectivosService();

        public async Task<ActionResult> ListAsync()
        {
            var anios = await aniosLectivosService.ListAsync();
            var lista = (anios ?? Enumerable.Empty<GESTION_COLEGIAL.Business.Models.AnioLectivoViewModel>())
                .Select(a => new { a.Ani_Id, a.Ani_Nombre, a.Ani_EsActual })
                .ToList();
            bool puedeCambiar = TienePantalla("Listado de años lectivos")
                || string.Equals(Session[SesionUsuario.RoleName] as string, "admin", StringComparison.OrdinalIgnoreCase);
            return Json(new { data = lista, puedeCambiar });
        }
    }
}
