using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class SemestresService
    {
        public async Task<IEnumerable<SemestreViewModel>> ListAsync()
        {
            string url = "Semestres/ListAsync";
            IEnumerable<SemestreViewModel> apiUrl = await ApiRequests.ListAsync<SemestreViewModel>(url);
            return apiUrl;
        }

        public async Task<SemestreViewModel> Find(int id)
        {
            string url = "Semestres/FindAsync";
            SemestreViewModel apiUrl = await ApiRequests.FindAsync<SemestreViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(SemestreViewModel model)
        {
            string url = "Semestres/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(SemestreViewModel model)
        {
            string url = "Semestres/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<SemestreViewModel> Exist(string value)
        {
            string url = "Semestres/ExistAsync";
            return await ApiRequests.ExistAsync<SemestreViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Semestres/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}