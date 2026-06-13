using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ConfiguracionService
    {
        private const string _base = "Configuracion/";

        public async Task<List<dynamic>> ListAsync()
        {
            return await SendHttpClient.GetAsync<dynamic>(_base + "ListAsync") ?? new List<dynamic>();
        }

        public async Task<bool> UpdateAsync(object body)
        {
            return await SendHttpClient.PutAsync(_base + "UpdateAsync", body);
        }
    }
}
