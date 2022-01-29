using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DiasService
    {
        public async Task<IEnumerable<DiaViewModel>> List()
        {
            string url = "Diaes/List";
            IEnumerable<DiaViewModel> apiUrl = await ApiRequests.List<DiaViewModel>(url);
            return apiUrl;
        }

        public async Task<DiaViewModel> Find(int id)
        {
            string url = "Diaes/Find";
            DiaViewModel apiUrl = await ApiRequests.Find<DiaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(DiaViewModel model)
        {
            string url = "Diaes/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(DiaViewModel model)
        {
            string url = "Diaes/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<DiaViewModel> Exist(string value)
        {
            string url = "Diaes/Exist";
            return await ApiRequests.Exist<DiaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Diaes/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
