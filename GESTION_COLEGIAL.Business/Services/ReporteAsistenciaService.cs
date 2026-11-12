using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Reporte de asistencia de alumnos (API ReporteAsistencia): resumen por sección,
    /// resumen por alumno y planilla día por día. Solo lectura.
    /// </summary>
    public class ReporteAsistenciaService
    {
        private const string Base = "ReporteAsistencia/";

        private static string Fecha(DateTime d) => d.ToString("yyyy-MM-dd");

        private static string Opcional(string nombre, int? valor) => valor.HasValue ? $"&{nombre}={valor.Value}" : "";

        private static string Rango(int seaId, DateTime desde, DateTime hasta) =>
            $"?seaId={seaId}&desde={Fecha(desde)}&hasta={Fecha(hasta)}";

        private static async Task<List<T>> Lista<T>(string url) =>
            (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>();

        /// <summary>Año lectivo que contiene la fecha; null si no hay.</summary>
        public async Task<AsistenciaAnioViewModel> AnioAsync(DateTime fecha) =>
            await ApiRequests.GetSingleAsync<AsistenciaAnioViewModel>($"{Base}AnioAsync?fecha={Fecha(fecha)}");

        public Task<List<AsistenciaPeriodoViewModel>> PeriodosAsync(int aniId) =>
            Lista<AsistenciaPeriodoViewModel>($"{Base}PeriodosAsync?aniId={aniId}");

        /// <summary>Secciones del año; con empId, solo las del docente.</summary>
        public Task<List<AsistenciaFiltroViewModel>> FiltrosAsync(int aniId, int? empId) =>
            Lista<AsistenciaFiltroViewModel>($"{Base}FiltrosAsync?aniId={aniId}" + Opcional("empId", empId));

        public Task<List<AsistenciaSeccionViewModel>> SeccionesAsync(int aniId, DateTime desde, DateTime hasta,
                                                                    int? nivId, int? cunId, int? seaId, int? empId) =>
            Lista<AsistenciaSeccionViewModel>($"{Base}SeccionesAsync?aniId={aniId}&desde={Fecha(desde)}&hasta={Fecha(hasta)}"
                + Opcional("nivId", nivId) + Opcional("cunId", cunId) + Opcional("seaId", seaId) + Opcional("empId", empId));

        /// <summary>null si la sección no existe.</summary>
        public async Task<AsistenciaSeccionInfoViewModel> SeccionInfoAsync(int seaId) =>
            await ApiRequests.GetSingleAsync<AsistenciaSeccionInfoViewModel>($"{Base}SeccionInfoAsync?seaId={seaId}");

        public Task<List<AsistenciaAlumnoViewModel>> AlumnosAsync(int seaId, DateTime desde, DateTime hasta) =>
            Lista<AsistenciaAlumnoViewModel>(Base + "AlumnosAsync" + Rango(seaId, desde, hasta));

        public Task<List<AsistenciaDiaViewModel>> DiasAsync(int seaId, DateTime desde, DateTime hasta) =>
            Lista<AsistenciaDiaViewModel>(Base + "DiasAsync" + Rango(seaId, desde, hasta));

        public Task<List<AsistenciaPlanillaViewModel>> PlanillaAsync(int seaId, DateTime desde, DateTime hasta) =>
            Lista<AsistenciaPlanillaViewModel>(Base + "PlanillaAsync" + Rango(seaId, desde, hasta));
    }
}
