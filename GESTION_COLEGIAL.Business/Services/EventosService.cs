using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EventosService
    {
        public async Task<IEnumerable<ModalidadViewModel>> ListAsync()
        {
            string url = "Eventos/ListAsync";
            IEnumerable<ModalidadViewModel> apiUrl = await ApiRequests.ListAsync<ModalidadViewModel>(url);
            return apiUrl;
        }

        public async Task<ModalidadViewModel> Find(int id)
        {
            string url = "Eventos/FindAsync";
            ModalidadViewModel apiUrl = await ApiRequests.FindAsync<ModalidadViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ModalidadViewModel model)
        {
            string url = "Eventos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(ModalidadViewModel model)
        {
            string url = "Eventos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<ModalidadViewModel> Exist(string value)
        {
            string url = "Eventos/ExistAsync";
            return await ApiRequests.ExistAsync<ModalidadViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Eventos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}