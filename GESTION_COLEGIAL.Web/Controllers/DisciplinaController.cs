using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Disciplina (espejo de Skuul "Cases"), en 3 pantallas: Casos (listado), Registrar un caso y el caso.
    /// Permisos: "Casos de disciplina" (ver), "Registrar caso de disciplina", "Atender casos de disciplina"
    /// (mover de estado, acciones, notas) y "Ver casos reservados" (protección del menor y notas privadas).
    /// Un caso reservado también lo leen quien lo reportó y quien lo atiende.
    /// </summary>
    [SessionManager("Casos de disciplina")]
    public class DisciplinaController : BaseController
    {
        private const string PantallaRegistrar = "Registrar caso de disciplina";
        private const string PantallaAtender = "Atender casos de disciplina";
        private const string PantallaReservados = "Ver casos reservados";
        private readonly CasosDisciplinaService _service = new CasosDisciplinaService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(string pantalla) => EsAdmin() || TienePantalla(pantalla);
        private bool VerReservados() => Puede(PantallaReservados);

        private int? SesionEntero(string clave) =>
            int.TryParse(Session[clave]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        private int? UsuId() => SesionEntero(SesionUsuario.UserId);
        private int? EmpId() => SesionEntero(SesionUsuario.EmpId);

        private static string TextoAviso(string aviso) => aviso switch
        {
            "registrado" => "Se registró el caso.",
            _ => null
        };

        // ───────────────────────────── Casos ─────────────────────────────

        /// <summary>/disciplina?estado=reportado&amp;tipo=conducta&amp;abiertos=true&amp;pagina=2</summary>
        public async Task<ActionResult> Index(string estado, string tipo, bool abiertos = false, int pagina = 1)
        {
            if (!CatalogoCasos.ExisteEstado(estado)) estado = null;
            if (!CatalogoCasos.ExisteTipo(tipo)) tipo = null;
            if (pagina < 1) pagina = 1;

            bool reservados = VerReservados();
            var casos = await _service.ListAsync(estado, tipo, abiertos, pagina, reservados, UsuId(), EmpId());
            var modelo = new CasosIndexViewModel
            {
                Estado = estado,
                Tipo = tipo,
                SoloAbiertos = abiertos,
                Pagina = pagina,
                Casos = casos,
                Total = casos.FirstOrDefault()?.TotalFilas ?? 0,
                Abiertos = await _service.AbiertosAsync(reservados, UsuId(), EmpId()),
                PuedeRegistrar = Puede(PantallaRegistrar)
            };
            // Una página más allá del final: volver a la última que tiene casos.
            if (casos.Count == 0 && pagina > 1) return RedirectToAction("Index", new { estado, tipo, abiertos, pagina = 1 });
            return View(modelo);
        }

        // ───────────────────────────── Registrar un caso ─────────────────────────────

        public async Task<ActionResult> Registrar()
        {
            if (!Puede(PantallaRegistrar)) return RedirectToAction("SinAcceso", "Account");
            var modelo = new CasoRegistrarPaginaViewModel
            {
                Ahora = HoraHonduras.Ahora,
                Alumnos = await _service.AlumnosAsync(),
                Empleados = await _service.EmpleadosAsync()
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarAsync([FromBody] CasoRegistrarViewModel model)
        {
            if (!Puede(PantallaRegistrar)) return Json(new { success = false, message = "No tiene permiso para registrar casos." });
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });

            // Mismas reglas que Skuul; el campo con problema vuelve para marcarlo en el formulario.
            if (string.IsNullOrWhiteSpace(model.Cas_Resumen))
                return Json(new { success = false, field = "resumen", message = "Escriba un resumen del caso." });
            if (model.Cas_Resumen.Trim().Length > 255)
                return Json(new { success = false, field = "resumen", message = "El resumen admite hasta 255 caracteres." });
            if (!CatalogoCasos.ExisteTipo(model.Cas_Tipo))
                return Json(new { success = false, field = "tipo", message = "Elija el tipo de caso." });
            if (model.Cas_FechaOcurrio == default)
                return Json(new { success = false, field = "fecha", message = "Indique cuándo ocurrió." });
            if (model.Cas_FechaOcurrio > HoraHonduras.Ahora.AddMinutes(1))
                return Json(new { success = false, field = "fecha", message = "No se puede registrar un caso con una fecha que todavía no ha pasado." });
            if ((model.Personas?.Count(p => p.Alu_Id.HasValue) ?? 0) > 20)
                return Json(new { success = false, message = "Un caso admite hasta 20 personas." });

            model.Personas = (model.Personas ?? new()).Where(p => p.Alu_Id.HasValue).ToList();
            model.Usuario = GetUsuarioId();

            CasoResultadoViewModel r = await _service.RegistrarAsync(model);
            if (r == null || !r.Ok || r.Id == 0)
                return Json(new { success = false, message = r?.Mensaje ?? "No se pudo registrar el caso." });

            return Json(new
            {
                success = true,
                id = r.Id,
                message = $"Se registró el caso {r.Referencia}.",
                redirect = Url.Action("Caso", new { id = r.Id, aviso = "registrado" })
            });
        }

        // ───────────────────────────── El caso ─────────────────────────────

        /// <summary>/disciplina/caso/5</summary>
        public async Task<ActionResult> Caso(int id, string aviso = null)
        {
            bool reservados = VerReservados();
            CasoDisciplinaViewModel caso = await _service.FindAsync(id, reservados, UsuId(), EmpId());
            if (caso == null || caso.Cas_Id == 0) return RedirectToAction("Index");

            bool puedeAtender = Puede(PantallaAtender);
            var modelo = new CasoPaginaViewModel
            {
                Caso = caso,
                PuedeAtender = puedeAtender,
                Aviso = TextoAviso(aviso),
                Personas = await _service.PersonasAsync(id),
                Acciones = await _service.AccionesAsync(id),
                Notas = await _service.NotasAsync(id, reservados, UsuId(), EmpId()),
                Historial = await _service.EstadosAsync(id),
                Siguientes = CatalogoCasos.SiguientesDe(caso.Cas_Estado)
            };
            if (puedeAtender) modelo.Empleados = await _service.EmpleadosAsync();
            return View(modelo);
        }

        /// <summary>Solo quien atiende casos y puede leer este caso.</summary>
        private async Task<CasoDisciplinaViewModel> CasoAtendible(int casId)
        {
            if (!Puede(PantallaAtender)) return null;
            CasoDisciplinaViewModel caso = await _service.FindAsync(casId, VerReservados(), UsuId(), EmpId());
            return caso == null || caso.Cas_Id == 0 ? null : caso;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstadoAsync([FromBody] CasoCambiarEstadoViewModel model)
        {
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });
            CasoDisciplinaViewModel caso = await CasoAtendible(model.Cas_Id);
            if (caso == null) return Json(new { success = false, message = "No tiene permiso para atender este caso." });
            if (!CatalogoCasos.SiguientesDe(caso.Cas_Estado).Any(x => x.Codigo == model.Estado))
                return Json(new { success = false, field = "estado", message = $"Un caso no puede pasar de {CatalogoCasos.Estado(caso.Cas_Estado)} a {CatalogoCasos.Estado(model.Estado)}." });
            if ((model.Motivo ?? "").Trim().Length > 500)
                return Json(new { success = false, field = "motivo", message = "El motivo admite hasta 500 caracteres." });

            model.Usuario = GetUsuarioId();
            CasoResultadoViewModel r = await _service.CambiarEstadoAsync(model);
            if (r == null || !r.Ok) return Json(new { success = false, field = "estado", message = r?.Mensaje ?? "No se pudo mover el caso." });
            return Json(new { success = true, estado = r.Estado });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarAccionAsync([FromBody] CasoAgregarAccionViewModel model)
        {
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });
            CasoDisciplinaViewModel caso = await CasoAtendible(model.Cas_Id);
            if (caso == null) return Json(new { success = false, message = "No tiene permiso para atender este caso." });
            if (!CatalogoCasos.EstaAbierto(caso.Cas_Estado))
                return Json(new { success = false, field = "tipo", message = "Este caso ya terminó. Reábralo antes de agregar una acción." });
            if (string.IsNullOrWhiteSpace(model.Cac_Tipo))
                return Json(new { success = false, field = "tipo", message = "Diga qué tipo de paso es." });
            if (string.IsNullOrWhiteSpace(model.Cac_Descripcion))
                return Json(new { success = false, field = "descripcion", message = "Diga qué tiene que pasar." });

            model.Usuario = GetUsuarioId();
            CasoResultadoViewModel r = await _service.AgregarAccionAsync(model);
            if (r == null || !r.Ok) return Json(new { success = false, field = "tipo", message = r?.Mensaje ?? "No se pudo agregar la acción." });
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompletarAccionAsync(int casId, int cacId)
        {
            CasoDisciplinaViewModel caso = await CasoAtendible(casId);
            if (caso == null) return Json(new { success = false, message = "No tiene permiso para atender este caso." });
            if (!CatalogoCasos.EstaAbierto(caso.Cas_Estado))
                return Json(new { success = false, message = "Este caso ya terminó. Reábralo antes de cambiar sus acciones." });

            CasoResultadoViewModel r = await _service.CompletarAccionAsync(casId, cacId, GetUsuarioId());
            if (r == null || !r.Ok) return Json(new { success = false, message = r?.Mensaje ?? "No se pudo marcar la acción como hecha." });
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarNotaAsync([FromBody] CasoAgregarNotaViewModel model)
        {
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });
            CasoDisciplinaViewModel caso = await CasoAtendible(model.Cas_Id);
            if (caso == null) return Json(new { success = false, message = "No tiene permiso para atender este caso." });
            if (!CatalogoCasos.EstaAbierto(caso.Cas_Estado))
                return Json(new { success = false, field = "nota", message = "Este caso ya terminó. Reábralo antes de escribir una nota." });
            if (string.IsNullOrWhiteSpace(model.Cno_Texto))
                return Json(new { success = false, field = "nota", message = "Escriba la nota." });

            model.Usuario = GetUsuarioId();
            CasoResultadoViewModel r = await _service.AgregarNotaAsync(model);
            if (r == null || !r.Ok) return Json(new { success = false, field = "nota", message = r?.Mensaje ?? "No se pudo agregar la nota." });
            return Json(new { success = true });
        }
    }
}
