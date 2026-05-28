using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio para obtener el organigrama institucional.
    /// </summary>
    public class OrganigramaService
    {
        private const string _base = "Organigrama/";

        /// <summary>
        /// Obtiene la estructura jerárquica de empleados.
        /// </summary>
        /// <returns>Lista dinámica con empleados, cargos, departamentos y supervisores.</returns>
        public async Task<List<dynamic>> GetAsync()
        {
            string url = _base + "GetAsync";
            return await SendHttpClient.GetAsync<dynamic>(url) ?? new List<dynamic>();
        }
    }
}
