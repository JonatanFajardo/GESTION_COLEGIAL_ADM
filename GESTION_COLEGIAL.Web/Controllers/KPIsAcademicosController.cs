using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            });
        }

        public async Task<ActionResult> RendimientoAsync(int? Anio, int? Par_Id)
        {
            int anio  = Anio   ?? DateTime.Now.Year;
            int parId = Par_Id ?? 1;
            var result = await _rendimiento.ListAsync(anio, parId);
            return AjaxResult(result);
        }

        /// <summary>
        /// Alumnos con promedio menor a la nota mínima. El mínimo se manda siempre: si va vacío la API
        /// pasa NULL al procedimiento (no usa su valor por defecto) y la lista sale vacía.
        /// </summary>
        public async Task<ActionResult> EnRiesgoAsync(int? Anio, decimal? PromedioMinimo)
        {
            string anio = (Anio ?? DateTime.Now.Year).ToString();
            decimal minimo = PromedioMinimo.HasValue && PromedioMinimo.Value > 0 && PromedioMinimo.Value <= 100 ? PromedioMinimo.Value : 70m;
            var result = await _enRiesgo.ListAsync(minimo.ToString(System.Globalization.CultureInfo.InvariantCulture), anio);
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
