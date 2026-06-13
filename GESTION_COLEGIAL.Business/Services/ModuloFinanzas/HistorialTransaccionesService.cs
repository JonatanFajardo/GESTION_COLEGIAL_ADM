using GESTION_COLEGIAL.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Servicio para obtener el historial de transacciones financieras.
    /// </summary>
    public class HistorialTransaccionesService
    {
        /// <summary>
        /// Obtiene el historial de transacciones aplicando filtros opcionales.
        /// </summary>
        /// <param name="aluId">Identificador del alumno (opcional).</param>
        /// <param name="fechaDesde">Fecha de inicio del rango (opcional).</param>
        /// <param name="fechaHasta">Fecha de fin del rango (opcional).</param>
        /// <returns>Lista dinámica de transacciones.</returns>
        public async Task<List<dynamic>> ListAsync(int? aluId = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            string url = "HistorialTransacciones/ListAsync?";

            if (aluId.HasValue)
                url += $"Alu_Id={aluId.Value}&";

            if (fechaDesde.HasValue)
                url += $"FechaDesde={fechaDesde.Value:yyyy-MM-dd}&";

            if (fechaHasta.HasValue)
                url += $"FechaHasta={fechaHasta.Value:yyyy-MM-dd}&";

            url = url.TrimEnd('&').TrimEnd('?');

            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
