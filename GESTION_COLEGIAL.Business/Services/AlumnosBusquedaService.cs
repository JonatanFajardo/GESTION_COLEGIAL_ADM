using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AlumnosBusquedaService
    {
        public async Task<List<dynamic>> ListAsync(string Termino = null)
        {
            string qs = string.Empty;
            if (Termino != null) qs += $"&Termino={Termino}";
            string url = "AlumnosBusqueda/SearchAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
