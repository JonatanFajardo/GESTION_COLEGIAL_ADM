using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ParcialesService
    {
        public async Task<IEnumerable<ParcialViewModel>> ListAsync()
        {
            string url = "Modalidades/ListAsync";
            IEnumerable<ParcialViewModel> apiUrl = await ApiRequests.ListAsync<ParcialViewModel>(url);
            return apiUrl;
        }

        public async Task<ParcialViewModel> Find(int id)
        {
            string url = "Modalidades/FindAsync";
            ParcialViewModel apiUrl = await ApiRequests.FindAsync<ParcialViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ParcialViewModel model)
        {
            string url = "Modalidades/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(ParcialViewModel model)
        {
            string url = "Modalidades/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<ParcialViewModel> Exist(string value)
        {
            string url = "Modalidades/ExistAsync";
            return await ApiRequests.ExistAsync<ParcialViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Modalidades/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
