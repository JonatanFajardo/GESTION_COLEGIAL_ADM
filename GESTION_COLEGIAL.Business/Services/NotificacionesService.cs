using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NotificacionesService
    {
        public async Task<List<dynamic>> ListAsync(int usuId, bool soloNoLeidas = false)
        {
            string url = $"Notificaciones/ListAsync?usuId={usuId}&soloNoLeidas={soloNoLeidas}";
            return await SendHttpClient.GetAsync<dynamic>(url);
        }

        public async Task<bool> MarcarLeidaAsync(int notId)
        {
            string url = $"Notificaciones/MarcarLeidaAsync?notId={notId}";
            return await SendHttpClient.PutAsync(url, new { notId });
        }
    }
}
