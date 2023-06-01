using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HorasService
    {
        public async Task<IEnumerable<HoraViewModel>> ListAsync()
        {
            string url = "Horas/ListAsync";
            IEnumerable<HoraViewModel> apiUrl = await ApiRequests.ListAsync<HoraViewModel>(url);
            return apiUrl;
        }

        public async Task<HoraViewModel> Find(int id)
        {
            string url = "Horas/FindAsync";
            HoraViewModel apiUrl = await ApiRequests.FindAsync<HoraViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(HoraViewModel model)
        {
            string url = "Horas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(HoraViewModel model)
        {
            string url = "Horas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<HoraViewModel> Exist(string value)
        {
            string url = "Horas/ExistAsync";
            return await ApiRequests.ExistAsync<HoraViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Horas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}