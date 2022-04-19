using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EstadosService
    {
        public async Task<IEnumerable<EstadoViewModel>> ListAsync()
        {
            string url = "Estados/ListAsync";
            IEnumerable<EstadoViewModel> apiUrl = await ApiRequests.ListAsync<EstadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EstadoViewModel> Find(int id)
        {
            string url = "Estados/FindAsync";
            EstadoViewModel apiUrl = await ApiRequests.FindAsync<EstadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EstadoViewModel model)
        {
            string url = "Estados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(EstadoViewModel model)
        {
            string url = "Estados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<EstadoViewModel> Exist(string value)
        {
            string url = "Estados/ExistAsync";
            return await ApiRequests.ExistAsync<EstadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Estados/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
