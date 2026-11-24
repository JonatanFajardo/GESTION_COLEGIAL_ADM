using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Invitaciones al portal de padres (espejo de Skuul "users/invitations"): Secretaría genera el enlace
    /// de cada encargado, ve si ya lo usó, lo detiene o suspende la cuenta. No hay envío de correo en el
    /// sistema: el enlace se copia y se manda por el medio que use el colegio.
    /// Permiso "Invitaciones al portal" (admin, secretaria); la API lo vuelve a exigir.
    /// </summary>
    [SessionManager("Invitaciones al portal")]
    public class InvitacionesPortalController : BaseController
    {
        private readonly PortalPadresService _service = new PortalPadresService();

        private int UsuId() => int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : 0;

        public async Task<IActionResult> Index(string texto, string estado, bool todos = false)
        {
            var modelo = new InvitacionesPortalIndexViewModel
            {
                Texto = texto,
                Estado = estado,
                Todos = todos,
                Filas = await _service.InvitacionesAsync(texto, estado, !todos),
                Resumen = await _service.InvitacionesResumenAsync(),
                Aviso = TempData["InvAviso"] as string,
                Error = TempData["InvError"] as string,
                Enlace = TempData["InvEnlace"] as string,
                EnlaceNombre = TempData["InvNombre"] as string,
                EnlaceVence = TempData["InvVence"] as string
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invitar(int encId, string texto, string estado, bool todos = false)
        {
            var r = await _service.InvitarAsync(new PortalInvitacionAccionViewModel { Usuario = UsuId(), Enc_Id = encId });
            if (r.Ok && !string.IsNullOrEmpty(r.Codigo))
            {
                TempData["InvEnlace"] = Url.Action("Index", "Invitacion", new { codigo = r.Codigo }, Request.Scheme);
                TempData["InvNombre"] = r.Nombre;
                TempData["InvVence"] = r.Vence?.ToString("dd/MM/yyyy");
            }
            else
            {
                TempData["InvError"] = r.Mensaje ?? "No se pudo generar la invitación.";
            }
            return RedirectToAction("Index", new { texto, estado, todos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revocar(int encId, string texto, string estado, bool todos = false)
        {
            var r = await _service.RevocarAsync(new PortalInvitacionAccionViewModel { Usuario = UsuId(), Enc_Id = encId });
            TempData[r.Ok ? "InvAviso" : "InvError"] = r.Ok ? "La invitación quedó detenida: el enlace ya no sirve." : r.Mensaje;
            return RedirectToAction("Index", new { texto, estado, todos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acceso(int encId, bool suspender, string texto, string estado, bool todos = false)
        {
            var r = await _service.AccesoAsync(new PortalInvitacionAccionViewModel { Usuario = UsuId(), Enc_Id = encId, Suspender = suspender });
            TempData[r.Ok ? "InvAviso" : "InvError"] = r.Ok
                ? (suspender ? "La cuenta quedó suspendida: ya no puede entrar al portal." : "La cuenta volvió a tener acceso al portal.")
                : r.Mensaje;
            return RedirectToAction("Index", new { texto, estado, todos });
        }
    }
}
