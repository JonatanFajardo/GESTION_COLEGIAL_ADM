using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Prototipos: pantallas de muestra para revisar una propuesta antes de construirla.
    /// No guardan nada ni cambian la API; leen los datos que ya existen.
    /// "Prototipos" no está en seguridad.tbPantallas, así que solo entra el administrador.
    /// </summary>
    [SessionManager("Prototipos")]
    public class PrototiposController : BaseController
    {
        private readonly PagosService pagosService = new PagosService();
        private readonly AlumnosService alumnosService = new AlumnosService();

        public ActionResult Index() => View();

        public ActionResult CuentasPorCobrar() => View();

        public ActionResult MovimientosCaja() => View();

        public ActionResult CuentaAlumno(int alumnoId = 25)
        {
            ViewBag.AlumnoId = alumnoId;
            return View();
        }

        /// <summary>
        /// Pagos entre dos fechas. El endpoint de rango de la API hoy responde vacío,
        /// así que el prototipo filtra aquí la lista completa.
        /// </summary>
        public async Task<ActionResult> PagosRangoAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var d = (desde ?? DateTime.Today).Date;
            var h = (hasta ?? DateTime.Today).Date.AddDays(1);
            var result = (await pagosService.ListAsync() ?? Enumerable.Empty<Business.Models.PagoListViewModel>())
                .Where(p => p.FechaPago >= d && p.FechaPago < h)
                .OrderByDescending(p => p.FechaPago)
                .ToList();
            return AjaxResult(result);
        }

        /// <summary>Alumnos (id, nombre, nivel, sección) para enlazar las cuentas con su alumno.</summary>
        public async Task<ActionResult> AlumnosAsync()
        {
            var result = (await alumnosService.ListAsync())
                .Select(a => new { a.Alu_Id, a.Alu_Nombre, a.Niv_Descripcion, Grado = (a.Cno_Descripcion ?? "").Trim(), a.Sec_Descripcion, a.Per_Identidad })
                .ToList();
            return Json(result);
        }
    }
}
