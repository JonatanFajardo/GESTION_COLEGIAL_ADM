using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class BuscadorService
    {
        public async Task<List<dynamic>> SearchAsync(string q = null)
        {
            string qs = string.Empty;
            if (q != null) qs += $"&q={q}";
            string url = "Buscador/SearchAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
