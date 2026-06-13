using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class KPIsFinancierosService
    {
        public async Task<dynamic> FindAsync(int anio, int? mes)
        {
            string url = $"KPIsFinancieros/FindAsync?Anio={anio}";
            if (mes.HasValue)
                url += $"&Mes={mes.Value}";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
