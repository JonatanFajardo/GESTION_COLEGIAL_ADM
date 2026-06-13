using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class TopDeudoresService
    {
        public async Task<List<dynamic>> ListAsync()
        {
            string qs = string.Empty;
            string url = "TopDeudores/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
