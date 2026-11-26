using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Piezas compartidas que conectan las pantallas con el año lectivo: cupos de las secciones del año
    /// (ver: quien trabaja con Lista de espera, Matrícula o Año lectivo; editar: "Editar cupos del año"),
    /// años donde se puede inscribir, revisión de cupo al matricular y semestres según los períodos del año.
    /// </summary>
    [SessionManager("Home")]
    public class AnioConexionController : BaseController
    {
        public const string PantallaEditarCupos = "Editar cupos del año";
        private readonly AnioConexionService _service = new AnioConexionService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(string pantalla) => EsAdmin() || TienePantalla(pantalla);

        private bool PuedeVerCupos() =>
            Puede(PantallaEditarCupos) || Puede("Lista de espera") || Puede("Matrícula nueva") || Puede("Listado de años lectivos");

        /// <summary>Secciones del año con cupo e inscritos, y si quien pregunta puede cambiar el cupo.</summary>
        [HttpGet]
        public async Task<ActionResult> CuposAsync(int aniId)
        {
            if (!PuedeVerCupos()) return Json(new { data = new object[0], puedeEditar = false });
            var lista = await _service.CuposAsync(aniId);
            return Json(new { data = lista, puedeEditar = Puede(PantallaEditarCupos) });
        }

        [HttpPost]
        public async Task<ActionResult> ActualizarCupoAsync(int seaId, int cupo)
        {
            if (!Puede(PantallaEditarCupos))
                return Json(new { ok = false, mensaje = "No tiene permiso para cambiar los cupos." });
            var r = await _service.ActualizarCupoAsync(seaId, cupo, GetUsuarioId());
            return Json(new { ok = r.Ok, mensaje = r.Mensaje, inscritos = r.Inscritos, libres = r.Libres, cupo = r.Sea_Cupo });
        }

        [HttpGet]
        public async Task<ActionResult> AniosInscripcionAsync() =>
            Json(new { data = await _service.AniosInscripcionAsync() });

        [HttpGet]
        public async Task<ActionResult> VerificarCupoAsync(int? aniId, int nivId, int cunId, int? mdaId, int secId)
        {
            var r = await _service.VerificarCupoAsync(aniId, nivId, cunId, mdaId, secId);
            if (r == null) return Json(new { permitido = false, mensaje = "No se pudo revisar el cupo. Intente de nuevo." });
            return Json(new
            {
                permitido = r.Permitido, mensaje = r.Mensaje, aniNombre = r.Ani_Nombre, esActivo = r.EsActivo,
                cupo = r.Cupo, inscritos = r.Inscritos, libres = r.Libres
            });
        }

        /// <summary>Semestres del año según sus períodos (vacío si el año no tiene): el temario toma de ahí el inicio de clases.</summary>
        [HttpGet]
        public async Task<ActionResult> SemestresAsync(int anio)
        {
            var lista = await _service.SemestresAsync(anio);
            return Json(new { data = lista });
        }
    }
}
