using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Sustitución de docente (API Sustituciones): docentes ausentes y sus clases del día, suplentes
    /// sugeridos, docentes para marcar ausentes, historial y las escrituras. Las lecturas mandan el
    /// alcance (empId null = todo; un número = solo lo de ese docente).
    /// </summary>
    public class SustitucionesService
    {
        private const string Base = "Sustituciones/";

        private static string F(DateTime d) => d.ToString("yyyy-MM-dd");

        private static string Alcance(int? empId) => empId.HasValue ? "&empId=" + empId.Value : "";

        private static SustitucionResultadoViewModel Fallo(string mensaje) => new SustitucionResultadoViewModel { Ok = false, Mensaje = mensaje };

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        public Task<List<SustitucionDiaViewModel>> DiaAsync(DateTime fecha, int? empId) =>
            Lista<SustitucionDiaViewModel>($"{Base}DiaAsync?fecha={F(fecha)}{Alcance(empId)}");

        public async Task<SustitucionMesViewModel> MesAsync(DateTime fecha, int? empId)
        {
            try { return await ApiRequests.GetSingleAsync<SustitucionMesViewModel>($"{Base}MesAsync?fecha={F(fecha)}{Alcance(empId)}") ?? new SustitucionMesViewModel(); }
            catch { return new SustitucionMesViewModel(); }
        }

        public Task<List<SustitucionSugerenciaViewModel>> SugerenciasAsync(int horId, DateTime fecha) =>
            Lista<SustitucionSugerenciaViewModel>($"{Base}SugerenciasAsync?horId={horId}&fecha={F(fecha)}");

        public Task<List<SustitucionDocenteViewModel>> DocentesAsync(DateTime fecha) =>
            Lista<SustitucionDocenteViewModel>($"{Base}DocentesAsync?fecha={F(fecha)}");

        public Task<List<SustitucionHistorialViewModel>> HistorialAsync(DateTime desde, DateTime hasta, int? empId) =>
            Lista<SustitucionHistorialViewModel>($"{Base}HistorialAsync?desde={F(desde)}&hasta={F(hasta)}{Alcance(empId)}");

        private static async Task<SustitucionResultadoViewModel> Post(string accion, object model, string general)
        {
            try { return await ApiRequests.PostAsyncWithResponse<SustitucionResultadoViewModel>(Base + accion, model) ?? Fallo(general); }
            catch { return Fallo("No se pudo conectar con el servidor. Intente de nuevo."); }
        }

        public Task<SustitucionResultadoViewModel> AsignarAsync(SustitucionAsignarViewModel model) =>
            Post("AsignarAsync", model, "No se pudo asignar el suplente.");

        public Task<SustitucionResultadoViewModel> CancelarAsync(SustitucionCancelarViewModel model) =>
            Post("CancelarAsync", model, "No se pudo cancelar la sustitución.");

        public Task<SustitucionResultadoViewModel> MarcarAusenciaAsync(AusenciaDocenteMarcarViewModel model) =>
            Post("MarcarAusenciaAsync", model, "No se pudo marcar la ausencia.");

        public Task<SustitucionResultadoViewModel> QuitarAusenciaAsync(AusenciaDocenteQuitarViewModel model) =>
            Post("QuitarAusenciaAsync", model, "No se pudo quitar la ausencia.");
    }
}
