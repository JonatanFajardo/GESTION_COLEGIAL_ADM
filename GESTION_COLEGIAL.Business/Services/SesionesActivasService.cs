using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class SesionesActivasService
    {
        public async Task<List<dynamic>> ListAsync()
        {
            string url = "SesionesActivas/ListAsync";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
