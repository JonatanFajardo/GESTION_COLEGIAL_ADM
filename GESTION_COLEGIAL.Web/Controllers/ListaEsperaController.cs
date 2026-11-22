using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Lista de espera de admisión (espejo de Skuul "admission waitlist"), en 3 pantallas:
    /// la lista (con el resumen de cupo por grado), el registro de un aspirante y su detalle.
    /// Permisos: "Lista de espera" (ver: secretaría, dirección) y "Gestionar lista de espera"
    /// (registrar, editar, ofrecer, rechazar, desistió, reactivar: secretaría).
    /// Matricular desde la lista pide además "Matrícula nueva" (la que usa el wizard).
    /// </summary>
    [SessionManager("Lista de espera")]
    public class ListaEsperaController : BaseController
    {
        public const string PantallaGestionar = "Gestionar lista de espera";
        public const string PantallaMatricula = "Matrícula nueva";

        private readonly ListaEsperaService _service = new ListaEsperaService();
        private readonly AlumnosService _alumnos = new AlumnosService();
        private readonly ParentescosService _parentescos = new ParentescosService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(string pantalla) => EsAdmin() || TienePantalla(pantalla);
        private bool PuedeGestionar() => Puede(PantallaGestionar);
        private bool PuedeMatricular() => PuedeGestionar() && Puede(PantallaMatricula);

        private static readonly Regex Identidad = new Regex(@"^\d{13}$");
        private static readonly Regex Correo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private static string TextoAviso(string aviso) => aviso switch
        {
            "matriculado" => "El aspirante quedó matriculado.",
            _ => null
        };

        /// <summary>El año pedido si existe; si no, el que la API marca por defecto (activo o próximo abierto).</summary>
        private static int ElegirAnio(List<AspiranteAnioViewModel> anios, int? anio) =>
            anio.HasValue && anios.Any(a => a.Ani_Id == anio.Value) ? anio.Value
            : (anios.FirstOrDefault(a => a.EsPorDefecto) ?? anios.FirstOrDefault())?.Ani_Id ?? 0;

        private async Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> NivelesAsync()
        {
            var m = await _alumnos.Dropdown(new AlumnoViewModel());
            return m.NivelesEducativosList ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        }

        // ───────────────────────────── La lista ─────────────────────────────

        /// <summary>/listaespera?anio=1</summary>
        public async Task<ActionResult> Index(int? anio)
        {
            var anios = await _service.AniosAsync();
            var modelo = new ListaEsperaIndexViewModel
            {
                Anios = anios,
                AniId = ElegirAnio(anios, anio),
                PuedeGestionar = PuedeGestionar(),
                PuedeMatricular = PuedeMatricular(),
                Niveles = await NivelesAsync()
            };
            return View(modelo);
        }

        /// <summary>Aspirantes del año en el orden de la cola (para la tabla).</summary>
        [HttpGet]
        public async Task<ActionResult> ListAsync(int aniId) => AjaxResult(await _service.ListAsync(aniId));

        /// <summary>Cupo y demanda por grado del año (resumen y tarjetas).</summary>
        [HttpGet]
        public async Task<ActionResult> ResumenAsync(int aniId) => AjaxResult(await _service.ResumenAsync(aniId));

        // ───────────────────────────── Cascada Nivel → Grado → Modalidad → Curso ─────────────────────────────
        // Los mismos servicios que usa el wizard de Matrícula; aquí para que Dirección también filtre.

        [HttpGet]
        public async Task<ActionResult> GradosAsync(int id)
        {
            var r = await _alumnos.CursoNivelesDropdown(id);
            return AjaxResult((r ?? Enumerable.Empty<CursoNivelDropViewModel>()).Select(x => new { Value = x.Cun_Id, Text = x.Cun_Descripcion }));
        }

        [HttpGet]
        public async Task<ActionResult> ModalidadesAsync(int id)
        {
            var r = await _alumnos.ModalidadesDropdown(id);
            return AjaxResult((r ?? Enumerable.Empty<ModalidadViewModel>()).Select(x => new { Value = x.Mda_Id, Text = x.Mda_Descripcion }));
        }

        /// <summary>Cursos (carrera / plan) que se imparten en el grado.</summary>
        [HttpGet]
        public async Task<ActionResult> CursosAsync(int id)
        {
            var r = await _service.CursosAsync(id);
            return AjaxResult(r.Select(x => new { Value = x.Cur_Id, Text = x.Cur_Nombre }));
        }

        // ───────────────────────────── Registro de aspirante ─────────────────────────────

        /// <summary>/listaespera/registrar?anio=1 (nuevo) · /listaespera/registrar/5 (editar mientras siga abierto)</summary>
        public async Task<ActionResult> Registrar(int? id, int? anio)
        {
            if (!PuedeGestionar()) return RedirectToAction("SinAcceso", "Account");

            AspiranteViewModel aspirante = null;
            if (id.HasValue && id.Value > 0)
            {
                aspirante = await _service.FindAsync(id.Value);
                if (aspirante == null || aspirante.Asp_Id == 0) return RedirectToAction("Index");
                if (!aspirante.EstaAbierto) return RedirectToAction("Aspirante", new { id = id.Value });
            }

            var anios = (await _service.AniosAsync()).Where(a => a.AdmiteRegistro || a.Ani_Id == aspirante?.Ani_Id).ToList();
            var parentescos = await _parentescos.ListAsync();
            var modelo = new AspiranteRegistroPaginaViewModel
            {
                Aspirante = aspirante,
                Anios = anios,
                AniId = aspirante?.Ani_Id ?? ElegirAnio(anios, anio),
                Niveles = await NivelesAsync(),
                Parentescos = (parentescos ?? Enumerable.Empty<ParentescoViewModel>()).ToList(),
                Hoy = HoraHonduras.Ahora.ToString("yyyy-MM-dd")
            };
            return View(modelo);
        }

        /// <summary>Revisión en vivo del formulario: duplicado abierto en el año e identidad de un alumno.</summary>
        [HttpGet]
        public async Task<ActionResult> DuplicadoAsync(int aniId, string identidad, string nombres, string apellidos,
                                                      DateTime? fechaNacimiento, int? aspId)
        {
            if (!PuedeGestionar()) return Json(new { ok = false });
            var r = await _service.DuplicadoAsync(aniId, identidad, nombres, apellidos, fechaNacimiento, aspId);
            return Json(new
            {
                ok = true,
                duplicado = r?.AspiranteId != null
                    ? $"Ya está en la lista de este año: {r.AspiranteNombre} ({CatalogoAspirantes.Estado(r.AspiranteEstado)}, {r.AspiranteGrado})."
                    : null,
                duplicadoId = r?.AspiranteId,
                alumno = r?.AlumnoId != null
                    ? (r.AlumnoMatriculadoEnAnio
                        ? $"La identidad es del alumno {r.AlumnoNombre}, que ya está matriculado este año."
                        : $"La identidad ya es del alumno {r.AlumnoNombre}: al matricularlo no se podrá crear de nuevo.")
                    : null
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarAsync([FromBody] AspiranteGuardarViewModel model)
        {
            if (!PuedeGestionar()) return Json(new { success = false, message = "No tiene permiso para gestionar la lista de espera." });
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });

            // Mismas reglas que el procedimiento; el campo con problema vuelve para marcarlo en el formulario.
            if (string.IsNullOrWhiteSpace(model.Asp_Nombres))
                return Json(new { success = false, field = "nombres", message = "Escriba los nombres del aspirante." });
            if (string.IsNullOrWhiteSpace(model.Asp_Apellidos))
                return Json(new { success = false, field = "apellidos", message = "Escriba los apellidos del aspirante." });
            if (!string.IsNullOrWhiteSpace(model.Asp_Identidad) && !Identidad.IsMatch(model.Asp_Identidad.Trim()))
                return Json(new { success = false, field = "identidad", message = "La identidad lleva 13 números (o déjela vacía)." });
            if (!model.Asp_FechaNacimiento.HasValue || model.Asp_FechaNacimiento.Value.Date > HoraHonduras.Ahora.Date || model.Asp_FechaNacimiento.Value.Year < 1990)
                return Json(new { success = false, field = "nacimiento", message = "Revise la fecha de nacimiento." });
            if (model.Asp_Sexo != "M" && model.Asp_Sexo != "F")
                return Json(new { success = false, field = "sexo", message = "Elija el sexo del aspirante." });
            if (string.IsNullOrWhiteSpace(model.Asp_EncargadoNombre))
                return Json(new { success = false, field = "encargado", message = "Escriba el nombre del encargado." });
            if (!string.IsNullOrWhiteSpace(model.Asp_EncargadoIdentidad) && !Identidad.IsMatch(model.Asp_EncargadoIdentidad.Trim()))
                return Json(new { success = false, field = "encidentidad", message = "La identidad del encargado lleva 13 números (o déjela vacía)." });
            if (!model.Par_Id.HasValue || model.Par_Id.Value <= 0)
                return Json(new { success = false, field = "parentesco", message = "Elija el parentesco." });
            if (Regex.Replace(model.Asp_EncargadoTelefono ?? "", @"[^\d]", "").Length < 8)
                return Json(new { success = false, field = "telefono", message = "Escriba un teléfono de al menos 8 números." });
            if (!string.IsNullOrWhiteSpace(model.Asp_EncargadoCorreo) && !Correo.IsMatch(model.Asp_EncargadoCorreo.Trim()))
                return Json(new { success = false, field = "correo", message = "Revise el correo." });
            if (model.Ani_Id <= 0)
                return Json(new { success = false, field = "anio", message = "Elija el año lectivo." });
            if (model.Niv_Id <= 0)
                return Json(new { success = false, field = "nivel", message = "Elija el nivel." });
            if (model.Cun_Id <= 0)
                return Json(new { success = false, field = "grado", message = "Elija el grado." });
            if (model.Asp_Prioridad > 0 && string.IsNullOrWhiteSpace(model.Asp_MotivoPrioridad))
                return Json(new { success = false, field = "motivoprioridad", message = "Diga por qué tiene prioridad." });

            model.Usuario = GetUsuarioId();
            AspiranteResultadoViewModel r = await _service.GuardarAsync(model);
            if (r == null || !r.Ok || r.Id == 0)
                return Json(new { success = false, message = r?.Mensaje ?? "No se pudo guardar el aspirante." });

            return Json(new
            {
                success = true,
                id = r.Id,
                nuevo = model.Asp_Id == 0,
                estado = r.Estado,
                posicionCola = r.PosicionCola,
                enCola = r.EnCola,
                grado = r.Grado,
                aviso = r.Aviso,
                url = Url.Action("Aspirante", new { id = r.Id })
            });
        }

        // ───────────────────────────── El aspirante ─────────────────────────────

        /// <summary>/listaespera/aspirante/5</summary>
        public async Task<ActionResult> Aspirante(int id, string aviso = null)
        {
            AspiranteViewModel a = await _service.FindAsync(id);
            if (a == null || a.Asp_Id == 0) return RedirectToAction("Index");
            var modelo = new AspirantePaginaViewModel
            {
                Aspirante = a,
                Historial = await _service.HistorialAsync(id),
                PuedeGestionar = PuedeGestionar(),
                PuedeMatricular = PuedeMatricular(),
                Aviso = TextoAviso(aviso)
            };
            return View(modelo);
        }

        /// <summary>ofrecer | rechazar | desistio | reactivar. Rechazar, desistió y reactivar piden motivo.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstadoAsync([FromBody] AspiranteEstadoViewModel model)
        {
            if (!PuedeGestionar()) return Json(new { success = false, message = "No tiene permiso para gestionar la lista de espera." });
            if (model == null || model.Asp_Id <= 0) return Json(new { success = false, message = "No llegaron datos." });
            if (!new[] { "ofrecer", "rechazar", "desistio", "reactivar" }.Contains(model.Accion))
                return Json(new { success = false, message = "Elija qué hacer con el aspirante." });
            if (model.Accion != "ofrecer" && string.IsNullOrWhiteSpace(model.Motivo))
                return Json(new { success = false, field = "motivo", message = "Escriba el motivo." });
            if ((model.Motivo ?? "").Trim().Length > 500)
                return Json(new { success = false, field = "motivo", message = "El motivo admite hasta 500 caracteres." });

            model.Usuario = GetUsuarioId();
            AspiranteResultadoViewModel r = await _service.CambiarEstadoAsync(model);
            if (r == null || !r.Ok) return Json(new { success = false, message = r?.Mensaje ?? "No se pudo cambiar el estado del aspirante." });
            return Json(new
            {
                success = true,
                estado = r.Estado,
                aviso = r.Aviso,
                message = model.Accion switch
                {
                    "ofrecer" => "Se ofreció el cupo.",
                    "rechazar" => "El aspirante quedó como rechazado.",
                    "desistio" => "El aspirante quedó como que desistió.",
                    _ => "El aspirante volvió a la lista, al final de la cola."
                }
            });
        }
    }
}
