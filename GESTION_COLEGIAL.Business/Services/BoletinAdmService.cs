using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class BoletinAdmService
    {
        // Endpoint: GET /api/v1/Boletin/FindAsync?Alu_Id=&Anio=
        public async Task<List<dynamic>> GetAsync(int Alu_Id, int Anio)
            => await SendHttpClient.GetAsync<dynamic>(
                $"Boletin/FindAsync?Alu_Id={Alu_Id}&Anio={Anio}");
    }
}
