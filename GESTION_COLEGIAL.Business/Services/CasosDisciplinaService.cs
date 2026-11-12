using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Disciplina (API CasosDisciplina): casos de conducta y de protección del menor (reservados),
    /// sus personas, acciones, notas e historial de estados.
    /// </summary>
    public class CasosDisciplinaService
    {
        private const string Base = "CasosDisciplina/";

        /// <summary>"&amp;verReservados=…&amp;usuId=…&amp;empId=…": quién lee (los reservados dependen de esto).</summary>
        private static string Lector(bool verReservados, int? usuId, int? empId) =>
            "&verReservados=" + (verReservados ? "true" : "false")
            + (usuId.HasValue ? "&usuId=" + usuId.Value : "")
            + (empId.HasValue ? "&empId=" + empId.Value : "");

        private static CasoResultadoViewModel Fallo(string mensaje) => new CasoResultadoViewModel { Ok = false, Mensaje = mensaje };

        public async Task<List<CasoDisciplinaListaViewModel>> ListAsync(string estado, string tipo, bool soloAbiertos, int pagina,
                                                                       bool verReservados, int? usuId, int? empId)
        {
            string url = $"{Base}ListAsync?pagina={pagina}&soloAbiertos={(soloAbiertos ? "true" : "false")}"
                + (string.IsNullOrEmpty(estado) ? "" : "&estado=" + Uri.EscapeDataString(estado))
                + (string.IsNullOrEmpty(tipo) ? "" : "&tipo=" + Uri.EscapeDataString(tipo))
                + Lector(verReservados, usuId, empId);
            var r = await ApiRequests.ListAsync<CasoDisciplinaListaViewModel>(url);
            return r?.ToList() ?? new List<CasoDisciplinaListaViewModel>();
        }

        public async Task<int> AbiertosAsync(bool verReservados, int? usuId, int? empId)
        {
            var r = await ApiRequests.GetSingleAsync<CasosAbiertosViewModel>($"{Base}AbiertosAsync?x=1" + Lector(verReservados, usuId, empId));
            return r?.Abiertos ?? 0;
        }

        /// <summary>null si no existe o la persona no lo puede leer.</summary>
        public async Task<CasoDisciplinaViewModel> FindAsync(int id, bool verReservados, int? usuId, int? empId) =>
            await ApiRequests.GetSingleAsync<CasoDisciplinaViewModel>($"{Base}FindAsync?value={id}" + Lector(verReservados, usuId, empId));

        public async Task<List<CasoPersonaViewModel>> PersonasAsync(int id) =>
            (await ApiRequests.FindAllAsync<CasoPersonaViewModel>(Base + "PersonasAsync", id))?.ToList() ?? new List<CasoPersonaViewModel>();

        public async Task<List<CasoAccionViewModel>> AccionesAsync(int id) =>
            (await ApiRequests.FindAllAsync<CasoAccionViewModel>(Base + "AccionesAsync", id))?.ToList() ?? new List<CasoAccionViewModel>();

        public async Task<List<CasoEstadoViewModel>> EstadosAsync(int id) =>
            (await ApiRequests.FindAllAsync<CasoEstadoViewModel>(Base + "EstadosAsync", id))?.ToList() ?? new List<CasoEstadoViewModel>();

        public async Task<List<CasoNotaViewModel>> NotasAsync(int id, bool verReservados, int? usuId, int? empId)
        {
            var r = await ApiRequests.ListAsync<CasoNotaViewModel>($"{Base}NotasAsync?value={id}" + Lector(verReservados, usuId, empId));
            return r?.ToList() ?? new List<CasoNotaViewModel>();
        }

        public async Task<List<CasoAlumnoViewModel>> AlumnosAsync() =>
            (await ApiRequests.ListAsync<CasoAlumnoViewModel>(Base + "AlumnosAsync"))?.ToList() ?? new List<CasoAlumnoViewModel>();

        public async Task<List<CasoEmpleadoViewModel>> EmpleadosAsync() =>
            (await ApiRequests.ListAsync<CasoEmpleadoViewModel>(Base + "EmpleadosAsync"))?.ToList() ?? new List<CasoEmpleadoViewModel>();

        public async Task<CasoResultadoViewModel> RegistrarAsync(CasoRegistrarViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<CasoResultadoViewModel>(Base + "RegistrarAsync", model)
            ?? Fallo("No se pudo registrar el caso.");

        public async Task<CasoResultadoViewModel> CambiarEstadoAsync(CasoCambiarEstadoViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<CasoResultadoViewModel>(Base + "CambiarEstadoAsync", model)
            ?? Fallo("No se pudo mover el caso.");

        public async Task<CasoResultadoViewModel> AgregarAccionAsync(CasoAgregarAccionViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<CasoResultadoViewModel>(Base + "AgregarAccionAsync", model)
            ?? Fallo("No se pudo agregar la acción.");

        public async Task<CasoResultadoViewModel> CompletarAccionAsync(int casId, int cacId, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<CasoResultadoViewModel>(
                $"{Base}CompletarAccionAsync?casId={casId}&cacId={cacId}&usuario={usuario}", new { })
            ?? Fallo("No se pudo marcar la acción como hecha.");

        public async Task<CasoResultadoViewModel> AgregarNotaAsync(CasoAgregarNotaViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<CasoResultadoViewModel>(Base + "AgregarNotaAsync", model)
            ?? Fallo("No se pudo agregar la nota.");
    }
}
