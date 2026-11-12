using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio de Matrícula por Año + Promociones (consume la API externa MatriculaAnio).
    /// Capa multi-año — expediente/transcript y paso de alumnos entre años.
    /// </summary>
    public class MatriculaAnioService
    {
        /// <summary>Alumnos matriculados en una sección-año (filtra por ?value={seaId}).</summary>
        public async Task<IEnumerable<MatriculaAnioViewModel>> ListBySeccionAsync(int seaId)
        {
            string url = "MatriculaAnio/ListBySeccionAsync?value=" + seaId;
            return await ApiRequests.ListAsync<MatriculaAnioViewModel>(url);
        }

        /// <summary>Historial de matrícula del alumno por año (transcript); ?value={aluId}.</summary>
        public async Task<IEnumerable<MatriculaAnioViewModel>> ExpedienteAsync(int aluId)
        {
            string url = "MatriculaAnio/ExpedienteAsync?value=" + aluId;
            return await ApiRequests.ListAsync<MatriculaAnioViewModel>(url);
        }

        /// <summary>Matricula (o reactiva) al alumno en un año/sección; devuelve el Id.</summary>
        public async Task<int> Enroll(MatriculaAnioViewModel model)
        {
            string url = "MatriculaAnio/EnrollAsync";
            return await ApiRequests.PostAsyncWithResponse<int>(url, model);
        }

        /// <summary>Promueve a los alumnos activos de una sección-año a la del año destino; devuelve el conteo.</summary>
        public async Task<int> Promover(int anioOrigen, int seccionOrigen, int anioDestino, int seccionDestino)
        {
            string url = "MatriculaAnio/PromoverAsync"
                + "?anioOrigen=" + anioOrigen
                + "&seccionOrigen=" + seccionOrigen
                + "&anioDestino=" + anioDestino
                + "&seccionDestino=" + seccionDestino;
            return await ApiRequests.PostAsyncWithResponse<int>(url, new { });
        }

        /// <summary>Grupos de ubicación de los alumnos del año frente a las secciones del año e inscritos (aniId null = año activo).</summary>
        public async Task<IEnumerable<ResumenInscripcionViewModel>> ResumenInscripcionAsync(int? aniId)
        {
            string url = "MatriculaAnio/ResumenInscripcionAsync" + (aniId.HasValue ? "?value=" + aniId.Value : "");
            return await ApiRequests.ListAsync<ResumenInscripcionViewModel>(url);
        }

        /// <summary>Crea las secciones del año que faltan según la ubicación de los alumnos (aniId null = año activo).</summary>
        public async Task<ResultadoInscripcionViewModel> CrearSeccionesDesdeUbicacionAsync(int? aniId, int? aluId, int usuario)
        {
            string url = "MatriculaAnio/CrearSeccionesDesdeUbicacionAsync?usuario=" + usuario
                + (aniId.HasValue ? "&aniId=" + aniId.Value : "")
                + (aluId.HasValue ? "&aluId=" + aluId.Value : "");
            return await ApiRequests.PostAsyncWithResponse<ResultadoInscripcionViewModel>(url, new { });
        }

        /// <summary>Inscribe en el año (en lote, o solo aluId) según la ubicación de los alumnos (aniId null = año activo).</summary>
        public async Task<ResultadoInscripcionViewModel> InscribirDesdeUbicacionAsync(int? aniId, int? aluId, bool crearSecciones, bool activarEstado, int usuario)
        {
            string url = "MatriculaAnio/InscribirDesdeUbicacionAsync?usuario=" + usuario
                + "&crearSecciones=" + (crearSecciones ? "true" : "false")
                + "&activarEstado=" + (activarEstado ? "true" : "false")
                + (aniId.HasValue ? "&aniId=" + aniId.Value : "")
                + (aluId.HasValue ? "&aluId=" + aluId.Value : "");
            return await ApiRequests.PostAsyncWithResponse<ResultadoInscripcionViewModel>(url, new { });
        }
    }
}
