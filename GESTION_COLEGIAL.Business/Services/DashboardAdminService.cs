using GESTION_COLEGIAL.Business.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DashboardAdminService
    {
        public async Task<dynamic> Resumen()
        {
            string url = "DashboardAdmin/ResumenAsync";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
