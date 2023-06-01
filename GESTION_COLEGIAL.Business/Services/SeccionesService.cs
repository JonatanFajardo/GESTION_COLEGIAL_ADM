using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class SeccionesService
    {
        public async Task<IEnumerable<SeccionViewModel>> ListAsync()
        {
            string url = "Secciones/ListAsync";
            IEnumerable<SeccionViewModel> apiUrl = await ApiRequests.ListAsync<SeccionViewModel>(url);
            return apiUrl;
        }

        public async Task<SeccionViewModel> Find(int id)
        {
            string url = "Secciones/FindAsync";
            SeccionViewModel apiUrl = await ApiRequests.FindAsync<SeccionViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(SeccionViewModel model)
        {
            string url = "Secciones/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(SeccionViewModel model)
        {
            string url = "Secciones/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<SeccionViewModel> Exist(string value)
        {
            string url = "Secciones/ExistAsync";
            return await ApiRequests.ExistAsync<SeccionViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Secciones/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}