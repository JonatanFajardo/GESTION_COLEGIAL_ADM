using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CumplimientoDocumentosService
    {
        /// <summary>
        /// Retorna el JSON crudo de la API envuelto en { "data": [...] }
        /// para evitar la corrupción que produce JavaScriptSerializer al
        /// serializar objetos JObject de Newtonsoft.
        /// </summary>
        public async Task<string> ListRawAsync(int? Emp_Id = null)
        {
            string qs = string.Empty;
            if (Emp_Id.HasValue) qs += $"&Emp_Id={Emp_Id.Value}";
            string url = "CumplimientoDocumentos/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            string arrayJson = await SendHttpClient.GetRawAsync(url);
            return "{\"data\":" + arrayJson + "}";
        }
    }
}
