using GESTION_COLEGIAL.Business.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DashboardDocenteService
    {
        public async Task<dynamic> Resumen()
        {
            string url = "DashboardDocente/ResumenAsync";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
