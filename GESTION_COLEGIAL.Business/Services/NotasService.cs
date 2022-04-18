using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NotasService
    {
        public async Task<IEnumerable<NotaViewModel>> ListAsync()
        {
            string url = "Notas/ListAsync";
            IEnumerable<NotaViewModel> apiUrl = await ApiRequests.ListAsync<NotaViewModel>(url);
            return apiUrl;
        }

        public async Task<NotaViewModel> Find(int id)
        {
            string url = "Notas/FindAsync";
            NotaViewModel apiUrl = await ApiRequests.FindAsync<NotaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(NotaViewModel model)
        {
            string url = "Notas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(NotaViewModel model)
        {
            string url = "Notas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<NotaViewModel> Exist(string value)
        {
            string url = "Notas/ExistAsync";
            return await ApiRequests.ExistAsync<NotaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Notas/Remove";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
