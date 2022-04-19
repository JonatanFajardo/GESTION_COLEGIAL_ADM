using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NivelesEducativosService
    {
        public async Task<IEnumerable<NivelEducativoViewModel>> ListAsync()
        {
            string url = "NivelesEducativos/ListAsync";
            IEnumerable<NivelEducativoViewModel> apiUrl = await ApiRequests.ListAsync<NivelEducativoViewModel>(url);
            return apiUrl;
        }

        public async Task<NivelEducativoViewModel> Find(int id)
        {
            string url = "NivelesEducativos/FindAsync";
            NivelEducativoViewModel apiUrl = await ApiRequests.FindAsync<NivelEducativoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<NivelEducativoViewModel> Exist(string value)
        {
            string url = "NivelesEducativos/ExistAsync";
            return await ApiRequests.ExistAsync<NivelEducativoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "NivelesEducativos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
