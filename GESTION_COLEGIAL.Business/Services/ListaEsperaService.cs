using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Lista de espera de admisión (API Aspirantes): años, resumen de cupo por grado, la cola,
    /// un aspirante con su historial, revisión de duplicados y los cambios de estado.
    /// </summary>
    public class ListaEsperaService
    {
        private const string Base = "Aspirantes/";

        private static AspiranteResultadoViewModel Fallo(string mensaje) => new AspiranteResultadoViewModel { Ok = false, Mensaje = mensaje };

        public async Task<List<AspiranteAnioViewModel>> AniosAsync() =>
            (await ApiRequests.ListAsync<AspiranteAnioViewModel>(Base + "AniosAsync"))?.ToList() ?? new List<AspiranteAnioViewModel>();

        public async Task<List<AspiranteResumenViewModel>> ResumenAsync(int aniId) =>
            (await ApiRequests.ListAsync<AspiranteResumenViewModel>($"{Base}ResumenAsync?aniId={aniId}"))?.ToList() ?? new List<AspiranteResumenViewModel>();

        public async Task<List<AspiranteListaViewModel>> ListAsync(int aniId) =>
            (await ApiRequests.ListAsync<AspiranteListaViewModel>($"{Base}ListAsync?aniId={aniId}"))?.ToList() ?? new List<AspiranteListaViewModel>();

        /// <summary>null si no existe.</summary>
        public async Task<AspiranteViewModel> FindAsync(int id) =>
            await ApiRequests.GetSingleAsync<AspiranteViewModel>($"{Base}FindAsync?value={id}");

        public async Task<List<AspiranteHistorialViewModel>> HistorialAsync(int id) =>
            (await ApiRequests.FindAllAsync<AspiranteHistorialViewModel>(Base + "HistorialAsync", id))?.ToList() ?? new List<AspiranteHistorialViewModel>();

        public async Task<List<AspiranteCursoViewModel>> CursosAsync(int cunId) =>
            (await ApiRequests.ListAsync<AspiranteCursoViewModel>($"{Base}CursosAsync?cunId={cunId}"))?.ToList() ?? new List<AspiranteCursoViewModel>();

        public async Task<AspiranteDuplicadoViewModel> DuplicadoAsync(int aniId, string identidad, string nombres, string apellidos,
                                                                     DateTime? fechaNacimiento, int? aspId)
        {
            string url = $"{Base}DuplicadoAsync?aniId={aniId}"
                + (string.IsNullOrWhiteSpace(identidad) ? "" : "&identidad=" + Uri.EscapeDataString(identidad.Trim()))
                + (string.IsNullOrWhiteSpace(nombres) ? "" : "&nombres=" + Uri.EscapeDataString(nombres.Trim()))
                + (string.IsNullOrWhiteSpace(apellidos) ? "" : "&apellidos=" + Uri.EscapeDataString(apellidos.Trim()))
                + (fechaNacimiento.HasValue ? "&fechaNacimiento=" + fechaNacimiento.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "")
                + (aspId.HasValue && aspId.Value > 0 ? "&aspId=" + aspId.Value : "");
            return await ApiRequests.GetSingleAsync<AspiranteDuplicadoViewModel>(url);
        }

        public async Task<AspiranteResultadoViewModel> GuardarAsync(AspiranteGuardarViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<AspiranteResultadoViewModel>(Base + "GuardarAsync", model)
            ?? Fallo("No se pudo guardar el aspirante.");

        public async Task<AspiranteResultadoViewModel> CambiarEstadoAsync(AspiranteEstadoViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<AspiranteResultadoViewModel>(Base + "CambiarEstadoAsync", model)
            ?? Fallo("No se pudo cambiar el estado del aspirante.");

        /// <summary>Lo llama el wizard de Matrícula después de crear al alumno.</summary>
        public async Task<AspiranteResultadoViewModel> MatricularAsync(int aspId, int aluId, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<AspiranteResultadoViewModel>(Base + "MatricularAsync",
                new AspiranteMatricularViewModel { Asp_Id = aspId, Alu_Id = aluId, Usuario = usuario })
            ?? Fallo("No se pudo marcar al aspirante como matriculado.");
    }
}
