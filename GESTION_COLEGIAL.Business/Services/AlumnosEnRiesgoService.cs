using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AlumnosEnRiesgoService
    {
        public async Task<List<dynamic>> ListAsync(string PromedioMinimo = null, string Anio = null)
        {
            string qs = string.Empty;
            if (PromedioMinimo != null) qs += $"&PromedioMinimo={PromedioMinimo}";
            if (Anio != null) qs += $"&Anio={Anio}";
            string url = "AlumnosEnRiesgo/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
