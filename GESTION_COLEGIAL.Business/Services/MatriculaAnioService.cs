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
    }
}
