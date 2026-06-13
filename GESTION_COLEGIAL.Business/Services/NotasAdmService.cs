using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NotasAdmService
    {
        // Endpoint nuevo: GET /api/v1/Notas/ListAsync?Hor_Id=&Par_Id=
        public async Task<List<dynamic>> ListAsync(int Hor_Id, int Par_Id)
            => await SendHttpClient.GetAsync<dynamic>($"Notas/ListAsync?Hor_Id={Hor_Id}&Par_Id={Par_Id}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Notas/CreateAsync", model);

        public async Task<bool> EditAsync(dynamic model)
            => await SendHttpClient.PutAsync("Notas/EditAsync", model);
    }
}
