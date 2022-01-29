using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ParcialesService
    {
        public async Task<IEnumerable<ParcialViewModel>> List()
        {
            string url = "Modalidades/List";
            IEnumerable<ParcialViewModel> apiUrl = await ApiRequests.List<ParcialViewModel>(url);
            return apiUrl;
        }

        public async Task<ParcialViewModel> Find(int id)
        {
            string url = "Modalidades/Find";
            ParcialViewModel apiUrl = await ApiRequests.Find<ParcialViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ParcialViewModel model)
        {
            string url = "Modalidades/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(ParcialViewModel model)
        {
            string url = "Modalidades/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<ParcialViewModel> Exist(string value)
        {
            string url = "Modalidades/Exist";
            return await ApiRequests.Exist<ParcialViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Modalidades/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
