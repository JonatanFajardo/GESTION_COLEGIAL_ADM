using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Vínculos familiares: cola de trabajo de Secretaría para ligar encargados con sus hijos
    /// (alumnos activos sin encargado y encargados sin hijos). Usa el mismo permiso que ya protege
    /// el vínculo en la API ("Editar alumnos"); no agrega permisos nuevos.
    /// </summary>
    [SessionManager("Editar alumnos")]
    public class VinculosFamiliaresController : BaseController
    {
        private readonly EncargadosService _encargados = new EncargadosService();
        private readonly AlumnosService _alumnos = new AlumnosService();
        private readonly ParentescosService _parentescos = new ParentescosService();

        public ActionResult Index() => View();

        /// <summary>Alumnos activos que todavía no tienen ningún encargado.</summary>
        [HttpGet]
        public async Task<ActionResult> SinEncargadoAsync()
            => AjaxResult((await _encargados.SinVinculoAsync("alumnos") ?? Enumerable.Empty<VinculoPendienteViewModel>()).ToList());

        /// <summary>Encargados registrados que todavía no tienen ningún hijo ligado.</summary>
        [HttpGet]
        public async Task<ActionResult> SinHijosAsync()
            => AjaxResult((await _encargados.SinVinculoAsync("encargados") ?? Enumerable.Empty<VinculoPendienteViewModel>()).ToList());

        /// <summary>Totales para los indicadores y listas para los buscadores del cuadro de ligar.</summary>
        [HttpGet]
        public async Task<ActionResult> OpcionesAsync()
        {
            var alumnos = (await _alumnos.ListAsync() ?? Enumerable.Empty<AlumnoViewModel>())
                .Select(a => new { Value = a.Alu_Id, Text = a.Alu_Nombre, Identidad = a.Per_Identidad }).ToList();
            var encargados = (await _encargados.ListAsync() ?? Enumerable.Empty<EncargadoViewModel>())
                .Select(e => new { Value = e.Enc_Id, Text = e.Enc_Nombre, Identidad = e.Per_Identidad }).ToList();
            var parentescos = (await _parentescos.ListAsync() ?? Enumerable.Empty<ParentescoViewModel>())
                .Select(p => new { Value = p.Par_Id, Text = p.Par_Descripcion }).ToList();
            return AjaxResult(new { alumnos, encargados, parentescos });
        }

        [HttpPost]
        public async Task<ActionResult> VincularAsync(int aluId, int encId, int parId)
        {
            if (aluId <= 0 || encId <= 0 || parId <= 0)
                return Json(new { success = false, message = "Elija el alumno, el encargado y el parentesco." });
            var r = await _encargados.VincularAsync(encId, aluId, parId);
            return Json(new { success = !r.Error, message = r.Error ? r.Mensaje : "Vínculo guardado." });
        }
    }
}
