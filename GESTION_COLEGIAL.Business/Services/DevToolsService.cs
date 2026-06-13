using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DevToolsService
    {
        private const string _base = "DevTools/";

        public async Task<List<dynamic>> DetectarAsync(int? anioObjetivo)
        {
            string url = _base + "DetectarAsync";
            if (anioObjetivo.HasValue) url += "?AnioObjetivo=" + anioObjetivo.Value;
            return await SendHttpClient.GetAsync<dynamic>(url) ?? new List<dynamic>();
        }

        public async Task<List<dynamic>> PasoAsync(string tabla, int delta)
        {
            string url = _base + $"PasoAsync?Tabla={System.Uri.EscapeDataString(tabla)}&Delta={delta}";
            return await SendHttpClient.PostAsyncWithResponse<List<dynamic>>(url, new { }) ?? new List<dynamic>();
        }

        public async Task<List<dynamic>> EstadoBDAsync()
        {
            return await SendHttpClient.GetAsync<dynamic>(_base + "EstadoBDAsync") ?? new List<dynamic>();
        }

        public async Task<List<dynamic>> RolesAsync()
        {
            return await SendHttpClient.GetAsync<dynamic>(_base + "RolesAsync") ?? new List<dynamic>();
        }
    }
}
