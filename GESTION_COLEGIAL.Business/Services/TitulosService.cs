using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class TitulosService
    {
        public async Task<IEnumerable<TituloViewModel>> ListAsync()
        {
            string url = "Titulos/ListAsync";
            IEnumerable<TituloViewModel> apiUrl = await ApiRequests.ListAsync<TituloViewModel>(url);
            return apiUrl;
        }

        public async Task<TituloViewModel> Find(int id)
        {
            string url = "Titulos/FindAsync";
            TituloViewModel apiUrl = await ApiRequests.FindAsync<TituloViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(TituloViewModel model)
        {
            string url = "Titulos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(TituloViewModel model)
        {
            string url = "Titulos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<TituloViewModel> Exist(string value)
        {
            string url = "Titulos/ExistAsync";
            return await ApiRequests.ExistAsync<TituloViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Titulos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}