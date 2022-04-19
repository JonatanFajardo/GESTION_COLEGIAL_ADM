using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DiasService
    {
        public async Task<IEnumerable<DiaViewModel>> ListAsync()
        {
            string url = "Diaes/ListAsync";
            IEnumerable<DiaViewModel> apiUrl = await ApiRequests.ListAsync<DiaViewModel>(url);
            return apiUrl;
        }

        public async Task<DiaViewModel> Find(int id)
        {
            string url = "Diaes/FindAsync";
            DiaViewModel apiUrl = await ApiRequests.FindAsync<DiaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(DiaViewModel model)
        {
            string url = "Diaes/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(DiaViewModel model)
        {
            string url = "Diaes/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<DiaViewModel> Exist(string value)
        {
            string url = "Diaes/ExistAsync";
            return await ApiRequests.ExistAsync<DiaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Diaes/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
