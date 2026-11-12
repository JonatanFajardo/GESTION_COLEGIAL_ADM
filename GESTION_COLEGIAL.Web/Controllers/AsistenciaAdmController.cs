using GESTION_COLEGIAL.Business.Services;
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
        private readonly HorariosService      _horarios = new HorariosService();
        private readonly UsuariosService      _usuarios = new UsuariosService();
        private readonly CalendarioEscolarService _calendario = new CalendarioEscolarService();

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

        public async Task<ActionResult> DropdownsAsync()
        {
            var horariosTask = _horarios.ListAsync();
            // La sesión solo guarda Usu_Id/Rol/Token (ver AccountController.Login), no el Emp_Id
            // del docente. Se resuelve aquí vía Usuarios/FindAsync para poder filtrar sin tocar
            // AccountController/BaseController (fuera del alcance de este cambio).
            var usuarioTask  = _usuarios.Find(GetUsuarioId());

            await Task.WhenAll(horariosTask, usuarioTask);

            var horarios = horariosTask.Result;
            var usuario  = usuarioTask.Result;

            // Si el usuario logueado tiene un Empleado (docente) asociado, mostrar solo sus horarios.
            // Si no tiene Emp_Id (p.ej. un admin sin vínculo a empleado), se deja el listado completo
            // para no romper el acceso de otros roles (Director también usa Pase de Lista).
            if (usuario != null && usuario.Emp_Id > 0)
                horarios = horarios.Where(h => h.Emp_Id == usuario.Emp_Id);

            return Json(new
            {
                horarios = horarios.Select(h => new {
                    id    = h.Hor_Id,
                    texto = h.Dia_Descripcion + " " + h.Hor_HoraInicioDescripcion + " — " + h.Mat_Nombre + " (" + h.Sec_Descripcion + ")"
                })
            });
        }

        public async Task<ActionResult> ListAsync(int Hor_Id, string Fecha)
        {
            var result = await _service.ListAsync(Hor_Id, Fecha);
            return AjaxResult(result);
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
