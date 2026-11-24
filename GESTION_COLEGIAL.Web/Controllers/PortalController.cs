using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Portal de padres (espejo de Skuul "portal"): el encargado ve de cada hijo las notas publicadas,
    /// la asistencia, el estado de cuenta, el calendario y los anuncios, y hace solicitudes al colegio.
    /// Permiso "Portal de padres" (rol Encargado). Qué alumnos puede ver lo decide la API con el usuario
    /// firmado; aquí además se ignora un alumno que no sea hijo de la cuenta.
    /// </summary>
    [SessionManager("Portal de padres")]
    public class PortalController : BaseController
    {
        private readonly PortalPadresService _service = new PortalPadresService();

        private int UsuId() => int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : 0;

        /// <summary>Encargado, hijos y el hijo elegido (el primero si no se pidió uno válido).</summary>
        private async Task<PortalPaginaViewModel> Pagina(string seccion, int? alumno)
        {
            int usuId = UsuId();
            var modelo = new PortalPaginaViewModel
            {
                Seccion = seccion,
                Encargado = await _service.EncargadoAsync(usuId),
                Hijos = await _service.HijosAsync(usuId),
                Aviso = TempData["PortalAviso"] as string,
                Error = TempData["PortalError"] as string
            };
            modelo.Hijo = alumno.HasValue ? modelo.Hijos.FirstOrDefault(h => h.Alu_Id == alumno.Value) : null;
            if (alumno.HasValue && modelo.Hijo == null && modelo.Hijos.Count > 0)
            {
                modelo.Error = "Ese alumno no está ligado a su cuenta.";
            }
            modelo.Hijo ??= modelo.Hijos.FirstOrDefault();
            return modelo;
        }

        public async Task<IActionResult> Index(int? alumno)
        {
            var modelo = await Pagina("inicio", alumno);
            modelo.Anuncios = (await _service.AnunciosAsync()).Take(3).ToList();
            return View("Index", modelo);
        }

        public async Task<IActionResult> Notas(int? alumno)
        {
            var modelo = await Pagina("notas", alumno);
            if (modelo.Hijo != null)
                modelo.Notas = await _service.NotasAsync(UsuId(), modelo.Hijo.Alu_Id, modelo.Hijo.Anio);
            return View("Index", modelo);
        }

        public async Task<IActionResult> Asistencia(int? alumno)
        {
            var modelo = await Pagina("asistencia", alumno);
            if (modelo.Hijo != null)
                modelo.Asistencia = await _service.AsistenciaAsync(UsuId(), modelo.Hijo.Alu_Id, null);
            return View("Index", modelo);
        }

        public async Task<IActionResult> Cuenta(int? alumno)
        {
            var modelo = await Pagina("cuenta", alumno);
            if (modelo.Hijo != null)
                modelo.Cargos = await _service.CuentaAsync(UsuId(), modelo.Hijo.Alu_Id, null);
            return View("Index", modelo);
        }

        public async Task<IActionResult> Calendario(int? alumno, string mes)
        {
            var modelo = await Pagina("calendario", alumno);
            var hoy = DateTime.Today;
            modelo.Mes = DateTime.TryParseExact(mes ?? "", "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var m)
                ? m : new DateTime(hoy.Year, hoy.Month, 1);
            if (modelo.Hijo != null)
                modelo.Eventos = await _service.CalendarioAsync(UsuId(), modelo.Hijo.Alu_Id, modelo.Mes, modelo.Mes.AddMonths(1).AddDays(-1));
            return View("Index", modelo);
        }

        public async Task<IActionResult> Anuncios(int? alumno)
        {
            var modelo = await Pagina("anuncios", alumno);
            modelo.Anuncios = await _service.AnunciosAsync();
            return View("Index", modelo);
        }

        public async Task<IActionResult> Solicitudes(int? alumno, string tipo, DateTime? fecha)
        {
            var modelo = await Pagina("solicitudes", alumno);
            if (modelo.Hijo != null)
            {
                modelo.Solicitudes = await _service.SolicitudesAsync(UsuId(), modelo.Hijo.Alu_Id);
                modelo.Nueva = new PortalSolicitudCrearViewModel
                {
                    Alu_Id = modelo.Hijo.Alu_Id,
                    Tipo = PortalTextos.Tipos.Contains(tipo) ? tipo : null,
                    FechaFalta = fecha,
                    Asunto = tipo == "justificar" && fecha.HasValue ? $"Justificar la falta del {fecha.Value:dd/MM/yyyy}" : null
                };
            }
            return View("Index", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(PortalSolicitudCrearViewModel model)
        {
            model.Usuario = UsuId();
            var r = await _service.SolicitudCrearAsync(model);
            if (r.Ok)
                TempData["PortalAviso"] = "Su solicitud se envió al colegio. Aquí verá la respuesta.";
            else
                TempData["PortalError"] = r.Mensaje ?? "No se pudo enviar la solicitud.";
            return RedirectToAction("Solicitudes", new { alumno = model.Alu_Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retirar(int id, int? alumno)
        {
            var r = await _service.SolicitudRetirarAsync(new PortalSolicitudRetirarViewModel { Usuario = UsuId(), Sol_Id = id });
            if (r.Ok)
                TempData["PortalAviso"] = "La solicitud quedó retirada.";
            else
                TempData["PortalError"] = r.Mensaje ?? "No se pudo retirar la solicitud.";
            return RedirectToAction("Solicitudes", new { alumno });
        }
    }
}
