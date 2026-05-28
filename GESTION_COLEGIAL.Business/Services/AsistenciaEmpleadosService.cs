using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AsistenciaEmpleadosService
    {
        public async Task<string> ListRawAsync(string Emp_Id = null, string Fecha = null)
        {
            string qs = string.Empty;
            if (!string.IsNullOrEmpty(Emp_Id)) qs += $"&Emp_Id={Emp_Id}";
            if (!string.IsNullOrEmpty(Fecha))  qs += $"&Fecha={Fecha}";
            string url = "AsistenciaEmpleados/ListAsync" + (qs.Length > 0 ? "?" + qs.TrimStart('&') : "");
            string arrayJson = await SendHttpClient.GetRawAsync(url);
            return "{\"data\":" + arrayJson + "}";
        }

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("AsistenciaEmpleados/CreateAsync", model);
    }
}
