using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class VacacionesAdmService
    {
        public async Task<List<dynamic>> ListAsync(string Emp_Id = null)
        {
            string url = "Vacaciones/ListAsync" + (Emp_Id != null ? $"?Emp_Id={Emp_Id}" : "");
            return await SendHttpClient.GetAsync<dynamic>(url);
        }

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Vacaciones/CreateAsync", model);

        public async Task<bool> ApproveAsync(dynamic model)
            => await SendHttpClient.PutAsync("Vacaciones/ApproveAsync", model);

        public async Task<bool> RemoveAsync(dynamic model)
            => await SendHttpClient.PutAsync("Vacaciones/RemoveAsync", model);
    }
}
