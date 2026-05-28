using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MensajesService
    {
        public async Task<List<dynamic>> ListAsync(string Alu_Id = null)
        {
            string url = "Mensajes/ListAsync" + (Alu_Id != null ? $"?Alu_Id={Alu_Id}" : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Mensajes/CreateAsync", model);

        public async Task<bool> DeleteAsync(dynamic model)
            => await SendHttpClient.PostAsync("Mensajes/DeleteAsync", model);
    }
}
