using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AccountService
    {
        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model)
        {
            string url = "Account/Login";
            var response = await ApiRequests.PostAsyncWithResponse<LoginResponseViewModel>(url, model);
            return response;
        }

        public async Task<bool> LogoutAsync(int usuId)
        {
            string url = "Account/Logout";
            bool response = await ApiRequests.PostAsync(url, usuId);
            return response;
        }
    }
}
