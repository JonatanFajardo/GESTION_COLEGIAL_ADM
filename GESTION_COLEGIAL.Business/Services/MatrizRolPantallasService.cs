using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MatrizRolPantallasService
    {
        public async Task<List<dynamic>> GetAsync()
        {
            string url = "MatrizRolPantallas/GetAsync";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
