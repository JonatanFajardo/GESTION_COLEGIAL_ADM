using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class SemestresService
    {
        public async Task<IEnumerable<SemestreViewModel>> List()
        {
            string url = "Semestres/List";
            IEnumerable<SemestreViewModel> apiUrl = await ApiRequests.List<SemestreViewModel>(url);
            return apiUrl;
        }

        public async Task<SemestreViewModel> Find(int id)
        {
            string url = "Semestres/Find";
            SemestreViewModel apiUrl = await ApiRequests.Find<SemestreViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(SemestreViewModel model)
        {
            string url = "Semestres/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(SemestreViewModel model)
        {
            string url = "Semestres/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<SemestreViewModel> Exist(string value)
        {
            string url = "Semestres/Exist";
            return await ApiRequests.Exist<SemestreViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Semestres/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
