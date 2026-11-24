using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AccountService
    {
        /// <summary>
        /// Inicio de sesión. La API responde 200 también cuando no deja entrar (Success = false y un
        /// mensaje genérico); la IP del navegador va para el límite de intentos.
        /// </summary>
        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model, string ipCliente)
        {
            string url = "Account/Login";
            var datos = new { model.Username, model.Password, model.RememberMe, IpCliente = ipCliente };
            var response = await ApiRequests.PostAsyncWithResponse<LoginResponseViewModel>(url, datos);
            return response;
        }

        /// <summary>Cambio de la propia contraseña (la API toma el usuario de la sesión firmada).</summary>
        public async Task<Models.RespuestaApi> CambiarContrasenaAsync(int usuId, string actual, string nueva, string ipCliente)
        {
            return await ApiRequests.PostConMensajeAsync("Account/CambiarContrasena", new
            {
                Usu_Id = usuId,
                ContrasenaActual = actual,
                ContrasenaNueva = nueva,
                IpCliente = ipCliente
            });
        }

        public async Task<bool> LogoutAsync(int usuId)
        {
            string url = "Account/Logout";
            bool response = await ApiRequests.PostAsync(url, usuId);
            return response;
        }
    }
}
