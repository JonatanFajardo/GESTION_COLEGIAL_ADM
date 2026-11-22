using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Plan de apoyo al alumno (API PlanesApoyo): directorio, indicadores, el plan con sus pasos, notas
    /// e historial, y las escrituras. Cada lectura manda quién lee (ve todos, ve confidenciales, usuario):
    /// la API decide qué planes puede ver.
    /// </summary>
    public class PlanesApoyoService
    {
        private const string Base = "PlanesApoyo/";

        /// <summary>"verTodos=…&amp;verConfidenciales=…&amp;usuId=…"</summary>
        private static string Lector(bool verTodos, bool verConfidenciales, int? usuId) =>
            "verTodos=" + (verTodos ? "true" : "false")
            + "&verConfidenciales=" + (verConfidenciales ? "true" : "false")
            + (usuId.HasValue ? "&usuId=" + usuId.Value : "");

        private static PlanApoyoResultadoViewModel Fallo(string mensaje) => new PlanApoyoResultadoViewModel { Ok = false, Mensaje = mensaje };

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        public Task<List<PlanApoyoListaViewModel>> ListAsync(bool verTodos, bool verConfidenciales, int? usuId) =>
            Lista<PlanApoyoListaViewModel>(Base + "ListAsync?" + Lector(verTodos, verConfidenciales, usuId));

        public async Task<PlanApoyoResumenViewModel> ResumenAsync(bool verTodos, bool verConfidenciales, int? usuId) =>
            await ApiRequests.GetSingleAsync<PlanApoyoResumenViewModel>(Base + "ResumenAsync?" + Lector(verTodos, verConfidenciales, usuId))
            ?? new PlanApoyoResumenViewModel();

        /// <summary>null si no existe o la persona no lo puede leer.</summary>
        public async Task<PlanApoyoViewModel> FindAsync(int id, bool verTodos, bool verConfidenciales, int? usuId)
        {
            var r = await ApiRequests.GetSingleAsync<PlanApoyoViewModel>($"{Base}FindAsync?value={id}&" + Lector(verTodos, verConfidenciales, usuId));
            return r == null || r.Apo_Id == 0 ? null : r;
        }

        public Task<List<PlanApoyoPasoViewModel>> PasosAsync(int id, bool verTodos, bool verConfidenciales, int? usuId) =>
            Lista<PlanApoyoPasoViewModel>($"{Base}PasosAsync?value={id}&" + Lector(verTodos, verConfidenciales, usuId));

        public Task<List<PlanApoyoNotaViewModel>> NotasAsync(int id, bool verTodos, bool verConfidenciales, int? usuId) =>
            Lista<PlanApoyoNotaViewModel>($"{Base}NotasAsync?value={id}&" + Lector(verTodos, verConfidenciales, usuId));

        public Task<List<PlanApoyoEstadoViewModel>> EstadosAsync(int id, bool verTodos, bool verConfidenciales, int? usuId) =>
            Lista<PlanApoyoEstadoViewModel>($"{Base}EstadosAsync?value={id}&" + Lector(verTodos, verConfidenciales, usuId));

        public Task<List<PlanApoyoUsuarioViewModel>> UsuariosAsync() => Lista<PlanApoyoUsuarioViewModel>(Base + "UsuariosAsync");

        public Task<List<PlanApoyoAlumnoViewModel>> AlumnosAsync() => Lista<PlanApoyoAlumnoViewModel>(Base + "AlumnosAsync");

        public Task<List<PlanApoyoSeccionViewModel>> SeccionesAsync(int cunId) => Lista<PlanApoyoSeccionViewModel>($"{Base}SeccionesAsync?cunId={cunId}");

        /// <summary>Planes abiertos (legibles) de los alumnos dados.</summary>
        public Task<List<PlanApoyoAbiertoViewModel>> AbiertosAsync(IEnumerable<int> alumnos, bool verTodos, bool verConfidenciales, int? usuId)
        {
            string lista = string.Join(",", (alumnos ?? Enumerable.Empty<int>()).Where(x => x > 0).Distinct().Take(500));
            if (lista.Length == 0) return Task.FromResult(new List<PlanApoyoAbiertoViewModel>());
            return Lista<PlanApoyoAbiertoViewModel>($"{Base}AbiertosAsync?alumnos={Uri.EscapeDataString(lista)}&" + Lector(verTodos, verConfidenciales, usuId));
        }

        private static async Task<PlanApoyoResultadoViewModel> Post(string accion, object model, string general)
        {
            try { return await ApiRequests.PostAsyncWithResponse<PlanApoyoResultadoViewModel>(Base + accion, model) ?? Fallo(general); }
            catch { return Fallo("No se pudo conectar con el servidor. Intente de nuevo."); }
        }

        public Task<PlanApoyoResultadoViewModel> CrearAsync(PlanApoyoGuardarViewModel model) =>
            Post("CrearAsync", model, "No se pudo abrir el plan.");

        public Task<PlanApoyoResultadoViewModel> EditarAsync(PlanApoyoGuardarViewModel model) =>
            Post("EditarAsync", model, "No se pudieron guardar los cambios del plan.");

        public Task<PlanApoyoResultadoViewModel> CambiarEstadoAsync(PlanApoyoCambiarEstadoViewModel model) =>
            Post("CambiarEstadoAsync", model, "No se pudo cambiar el estado del plan.");

        public Task<PlanApoyoResultadoViewModel> AgregarPasoAsync(PlanApoyoAgregarPasoViewModel model) =>
            Post("AgregarPasoAsync", model, "No se pudo agregar el paso.");

        public Task<PlanApoyoResultadoViewModel> CompletarPasoAsync(PlanApoyoPasoAccionViewModel model) =>
            Post("CompletarPasoAsync", model, "No se pudo marcar el paso como hecho.");

        public Task<PlanApoyoResultadoViewModel> QuitarPasoAsync(PlanApoyoPasoAccionViewModel model) =>
            Post("QuitarPasoAsync", model, "No se pudo quitar el paso.");

        public Task<PlanApoyoResultadoViewModel> AgregarNotaAsync(PlanApoyoAgregarNotaViewModel model) =>
            Post("AgregarNotaAsync", model, "No se pudo agregar la nota.");

        public Task<PlanApoyoResultadoViewModel> EliminarAsync(PlanApoyoEliminarViewModel model) =>
            Post("EliminarAsync", model, "No se pudo eliminar el plan.");
    }
}
