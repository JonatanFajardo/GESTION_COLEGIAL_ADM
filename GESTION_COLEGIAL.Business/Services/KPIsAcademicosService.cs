using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class KPIsAcademicosService
    {
        public async Task<dynamic> FindAsync(int anio)
        {
            string url = $"KPIsAcademicos/FindAsync?Anio={anio}";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
