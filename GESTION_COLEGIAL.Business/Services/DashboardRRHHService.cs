using GESTION_COLEGIAL.Business.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DashboardRRHHService
    {
        public async Task<dynamic> Resumen()
        {
            string url = "DashboardRRHH/ResumenAsync";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
