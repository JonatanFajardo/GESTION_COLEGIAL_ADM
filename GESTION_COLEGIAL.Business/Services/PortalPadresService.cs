using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Portal de padres (API PortalPadres): lo que ve la familia, sus solicitudes, la bandeja de
    /// Secretaría y las invitaciones. Con la llave de la API configurada, el usuario lo pone el servidor
    /// (usuario firmado); sin ella se manda el de la sesión.
    /// </summary>
    public class PortalPadresService
    {
        private const string Base = "PortalPadres/";

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        private static async Task<T> Uno<T>(string url) where T : class
        {
            try { return await ApiRequests.GetSingleAsync<T>(url); }
            catch { return null; }
        }

        private static async Task<PortalResultadoViewModel> Post(string accion, object model, string general)
        {
            try
            {
                return await ApiRequests.PostAsyncWithResponse<PortalResultadoViewModel>(Base + accion, model)
                       ?? new PortalResultadoViewModel { Ok = false, Mensaje = general };
            }
            catch { return new PortalResultadoViewModel { Ok = false, Mensaje = "No se pudo conectar con el servidor. Intente de nuevo." }; }
        }

        private static string Q(string valor) => Uri.EscapeDataString(valor ?? "");

        // ── Familia ──
        public async Task<PortalEncargadoViewModel> EncargadoAsync(int usuId)
        {
            var r = await Uno<PortalEncargadoViewModel>($"{Base}EncargadoAsync?usuId={usuId}");
            return r == null || r.Enc_Id == 0 ? null : r;
        }

        public Task<List<PortalHijoViewModel>> HijosAsync(int usuId) =>
            Lista<PortalHijoViewModel>($"{Base}HijosAsync?usuId={usuId}");

        public Task<List<PortalNotaViewModel>> NotasAsync(int usuId, int aluId, int? anio) =>
            Lista<PortalNotaViewModel>($"{Base}NotasAsync?aluId={aluId}&usuId={usuId}" + (anio.HasValue ? "&anio=" + anio : ""));

        public Task<List<PortalAsistenciaViewModel>> AsistenciaAsync(int usuId, int aluId, int? anio) =>
            Lista<PortalAsistenciaViewModel>($"{Base}AsistenciaAsync?aluId={aluId}&usuId={usuId}" + (anio.HasValue ? "&anio=" + anio : ""));

        public Task<List<PortalCargoViewModel>> CuentaAsync(int usuId, int aluId, int? anio) =>
            Lista<PortalCargoViewModel>($"{Base}CuentaAsync?aluId={aluId}&usuId={usuId}" + (anio.HasValue ? "&anio=" + anio : ""));

        public Task<List<PortalEventoViewModel>> CalendarioAsync(int usuId, int aluId, DateTime desde, DateTime hasta) =>
            Lista<PortalEventoViewModel>($"{Base}CalendarioAsync?aluId={aluId}&usuId={usuId}&desde={desde:yyyy-MM-dd}&hasta={hasta:yyyy-MM-dd}");

        public Task<List<PortalAnuncioViewModel>> AnunciosAsync() =>
            Lista<PortalAnuncioViewModel>($"{Base}AnunciosAsync");

        public Task<List<PortalSolicitudViewModel>> SolicitudesAsync(int usuId, int? aluId) =>
            Lista<PortalSolicitudViewModel>($"{Base}SolicitudesAsync?usuId={usuId}" + (aluId.HasValue ? "&aluId=" + aluId : ""));

        public Task<PortalResultadoViewModel> SolicitudCrearAsync(PortalSolicitudCrearViewModel model) =>
            Post("SolicitudCrearAsync", model, "No se pudo enviar la solicitud.");

        public Task<PortalResultadoViewModel> SolicitudRetirarAsync(PortalSolicitudRetirarViewModel model) =>
            Post("SolicitudRetirarAsync", model, "No se pudo retirar la solicitud.");

        // ── Bandeja ──
        public Task<List<PortalBandejaFilaViewModel>> BandejaAsync(string estado, string tipo, string texto) =>
            Lista<PortalBandejaFilaViewModel>($"{Base}BandejaAsync?estado={Q(estado)}&tipo={Q(tipo)}&texto={Q(texto)}");

        public async Task<PortalBandejaResumenViewModel> BandejaResumenAsync() =>
            await Uno<PortalBandejaResumenViewModel>($"{Base}BandejaResumenAsync") ?? new PortalBandejaResumenViewModel();

        public async Task<PortalBandejaDetalleViewModel> BandejaFindAsync(int solId)
        {
            var r = await Uno<PortalBandejaDetalleViewModel>($"{Base}BandejaFindAsync?value={solId}");
            return r == null || r.Sol_Id == 0 ? null : r;
        }

        public Task<List<PortalBandejaHistorialViewModel>> BandejaHistorialAsync(int solId) =>
            Lista<PortalBandejaHistorialViewModel>($"{Base}BandejaHistorialAsync?value={solId}");

        public Task<PortalResultadoViewModel> BandejaCambiarAsync(PortalBandejaCambiarViewModel model) =>
            Post("BandejaCambiarAsync", model, "No se pudo cambiar la solicitud.");

        // ── Invitaciones ──
        public Task<List<PortalInvitacionFilaViewModel>> InvitacionesAsync(string texto, string estado, bool soloConHijos) =>
            Lista<PortalInvitacionFilaViewModel>($"{Base}InvitacionesAsync?texto={Q(texto)}&estado={Q(estado)}&soloConHijos={(soloConHijos ? "true" : "false")}");

        public async Task<PortalInvitacionesResumenViewModel> InvitacionesResumenAsync() =>
            await Uno<PortalInvitacionesResumenViewModel>($"{Base}InvitacionesResumenAsync") ?? new PortalInvitacionesResumenViewModel();

        public async Task<PortalInvitacionCreadaViewModel> InvitarAsync(PortalInvitacionAccionViewModel model)
        {
            try
            {
                return await ApiRequests.PostAsyncWithResponse<PortalInvitacionCreadaViewModel>(Base + "InvitarAsync", model)
                       ?? new PortalInvitacionCreadaViewModel { Ok = false, Mensaje = "No se pudo generar la invitación." };
            }
            catch { return new PortalInvitacionCreadaViewModel { Ok = false, Mensaje = "No se pudo conectar con el servidor. Intente de nuevo." }; }
        }

        public Task<PortalResultadoViewModel> RevocarAsync(PortalInvitacionAccionViewModel model) =>
            Post("RevocarAsync", model, "No se pudo detener la invitación.");

        public Task<PortalResultadoViewModel> AccesoAsync(PortalInvitacionAccionViewModel model) =>
            Post("AccesoAsync", model, "No se pudo cambiar el acceso.");

        // ── Activación pública ──
        public async Task<PortalInvitacionInfoViewModel> InvitacionInfoAsync(string codigo) =>
            await Uno<PortalInvitacionInfoViewModel>($"{Base}InvitacionAsync?codigo={Q(codigo)}")
            ?? new PortalInvitacionInfoViewModel { Estado = "no_existe" };

        public Task<PortalResultadoViewModel> ActivarAsync(PortalActivarViewModel model) =>
            Post("ActivarAsync", model, "No se pudo activar la cuenta.");
    }
}
