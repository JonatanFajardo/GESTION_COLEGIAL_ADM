using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class PlanClasesAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Hor_Id)
            => await SendHttpClient.GetAsync<dynamic>($"PlanClases/ListAsync?Hor_Id={Hor_Id}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("PlanClases/CreateAsync", model);

        public async Task<bool> EditAsync(dynamic model)
            => await SendHttpClient.PutAsync("PlanClases/EditAsync", model);

        public async Task<bool> DeleteAsync(int Pla_Id, int usuarioModifica)
            => await SendHttpClient.PostAsync("PlanClases/DeleteAsync", new { Pla_Id, UsuarioModifica = usuarioModifica });
    }
}
