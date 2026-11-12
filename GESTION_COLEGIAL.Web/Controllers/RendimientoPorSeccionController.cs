using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Rendimiento por Sección")]
    public class RendimientoPorSeccionController : BaseController
    {
        private readonly RendimientoPorSeccionService _service   = new RendimientoPorSeccionService();
        private readonly ParcialesService             _parciales = new ParcialesService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var parciales = await _parciales.ListAsync();
            return Json(new
            {
                parciales = parciales.Select(p => new { id = p.Pac_Id, texto = p.Pac_Descripcion })
            });
        }

        public async Task<ActionResult> ListAsync(int? Anio, int? Par_Id)
        {
            int anio  = Anio   ?? DateTime.Now.Year;
            int parId = Par_Id ?? 1;
            var result = await _service.ListAsync(anio, parId);
            return AjaxResult(result);
        }
    }
}
