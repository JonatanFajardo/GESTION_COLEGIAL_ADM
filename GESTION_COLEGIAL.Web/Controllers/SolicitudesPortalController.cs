using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Bandeja de solicitudes del portal de padres (espejo de Skuul "portal-requests"): lo que pidieron
    /// las familias (justificar una falta, cita, constancia u otro), filtrar, ponerlo en proceso,
    /// atenderlo o rechazarlo con una respuesta que la familia ve, e historial de cada cambio.
    /// Permiso "Solicitudes del portal" (admin, secretaria, director); la API lo vuelve a exigir.
    /// </summary>
    [SessionManager("Solicitudes del portal")]
    public class SolicitudesPortalController : BaseController
    {
        private readonly PortalPadresService _service = new PortalPadresService();

        private int UsuId() => int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : 0;

        private bool EsAdmin() => (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        public async Task<IActionResult> Index(string estado = "abiertas", string tipo = null, string texto = null)
        {
            var modelo = new SolicitudesPortalIndexViewModel
            {
                Estado = estado,
                Tipo = tipo,
                Texto = texto,
                Filas = await _service.BandejaAsync(estado == "todas" ? null : estado, tipo, texto),
                Resumen = await _service.BandejaResumenAsync()
            };
            return View(modelo);
        }

        public async Task<IActionResult> Ver(int id)
        {
            var solicitud = await _service.BandejaFindAsync(id);
            if (solicitud == null) return RedirectToAction("Index");
            var modelo = new SolicitudesPortalVerViewModel
            {
                Solicitud = solicitud,
                Historial = await _service.BandejaHistorialAsync(id),
                PuedeEmitirConstancias = EsAdmin() || TienePantalla("Constancias y expediente"),
                Aviso = TempData["SolAviso"] as string,
                Error = TempData["SolError"] as string,
                EstadoElegido = TempData["SolEstado"] as string,
                Respuesta = TempData["SolRespuesta"] as string
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cambiar(int id, string estado, string respuesta)
        {
            var r = await _service.BandejaCambiarAsync(new PortalBandejaCambiarViewModel
            {
                Usuario = UsuId(),
                Sol_Id = id,
                Estado = estado,
                Respuesta = respuesta
            });
            if (r.Ok)
            {
                TempData["SolAviso"] = "Listo: la solicitud quedó " + PortalTextos.Estado(r.Estado).ToLowerInvariant() + ". La familia ya lo ve en su portal.";
            }
            else
            {
                TempData["SolError"] = r.Mensaje ?? "No se pudo cambiar la solicitud.";
                TempData["SolEstado"] = estado;
                TempData["SolRespuesta"] = respuesta;
            }
            return RedirectToAction("Ver", new { id });
        }
    }
}
