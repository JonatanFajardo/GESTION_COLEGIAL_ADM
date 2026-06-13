using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener el estado de resultados por período.
    /// </summary>
    public class EstadoResultadosService
    {
        private const string _base = "EstadoResultados/";

        /// <summary>
        /// Obtiene el estado de resultados del año y mes indicados.
        /// </summary>
        /// <param name="anio">Año a consultar.</param>
        /// <param name="mes">Mes a consultar (1-12).</param>
        /// <returns>Lista dinámica de conceptos con montos.</returns>
        public async Task<List<dynamic>> GetAsync(int anio, int mes)
        {
            string url = _base + $"FindAsync?Anio={anio}&Mes={mes}";
            return await SendHttpClient.GetAsync<dynamic>(url) ?? new List<dynamic>();
        }
    }
}
