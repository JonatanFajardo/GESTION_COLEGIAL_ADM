using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CentroAnunciosService
    {
        // GET /api/v1/CentroAnuncios/ListAsync
        public async Task<List<dynamic>> ListAsync()
            => await SendHttpClient.GetAsync<dynamic>("CentroAnuncios/ListAsync");

        // POST /api/v1/CentroAnuncios/CreateAsync
        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("CentroAnuncios/CreateAsync", model);

        // POST /api/v1/CentroAnuncios/DeleteAsync
        public async Task<bool> DeleteAsync(dynamic model)
            => await SendHttpClient.PostAsync("CentroAnuncios/DeleteAsync", model);
    }
}
