using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("KPIs Académicos")]
    public class KPIsAcademicosController : BaseController
    {
        private readonly KPIsAcademicosService      _service     = new KPIsAcademicosService();
        private readonly RendimientoPorSeccionService _rendimiento = new RendimientoPorSeccionService();
        private readonly AlumnosEnRiesgoService     _enRiesgo    = new AlumnosEnRiesgoService();
        private readonly PiramideMatriculaService   _piramide    = new PiramideMatriculaService();
        private readonly ParcialesService           _parciales   = new ParcialesService();

        public ActionResult Index() => View();

        public async Task<ActionResult> FindAsync(int? Anio)
        {
            int anio = Anio ?? DateTime.Now.Year;
            var result = await _service.FindAsync(anio);
            return AjaxResult(result);
        }

        public async Task<ActionResult> DropdownsAsync()
        {
            var parciales = await _parciales.ListAsync();
            return Json(new
            {
                parciales = parciales?.Select(p => new { id = p.Pac_Id, texto = p.Pac_Descripcion })
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RendimientoAsync(int? Anio, int? Par_Id)
        {
            int anio  = Anio   ?? DateTime.Now.Year;
            int parId = Par_Id ?? 1;
            var result = await _rendimiento.ListAsync(anio, parId);
            return AjaxResult(result);
        }

        public async Task<ActionResult> EnRiesgoAsync(int? Anio)
        {
            string anio = (Anio ?? DateTime.Now.Year).ToString();
            var result = await _enRiesgo.ListAsync(null, anio);
            return AjaxResult(result);
        }

        public async Task<ActionResult> PiramideAsync(int? Anio)
        {
            string anio = (Anio ?? DateTime.Now.Year).ToString();
            var result = await _piramide.ListAsync(anio);
            return AjaxResult(result);
        }
    }
}
