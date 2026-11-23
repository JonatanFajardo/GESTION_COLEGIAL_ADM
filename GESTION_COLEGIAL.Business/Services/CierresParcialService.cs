using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Cierre y publicación de parciales (API CierresParcial): años, tablero con avance, notas faltantes,
    /// historial, estado de un período (cuaderno de notas y boletín) y las acciones.
    /// </summary>
    public class CierresParcialService
    {
        private const string Base = "CierresParcial/";

        private static CierreParcialResultadoViewModel Fallo(string mensaje) => new CierreParcialResultadoViewModel { Ok = false, Mensaje = mensaje };

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        public Task<List<CierreParcialAnioViewModel>> AniosAsync() =>
            Lista<CierreParcialAnioViewModel>(Base + "AniosAsync");

        public Task<List<CierreParcialEstadoViewModel>> EstadoAsync(int anio) =>
            Lista<CierreParcialEstadoViewModel>($"{Base}EstadoAsync?anio={anio}");

        public Task<List<CierreParcialFaltanteViewModel>> FaltantesAsync(int anio, int semId, int pacId) =>
            Lista<CierreParcialFaltanteViewModel>($"{Base}FaltantesAsync?anio={anio}&semId={semId}&pacId={pacId}");

        public Task<List<CierreParcialHistorialViewModel>> HistorialAsync(int anio) =>
            Lista<CierreParcialHistorialViewModel>($"{Base}HistorialAsync?anio={anio}");

        /// <summary>Estado de los períodos de un año (para marcar resultados provisionales).</summary>
        public Task<List<CierreParcialPeriodoViewModel>> PeriodosAsync(int anio) =>
            Lista<CierreParcialPeriodoViewModel>($"{Base}PeriodosAsync?anio={anio}");

        /// <summary>Estado de un período; null si no se pudo leer.</summary>
        public async Task<CierreParcialPeriodoViewModel> PeriodoAsync(int anio, int semId, int pacId) =>
            (await Lista<CierreParcialPeriodoViewModel>($"{Base}PeriodosAsync?anio={anio}&semId={semId}&pacId={pacId}")).FirstOrDefault();

        /// <summary>Estado del período de una nota; null si no se pudo leer.</summary>
        public async Task<CierreParcialPeriodoViewModel> PeriodoDeNotaAsync(int notId) =>
            (await Lista<CierreParcialPeriodoViewModel>($"{Base}PeriodosAsync?notId={notId}")).FirstOrDefault();

        private static async Task<CierreParcialResultadoViewModel> Post(string accion, CierreParcialAccionViewModel model, string general)
        {
            try { return await ApiRequests.PostAsyncWithResponse<CierreParcialResultadoViewModel>(Base + accion, model) ?? Fallo(general); }
            catch { return Fallo("No se pudo conectar con el servidor. Intente de nuevo."); }
        }

        public Task<CierreParcialResultadoViewModel> CerrarAsync(CierreParcialAccionViewModel model) =>
            Post("CerrarAsync", model, "No se pudo cerrar el parcial.");

        public Task<CierreParcialResultadoViewModel> PublicarAsync(CierreParcialAccionViewModel model) =>
            Post("PublicarAsync", model, "No se pudieron publicar los resultados.");

        public Task<CierreParcialResultadoViewModel> DespublicarAsync(CierreParcialAccionViewModel model) =>
            Post("DespublicarAsync", model, "No se pudo retirar la publicación.");

        public Task<CierreParcialResultadoViewModel> ReabrirAsync(CierreParcialAccionViewModel model) =>
            Post("ReabrirAsync", model, "No se pudo reabrir el parcial.");

        public Task<CierreParcialResultadoViewModel> FechasAsync(CierreParcialAccionViewModel model) =>
            Post("FechasAsync", model, "No se pudieron guardar las fechas.");
    }
}
