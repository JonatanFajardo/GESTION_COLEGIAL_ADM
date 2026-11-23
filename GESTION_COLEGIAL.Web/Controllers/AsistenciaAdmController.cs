using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pase de lista")]
    public class AsistenciaAdmController : BaseController
    {
        private readonly AsistenciaAdmService _service  = new AsistenciaAdmService();
        private readonly CalendarioEscolarService _calendario = new CalendarioEscolarService();
        private readonly FichasSaludService _salud = new FichasSaludService();

        private static DateTime? Fecha(string fecha) =>
            DateTime.TryParseExact((fecha ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)
                ? d : (DateTime?)null;

        /// <summary>
        /// ¿El colegio cierra ese día para la sección de este horario? (Feriado o Cierre publicado en el
        /// Calendario escolar.) Ese día no tiene pase de lista.
        /// </summary>
        public async Task<ActionResult> CierreAsync(int Hor_Id, string Fecha)
        {
            DateTime? dia = AsistenciaAdmController.Fecha(Fecha);
            if (dia == null) return Json(new { cerrado = false });
            var c = await _calendario.CierreEnFechaAsync(dia.Value, null, Hor_Id);
            return Json(new
            {
                cerrado = c.Cerrado,
                id = c.Eve_Id,
                titulo = c.Eve_Titulo,
                mensaje = c.Cerrado ? MensajeCierre(dia.Value, c.Eve_Titulo) : null
            });
        }

        private static string MensajeCierre(DateTime dia, string titulo) =>
            "El colegio cierra el " + dia.ToString("dddd d 'de' MMMM", new CultureInfo("es-HN")) + " por " + titulo + ". Ese día no tiene pase de lista.";

        public ActionResult Index() => View();

        /// <summary>Dirección (admin o director) ve y filtra todos los horarios; el docente solo los suyos y los que cubre.</summary>
        private bool EsDireccion()
        {
            string rol = Session[SesionUsuario.RoleName] as string ?? "";
            return rol.Equals("admin", StringComparison.OrdinalIgnoreCase) || rol.Equals("director", StringComparison.OrdinalIgnoreCase);
        }

        private const string SinAcceso = "No puede pasar lista de este horario en esa fecha.";

        private Task<bool> PuedeAsync(int Hor_Id, DateTime dia) =>
            _service.PuedeAsync(GetUsuarioId(), Hor_Id, dia.ToString("yyyy-MM-dd"));

        /// <summary>
        /// Horarios del selector para la fecha. Docente: los suyos y las clases que cubre ese día (marcadas
        /// como sustitución). Dirección: ninguno hasta elegir la sección (Nivel → Grado → Sección), porque son miles.
        /// </summary>
        public async Task<ActionResult> DropdownsAsync(string Fecha, int Niv_Id = 0, int Cun_Id = 0, int Sec_Id = 0)
        {
            DateTime dia = AsistenciaAdmController.Fecha(Fecha) ?? DateTime.Today;
            bool direccion = EsDireccion();
            if (direccion && Sec_Id <= 0)
                return Json(new { direccion, horarios = Array.Empty<object>() });

            var lista = await _service.HorariosAsync(GetUsuarioId(), dia.ToString("yyyy-MM-dd"),
                direccion ? Niv_Id : 0, direccion ? Cun_Id : 0, direccion ? Sec_Id : 0);

            return Json(new
            {
                direccion,
                horarios = lista.Select(h => new {
                    id    = h.Hor_Id,
                    texto = (h.EsSustitucion ? "Sustitución de " + h.Titular + " · " : "")
                            + h.Dia + " " + h.Hora + " — " + h.Materia + " (" + h.Grado + " " + h.Seccion + ")"
                })
            });
        }

        /// <summary>
        /// Opciones del siguiente nivel de la cascada Nivel → Grado → Sección, pedidas ya filtradas por el padre.
        /// Solo dirección. Sin padre: niveles; con Niv_Id: grados; con Niv_Id y Cun_Id: secciones.
        /// </summary>
        public async Task<ActionResult> FiltrosAsync(string Fecha, int Niv_Id = 0, int Cun_Id = 0)
        {
            if (!EsDireccion()) return Json(Array.Empty<object>());
            DateTime dia = AsistenciaAdmController.Fecha(Fecha) ?? DateTime.Today;
            var lista = await _service.HorariosAsync(GetUsuarioId(), dia.ToString("yyyy-MM-dd"), Niv_Id, Cun_Id, 0);

            if (Niv_Id <= 0)
                return Json(lista.Where(h => h.Niv_Id > 0).GroupBy(h => h.Niv_Id).OrderBy(g => g.Key)
                    .Select(g => new { id = g.Key, texto = g.First().Nivel }));
            if (Cun_Id <= 0)
                return Json(lista.Where(h => h.Cun_Id > 0).GroupBy(h => h.Cun_Id).OrderBy(g => g.Key)
                    .Select(g => new { id = g.Key, texto = g.First().Grado }));
            return Json(lista.Where(h => h.Sec_Id > 0).GroupBy(h => h.Sec_Id).OrderBy(g => g.First().Seccion)
                .Select(g => new { id = g.Key, texto = g.First().Seccion }));
        }

        public async Task<ActionResult> ListAsync(int Hor_Id, string Fecha)
        {
            DateTime? dia = AsistenciaAdmController.Fecha(Fecha);
            if (dia == null || !await PuedeAsync(Hor_Id, dia.Value))
                return Json(new { data = Array.Empty<object>(), sinAcceso = true, message = SinAcceso });
            var result = await _service.ListAsync(Hor_Id, Fecha);
            return AjaxResult(result);
        }

        /// <summary>
        /// Alertas críticas de salud de los alumnos de este horario: solo Alu_Id y el texto corto
        /// (Fsa_Alerta). Lo ve quien pasa lista aunque no tenga permiso a las fichas de salud.
        /// </summary>
        public async Task<ActionResult> AlertasSaludAsync(int Hor_Id, string Fecha)
        {
            DateTime? dia = AsistenciaAdmController.Fecha(Fecha);
            if (dia == null || !await PuedeAsync(Hor_Id, dia.Value)) return Json(Array.Empty<object>());
            try
            {
                var alertas = await _salud.AlertasSeccionAsync(Hor_Id, dia.Value.ToString("yyyy-MM-dd"), GetUsuarioId());
                return Json(alertas.Select(a => new { a.Alu_Id, a.Fsa_Alerta }));
            }
            catch
            {
                return Json(Array.Empty<object>());
            }
        }

        [HttpPost]
        public async Task<ActionResult> MarcarAsync(int? Asi_Id, int Hor_Id, int Alu_Id,
            string Asi_Fecha, string Asi_Estado, string Asi_Observacion)
        {
            DateTime? dia = Fecha(Asi_Fecha);
            if (dia == null) return Json(new { success = false, cerrado = false, message = "Fecha no válida." });

            // Estado como letra (la columna es de 1 carácter): P presente, A ausente, T tardanza, J justificado.
            string estado = (Asi_Estado ?? "").Trim().ToUpperInvariant();
            estado = estado.Length > 0 ? estado.Substring(0, 1) : "";
            if (Array.IndexOf(EstadosAsistencia, estado) < 0)
                return Json(new { success = false, cerrado = false, message = "Estado no válido." });

            // Solo quien da la clase (titular), la cubre ese día (suplente) o es dirección puede pasar lista.
            if (!await PuedeAsync(Hor_Id, dia.Value))
                return Json(new { success = false, cerrado = false, message = SinAcceso });

            // Un día en que el colegio cierra (calendario escolar) no tiene pase de lista.
            // La base de datos también lo rechaza (incluso al editar un registro ya guardado).
            var cierre = await _calendario.CierreEnFechaAsync(dia.Value, null, Hor_Id);
            if (cierre.Cerrado)
                return Json(new { success = false, cerrado = true, message = MensajeCierre(dia.Value, cierre.Eve_Titulo) });

            string fecha = dia.Value.ToString("yyyy-MM-dd");
            bool err = (Asi_Id ?? 0) == 0
                ? await _service.CreateAsync(new { Hor_Id, Alu_Id, Asi_Fecha = fecha, Asi_Estado = estado, Asi_Observacion, UsuarioRegistra = GetUsuarioId() })
                : await _service.EditAsync(new { Asi_Id, Asi_Estado = estado, Asi_Observacion, UsuarioModifica = GetUsuarioId() });

            return Json(err
                ? new { success = false, cerrado = false, message = "No se pudo guardar la asistencia." }
                : new { success = true, cerrado = false, message = "Guardado." });
        }

        private static readonly string[] EstadosAsistencia = { "P", "A", "T", "J" };
    }
}
