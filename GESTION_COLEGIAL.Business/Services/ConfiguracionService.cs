using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models.Graduacion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Cliente del endpoint /Configuracion del API. Expone helpers
    /// tipados para los valores conocidos del dominio.
    /// </summary>
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

        public async Task<decimal> GetNotaMinimaAprobacionAsync()
        {
            var result = await SendHttpClient.GetSingleAsync<ConfiguracionValorViewModel>(_base + "NotaMinimaAprobacionAsync");
            return result?.Valor ?? 70m;
        }

        public async Task<bool> SetNotaMinimaAprobacionAsync(decimal valor, int usuarioId)
        {
            string url = string.Format("{0}NotaMinimaAprobacionAsync?valor={1}&usuarioId={2}",
                _base, valor, usuarioId);
            return await SendHttpClient.PutAsync(url, new { valor, usuarioId });
        }
    }
}
