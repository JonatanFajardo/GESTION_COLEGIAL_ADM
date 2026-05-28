using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class PiramideMatriculaService
    {
        public async Task<List<dynamic>> ListAsync(string Anio = null)
        {
            string qs = string.Empty;
            if (Anio != null) qs += $"&Anio={Anio}";
            string url = "PiramideMatricula/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
