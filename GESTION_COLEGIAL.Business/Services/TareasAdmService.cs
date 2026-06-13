using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class TareasAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Hor_Id)
            => await SendHttpClient.GetAsync<dynamic>($"Tareas/ListAsync?Hor_Id={Hor_Id}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Tareas/CreateAsync", model);

        public async Task<bool> EditAsync(dynamic model)
            => await SendHttpClient.PutAsync("Tareas/EditAsync", model);

        public async Task<bool> DeleteAsync(int Tar_Id, int usuarioModifica)
            => await SendHttpClient.PostAsync("Tareas/DeleteAsync", new { Tar_Id, UsuarioModifica = usuarioModifica });
    }
}
