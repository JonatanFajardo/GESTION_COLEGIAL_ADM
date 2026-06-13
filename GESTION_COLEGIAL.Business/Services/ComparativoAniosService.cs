using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ComparativoAniosService
    {
        public async Task<List<dynamic>> ListAsync(int anioBase, int anioComparacion)
        {
            string url = $"ComparativoAnios/ListAsync?AnioBase={anioBase}&AnioComparacion={anioComparacion}";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
