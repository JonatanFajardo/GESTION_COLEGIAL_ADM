using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MisAlumnosAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Hor_Id)
            => await SendHttpClient.GetAsync<dynamic>($"MisAlumnos/ListAsync?Hor_Id={Hor_Id}");
    }
}
