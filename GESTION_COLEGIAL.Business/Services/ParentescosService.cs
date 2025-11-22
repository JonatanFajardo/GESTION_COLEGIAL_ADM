using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de los parentescos.
    /// </summary>
    public class ParentescosService
    {
        public async Task<IEnumerable<ParentescoViewModel>> ListAsync()
        {
            string url = "Parentescos/ListAsync";
            IEnumerable<ParentescoViewModel> apiUrl = await ApiRequests.ListAsync<ParentescoViewModel>(url);
            return apiUrl;
        }

        public async Task<ParentescoViewModel> Find(int id)
        {
            string url = "Parentescos/FindAsync";
            ParentescoViewModel apiUrl = await ApiRequests.FindAsync<ParentescoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<ParentescoDetailViewModel> Detail(int id)
        {
            string url = "Parentescos/DetailAsync";
            ParentescoDetailViewModel apiUrl = await ApiRequests.FindAsync<ParentescoDetailViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ParentescoViewModel model)
        {
            string url = "Parentescos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(ParentescoViewModel model)
        {
            string url = "Parentescos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<ParentescoViewModel> Exist(string value)
        {
            string url = "Parentescos/ExistAsync";
            return await ApiRequests.ExistAsync<ParentescoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Parentescos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}