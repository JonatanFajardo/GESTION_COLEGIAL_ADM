using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HorasService
    {
        public async Task<IEnumerable<HoraViewModel>> List()
        {
            string url = "Horas/List";
            IEnumerable<HoraViewModel> apiUrl = await ApiRequests.List<HoraViewModel>(url);
            return apiUrl;
        }

        public async Task<HoraViewModel> Find(int id)
        {
            string url = "Horas/Find";
            HoraViewModel apiUrl = await ApiRequests.Find<HoraViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(HoraViewModel model)
        {
            string url = "Horas/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(HoraViewModel model)
        {
            string url = "Horas/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<HoraViewModel> Exist(string value)
        {
            string url = "Horas/Exist";
            return await ApiRequests.Exist<HoraViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Horas/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
