using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class BitacoraService
    {
        public async Task<List<dynamic>> ListAsync(string Usu_Id = null, string Bit_Tabla = null, string Top = null)
        {
            string qs = string.Empty;
            if (Usu_Id   != null) qs += $"&Usu_Id={Usu_Id}";
            if (Bit_Tabla!= null) qs += $"&Bit_Tabla={Bit_Tabla}";
            if (Top      != null) qs += $"&Top={Top}";
            string url = "Bitacora/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
