using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AlumnosDirectorioService
    {
        public async Task<List<dynamic>> ListAsync(string Cur_Id = null, string Sec_Id = null)
        {
            string qs = string.Empty;
            if (Cur_Id != null) qs += $"&Cur_Id={Cur_Id}";
            if (Sec_Id != null) qs += $"&Sec_Id={Sec_Id}";
            string url = "AlumnosDirectorio/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }

        public async Task<List<dynamic>> CursosDropdown()
        {
            string url = "CursosNiveles/CursosNivelesDropdown";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }

        public async Task<List<dynamic>> SeccionesDropdown(string Cur_Id = null)
        {
            string qs = Cur_Id != null ? $"?id={Cur_Id}" : string.Empty;
            string url = "Secciones/SeccionesDropdown" + qs;
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
