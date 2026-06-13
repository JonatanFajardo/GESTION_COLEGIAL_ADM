using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ArqueosCajaAdmService
    {
        public async Task<dynamic> LastAsync(int Usu_Id)
            => await SendHttpClient.GetSingleAsync<dynamic>($"ArqueosCaja/LastByUsuarioAsync?Usu_Id={Usu_Id}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("ArqueosCaja/CreateAsync", model);
    }
}
