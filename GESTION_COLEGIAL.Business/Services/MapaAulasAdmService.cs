using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MapaAulasAdmService
    {
        public async Task<List<dynamic>> GetAsync()
            => await SendHttpClient.GetAsync<dynamic>("MapaAulas/ListAsync");
    }
}
