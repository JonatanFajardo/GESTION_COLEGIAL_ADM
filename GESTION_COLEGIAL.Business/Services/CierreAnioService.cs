using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Cierre del año escolar (API CierreAnio, script 090): resumen, revisión, reglas por nivel, secciones,
    /// resultados finales, decisiones de promoción, historial y las acciones del asistente.
    /// </summary>
    public class CierreAnioService
    {
        private const string Base = "CierreAnio/";

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        public async Task<CierreAnioResumenViewModel> ResumenAsync(int aniId)
        {
            try { return await ApiRequests.GetSingleAsync<CierreAnioResumenViewModel>($"{Base}ResumenAsync?aniId={aniId}"); }
            catch { return null; }
        }

        public Task<List<CierreAnioRevisionViewModel>> RevisionAsync(int aniId) =>
            Lista<CierreAnioRevisionViewModel>($"{Base}RevisionAsync?aniId={aniId}");

        public Task<List<CierreAnioReglaViewModel>> ReglasAsync(int aniId) =>
            Lista<CierreAnioReglaViewModel>($"{Base}ReglasAsync?aniId={aniId}");

        public Task<List<CierreAnioSeccionViewModel>> SeccionesAsync(int aniId) =>
            Lista<CierreAnioSeccionViewModel>($"{Base}SeccionesAsync?aniId={aniId}");

        public Task<List<CierreAnioResultadoViewModel>> ResultadosAsync(int aniId, int seaId) =>
            Lista<CierreAnioResultadoViewModel>($"{Base}ResultadosAsync?aniId={aniId}&seaId={seaId}");

        public Task<List<CierreAnioDecisionViewModel>> DecisionesAsync(int aniId, int seaId) =>
            Lista<CierreAnioDecisionViewModel>($"{Base}DecisionesAsync?aniId={aniId}&seaId={seaId}");

        public Task<List<CierreAnioHistorialViewModel>> HistorialAsync(int aniId) =>
            Lista<CierreAnioHistorialViewModel>($"{Base}HistorialAsync?aniId={aniId}");

        private static async Task<CierreAnioRespuestaViewModel> Post(string accion, CierreAnioAccionViewModel model, string general)
        {
            try
            {
                return await ApiRequests.PostAsyncWithResponse<CierreAnioRespuestaViewModel>(Base + accion, model)
                       ?? new CierreAnioRespuestaViewModel { Ok = false, Mensaje = general };
            }
            catch { return new CierreAnioRespuestaViewModel { Ok = false, Mensaje = "No se pudo conectar con el servidor. Intente de nuevo." }; }
        }

        public Task<CierreAnioRespuestaViewModel> EstadoAsync(CierreAnioAccionViewModel m) => Post("EstadoAsync", m, "No se pudo cambiar el estado del año.");
        public Task<CierreAnioRespuestaViewModel> CalcularAsync(CierreAnioAccionViewModel m) => Post("CalcularAsync", m, "No se pudieron calcular los resultados.");
        public Task<CierreAnioRespuestaViewModel> ReglaAsync(CierreAnioAccionViewModel m) => Post("ReglaAsync", m, "No se pudo guardar la regla.");
        public Task<CierreAnioRespuestaViewModel> RecuperacionAsync(CierreAnioAccionViewModel m) => Post("RecuperacionAsync", m, "No se pudo guardar la nota de recuperación.");
        public Task<CierreAnioRespuestaViewModel> DecisionAsync(CierreAnioAccionViewModel m) => Post("DecisionAsync", m, "No se pudo guardar la decisión.");
        public Task<CierreAnioRespuestaViewModel> ConfirmarSeccionAsync(CierreAnioAccionViewModel m) => Post("ConfirmarSeccionAsync", m, "No se pudieron confirmar las decisiones.");
        public Task<CierreAnioRespuestaViewModel> DeshacerSeccionAsync(CierreAnioAccionViewModel m) => Post("DeshacerSeccionAsync", m, "No se pudo deshacer la confirmación.");
    }
}
