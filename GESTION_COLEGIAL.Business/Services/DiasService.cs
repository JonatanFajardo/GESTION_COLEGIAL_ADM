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
            string url = "Dias/ListAsync";
            IEnumerable<DiaViewModel> apiUrl = await ApiRequests.ListAsync<DiaViewModel>(url);
            return apiUrl;
        }

        public async Task<DiaViewModel> Find(int id)
        {
            string url = "Dias/FindAsync";
            DiaViewModel apiUrl = await ApiRequests.FindAsync<DiaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(DiaViewModel model)
        {
            string url = "Dias/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(DiaViewModel model)
        {
            string url = "Dias/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<DiaViewModel> Exist(string value)
        {
            string url = "Dias/ExistAsync";
            return await ApiRequests.ExistAsync<DiaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Dias/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
