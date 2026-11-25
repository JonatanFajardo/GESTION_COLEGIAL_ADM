using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Conexión con el año lectivo (API AnioConexion): cupos de las secciones del año, años donde
    /// se puede inscribir, inscripción de un alumno en un año, paso de la ubicación al abrir el año
    /// y semestres desde los períodos del año.
    /// </summary>
    public class AnioConexionService
    {
        private const string Base = "AnioConexion/";

        private static AnioConexionResultadoViewModel Fallo(string mensaje) => new AnioConexionResultadoViewModel { Ok = false, Mensaje = mensaje };

        public async Task<List<CupoSeccionViewModel>> CuposAsync(int aniId) =>
            (await ApiRequests.ListAsync<CupoSeccionViewModel>($"{Base}CuposAsync?aniId={aniId}"))?.ToList() ?? new List<CupoSeccionViewModel>();

        public async Task<AnioConexionResultadoViewModel> ActualizarCupoAsync(int seaId, int cupo, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<AnioConexionResultadoViewModel>(Base + "ActualizarCupoAsync",
                new { Sea_Id = seaId, Sea_Cupo = cupo, Usuario = usuario })
            ?? Fallo("No se pudo cambiar el cupo.");

        public async Task<List<AnioInscripcionViewModel>> AniosInscripcionAsync() =>
            (await ApiRequests.ListAsync<AnioInscripcionViewModel>(Base + "AniosInscripcionAsync"))?.ToList() ?? new List<AnioInscripcionViewModel>();

        /// <summary>null si la API no responde.</summary>
        public async Task<VerificarCupoViewModel> VerificarCupoAsync(int? aniId, int nivId, int cunId, int? mdaId, int secId)
        {
            string url = $"{Base}VerificarCupoAsync?nivId={nivId}&cunId={cunId}&secId={secId}"
                + (aniId.HasValue ? "&aniId=" + aniId.Value : "")
                + (mdaId.HasValue ? "&mdaId=" + mdaId.Value : "");
            return await ApiRequests.GetSingleAsync<VerificarCupoViewModel>(url);
        }

        public async Task<AnioConexionResultadoViewModel> InscribirAlumnoAsync(int aluId, int? aniId, int usuario) =>
            await ApiRequests.PostAsyncWithResponse<AnioConexionResultadoViewModel>(Base + "InscribirAlumnoAsync",
                new { Alu_Id = aluId, Ani_Id = aniId, Usuario = usuario })
            ?? Fallo("No se pudo inscribir al alumno en el año.");

        /// <summary>Cuántos alumnos cambiarían de sección al abrir el año (no cambia nada).</summary>
        public async Task<AnioConexionResultadoViewModel> UbicacionPreviaAsync(int aniId) =>
            await ApiRequests.GetSingleAsync<AnioConexionResultadoViewModel>($"{Base}UbicacionPreviaAsync?aniId={aniId}")
            ?? Fallo("No se pudo calcular cuántos alumnos cambiarían de sección.");

        /// <summary>Pasa la ubicación de los inscritos al año y, con activar, lo marca como activo en la misma transacción.</summary>
        public async Task<AnioConexionResultadoViewModel> AplicarUbicacionAsync(int aniId, int usuario, bool activar = false) =>
            await ApiRequests.PostAsyncWithResponse<AnioConexionResultadoViewModel>(Base + "AplicarUbicacionAsync",
                new { Ani_Id = aniId, Usuario = usuario, Activar = activar })
            ?? Fallo("No se pudo pasar la ubicación de los alumnos al año.");

        public async Task<List<SemestreAnioViewModel>> SemestresAsync(int anio) =>
            (await ApiRequests.ListAsync<SemestreAnioViewModel>($"{Base}SemestresAsync?anio={anio}"))?.ToList() ?? new List<SemestreAnioViewModel>();
    }
}
