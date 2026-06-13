using GESTION_COLEGIAL.Business.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DashboardSecretariaService
    {
        public async Task<dynamic> Resumen()
        {
            string url = "DashboardSecretaria/ResumenAsync";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
