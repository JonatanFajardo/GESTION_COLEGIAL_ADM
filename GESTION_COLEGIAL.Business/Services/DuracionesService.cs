using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DuracionesService
    {
        public async Task<IEnumerable<DuracionViewModel>> ListAsync()
        {
            string url = "Duraciones/ListAsync";
            IEnumerable<DuracionViewModel> apiUrl = await ApiRequests.ListAsync<DuracionViewModel>(url);
            return apiUrl;
        }

        public async Task<DuracionViewModel> Find(int id)
        {
            string url = "Duraciones/FindAsync";
            DuracionViewModel apiUrl = await ApiRequests.FindAsync<DuracionViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(DuracionViewModel model)
        {
            string url = "Duraciones/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(DuracionViewModel model)
        {
            string url = "Duraciones/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<DuracionViewModel> Exist(string value)
        {
            string url = "Duraciones/ExistAsync";
            return await ApiRequests.ExistAsync<DuracionViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Duraciones/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}