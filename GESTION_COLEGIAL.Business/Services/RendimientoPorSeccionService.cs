using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class RendimientoPorSeccionService
    {
        public async Task<List<dynamic>> ListAsync(int anio, int parId)
        {
            string url = $"RendimientoPorSeccion/ListAsync?Anio={anio}&Par_Id={parId}";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
