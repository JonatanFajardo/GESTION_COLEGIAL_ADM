using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class PerfilService
    {
        public async Task<dynamic> FindAsync(int usuId)
        {
            string url = $"Perfil/FindAsync?usuId={usuId}";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}
