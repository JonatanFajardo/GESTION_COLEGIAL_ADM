using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Notas ponderadas (API Evaluaciones, script 089): plantillas de evaluación de Dirección, uso por grupo y
    /// las actividades del cuaderno de notas con sus puntos. La API recibe el usuario y su procedimiento
    /// decide qué puede ver y hacer (el docente, solo los grupos de su horario).
    /// </summary>
    public class EvaluacionesService
    {
        private const string Base = "Evaluaciones/";

        private static EvaluacionRespuestaViewModel Fallo(string mensaje) => new EvaluacionRespuestaViewModel { Ok = false, Mensaje = mensaje };

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        private static async Task<EvaluacionRespuestaViewModel> Post(string accion, object model, string general)
        {
            try { return await ApiRequests.PostAsyncWithResponse<EvaluacionRespuestaViewModel>(Base + accion, model) ?? Fallo(general); }
            catch { return Fallo("No se pudo conectar con el servidor. Intente de nuevo."); }
        }

        /// <summary>JSON del cuaderno tal cual, o { Error, Message } si no se puede ver ("[]" si la API no respondió).</summary>
        public Task<string> CuadernoRawAsync(int horId, int pacId, int usuId) =>
            SendHttpClient.GetRawAsync($"{Base}CuadernoAsync?horId={horId}&pacId={pacId}&usuId={usuId}");

        public Task<EvaluacionRespuestaViewModel> GuardarActividadAsync(ActividadEvaluacionGuardarViewModel model) =>
            Post("GuardarActividadAsync", model, "No se pudo guardar la actividad.");

        public Task<EvaluacionRespuestaViewModel> QuitarActividadAsync(ActividadEvaluacionQuitarViewModel model) =>
            Post("QuitarActividadAsync", model, "No se pudo quitar la actividad.");

        public Task<EvaluacionRespuestaViewModel> GuardarPuntosAsync(PuntosEvaluacionGuardarViewModel model) =>
            Post("GuardarPuntosAsync", model, "No se pudieron guardar los puntos.");

        public Task<EvaluacionRespuestaViewModel> ActualizarNotasAsync(int cevId, int usuId) =>
            Post($"ActualizarNotasAsync?cevId={cevId}&usuId={usuId}", new { }, "No se pudieron actualizar las notas del parcial.");

        public Task<List<PlantillaEvaluacionViewModel>> PlantillasAsync(int anio, int usuId) =>
            Lista<PlantillaEvaluacionViewModel>($"{Base}PlantillasAsync?anio={anio}&usuId={usuId}");

        public Task<List<EvaluacionOpcionViewModel>> OpcionesAsync(int usuId, string tipo, int? anio = null, int? nivId = null) =>
            Lista<EvaluacionOpcionViewModel>($"{Base}OpcionesAsync?usuId={usuId}&tipo={tipo}"
                + (anio.HasValue ? $"&anio={anio}" : "") + (nivId.HasValue ? $"&nivId={nivId}" : ""));

        public Task<EvaluacionRespuestaViewModel> GuardarPlantillaAsync(PlantillaEvaluacionGuardarViewModel model) =>
            Post("GuardarPlantillaAsync", model, "No se pudo guardar la plantilla.");

        public Task<EvaluacionRespuestaViewModel> QuitarPlantillaAsync(PlantillaEvaluacionQuitarViewModel model) =>
            Post("QuitarPlantillaAsync", model, "No se pudo quitar la plantilla.");

        public Task<List<EvaluacionAvanceViewModel>> AvanceAsync(int anio, int semId, int pacId, int usuId) =>
            Lista<EvaluacionAvanceViewModel>($"{Base}AvanceAsync?anio={anio}&semId={semId}&pacId={pacId}&usuId={usuId}");
    }
}
