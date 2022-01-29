using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DuracionesService
    {
        public async Task<IEnumerable<DuracionViewModel>> List()
        {
            string url = "Duraciones/List";
            IEnumerable<DuracionViewModel> apiUrl = await ApiRequests.List<DuracionViewModel>(url);
            return apiUrl;
        }

        public async Task<DuracionViewModel> Find(int id)
        {
            string url = "Duraciones/Find";
            DuracionViewModel apiUrl = await ApiRequests.Find<DuracionViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(DuracionViewModel model)
        {
            string url = "Duraciones/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(DuracionViewModel model)
        {
            string url = "Duraciones/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<DuracionViewModel> Exist(string value)
        {
            string url = "Duraciones/Exist";
            return await ApiRequests.Exist<DuracionViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Duraciones/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}