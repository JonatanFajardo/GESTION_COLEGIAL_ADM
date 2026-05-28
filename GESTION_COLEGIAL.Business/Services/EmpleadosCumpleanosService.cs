using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EmpleadosCumpleanosService
    {
        public async Task<List<dynamic>> ListAsync(int mes)
        {
            string url = $"EmpleadosCumpleanos/ListAsync?Mes={mes}";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }
    }
}
