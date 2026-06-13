using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AsistenciaAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Hor_Id, string Fecha)
            => await SendHttpClient.GetAsync<dynamic>($"Asistencia/ListAsync?Hor_Id={Hor_Id}&Fecha={Fecha}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Asistencia/CreateAsync", model);

        public async Task<bool> EditAsync(dynamic model)
            => await SendHttpClient.PutAsync("Asistencia/EditAsync", model);
    }
}
