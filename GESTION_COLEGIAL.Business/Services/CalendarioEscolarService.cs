using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Calendario escolar (API EventosCalendario): días del calendario, "para quién",
    /// borrador/publicado, cierres que bloquean el pase de lista, próximos días y feriados de Honduras.
    /// </summary>
    public class CalendarioEscolarService
    {
        private const string Base = "EventosCalendario/";

        private static string F(DateTime d) => d.ToString("yyyy-MM-dd");

        /// <summary>"&amp;verTodo=…&amp;empId=…&amp;rolId=…": quien edita ve todo; el resto, lo de todo el colegio + lo suyo.</summary>
        private static string Persona(bool verTodo, int? empId, int? rolId) =>
            "&verTodo=" + (verTodo ? "true" : "false")
            + (empId.HasValue ? "&empId=" + empId.Value : "")
            + (rolId.HasValue ? "&rolId=" + rolId.Value : "");

        public async Task<List<EventoCalendarioViewModel>> ListRangoAsync(DateTime desde, DateTime hasta, bool incluirBorradores,
                                                                         bool verTodo, int? empId, int? rolId)
        {
            var r = await ApiRequests.ListAsync<EventoCalendarioViewModel>(
                $"{Base}ListRangoAsync?desde={F(desde)}&hasta={F(hasta)}&incluirBorradores={(incluirBorradores ? "true" : "false")}"
                + Persona(verTodo, empId, rolId));
            return r?.ToList() ?? new List<EventoCalendarioViewModel>();
        }

        /// <summary>Un día del calendario; null si no existe o (para quien no edita) no es para esa persona.</summary>
        public async Task<EventoCalendarioViewModel> FindAsync(int id, bool verTodo, int? empId, int? rolId) =>
            await ApiRequests.GetSingleAsync<EventoCalendarioViewModel>($"{Base}FindAsync?value={id}" + Persona(verTodo, empId, rolId));

        public async Task<List<EventoAudienciaViewModel>> AudienciaAsync(int id)
        {
            var r = await ApiRequests.FindAllAsync<EventoAudienciaViewModel>(Base + "AudienciaAsync", id);
            return r?.ToList() ?? new List<EventoAudienciaViewModel>();
        }

        /// <summary>Borradores por mes (los meses sin borradores no vienen).</summary>
        public async Task<List<BorradoresCalendarioViewModel>> BorradoresAsync()
        {
            var r = await ApiRequests.ListAsync<BorradoresCalendarioViewModel>(Base + "BorradoresAsync");
            return r?.ToList() ?? new List<BorradoresCalendarioViewModel>();
        }

        /// <summary>¿Cierra el colegio ese día para la sección del año o la del horario?</summary>
        public async Task<CierreCalendarioViewModel> CierreEnFechaAsync(DateTime fecha, int? seaId, int? horId)
        {
            string url = $"{Base}CierreEnFechaAsync?fecha={F(fecha)}"
                + (seaId.HasValue ? "&seaId=" + seaId.Value : "")
                + (horId.HasValue ? "&horId=" + horId.Value : "");
            return await ApiRequests.GetSingleAsync<CierreCalendarioViewModel>(url)
                   ?? new CierreCalendarioViewModel { Cerrado = false, Fecha = fecha };
        }

        public async Task<List<ProximoEventoViewModel>> ProximosAsync(DateTime hoy, int dias, bool verTodo, int? empId, int? rolId, int limite = 5)
        {
            string url = $"{Base}ProximosAsync?hoy={F(hoy)}&dias={dias}&verTodo={(verTodo ? "true" : "false")}&limite={limite}"
                + (empId.HasValue ? "&empId=" + empId.Value : "")
                + (rolId.HasValue ? "&rolId=" + rolId.Value : "");
            var r = await ApiRequests.ListAsync<ProximoEventoViewModel>(url);
            return r?.ToList() ?? new List<ProximoEventoViewModel>();
        }

        public async Task<List<SeccionAudienciaViewModel>> SeccionesAudienciaAsync(DateTime? fecha, int? eveId)
        {
            string url = $"{Base}SeccionesAudienciaAsync?x=1"
                + (fecha.HasValue ? "&fecha=" + F(fecha.Value) : "")
                + (eveId.HasValue ? "&eveId=" + eveId.Value : "");
            var r = await ApiRequests.ListAsync<SeccionAudienciaViewModel>(url);
            return r?.ToList() ?? new List<SeccionAudienciaViewModel>();
        }

        public async Task<List<RolAudienciaViewModel>> RolesAudienciaAsync()
        {
            var r = await ApiRequests.ListAsync<RolAudienciaViewModel>(Base + "RolesAudienciaAsync");
            return r?.ToList() ?? new List<RolAudienciaViewModel>();
        }

        public async Task<List<PersonaAudienciaViewModel>> BuscarPersonasAsync(string texto)
        {
            var r = await ApiRequests.ListAsync<PersonaAudienciaViewModel>(
                $"{Base}BuscarPersonasAsync?limite=8&texto={Uri.EscapeDataString(texto ?? "")}");
            return r?.ToList() ?? new List<PersonaAudienciaViewModel>();
        }

        public async Task<EventoGuardarResultadoViewModel> GuardarAsync(EventoCalendarioGuardarViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<EventoGuardarResultadoViewModel>(Base + "GuardarAsync", model)
            ?? new EventoGuardarResultadoViewModel { Eve_Id = 0, Mensaje = "No se pudo guardar el día en el calendario." };

        public async Task<EventoPublicarResultadoViewModel> PublicarAsync(int id, bool publicar, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<EventoPublicarResultadoViewModel>(
                $"{Base}PublicarAsync?id={id}&publicar={(publicar ? "true" : "false")}&usuario={usuario}", new { });

        public async Task<EventoEliminarResultadoViewModel> EliminarAsync(int id, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<EventoEliminarResultadoViewModel>(
                $"{Base}EliminarAsync?id={id}&usuario={usuario}", new { });

        public async Task<List<FeriadoPropuestoViewModel>> FeriadosHondurasAsync(int anio)
        {
            var r = await ApiRequests.ListAsync<FeriadoPropuestoViewModel>($"{Base}FeriadosHondurasAsync?anio={anio}");
            return r?.ToList() ?? new List<FeriadoPropuestoViewModel>();
        }

        public async Task<CargarFeriadosResultadoViewModel> CargarFeriadosHondurasAsync(CargarFeriadosViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<CargarFeriadosResultadoViewModel>(Base + "CargarFeriadosHondurasAsync", model);
    }
}
