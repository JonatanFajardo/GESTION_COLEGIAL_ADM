using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class TitulosService
    {
        public async Task<IEnumerable<TituloViewModel>> List()
        {
            string url = "Titulos/List";
            IEnumerable<TituloViewModel> apiUrl = await ApiRequests.List<TituloViewModel>(url);
            return apiUrl;
        }

        public async Task<TituloViewModel> Find(int id)
        {
            string url = "Titulos/Find";
            TituloViewModel apiUrl = await ApiRequests.Find<TituloViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(TituloViewModel model)
        {
            string url = "Titulos/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(TituloViewModel model)
        {
            string url = "Titulos/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<TituloViewModel> Exist(string value)
        {
            string url = "Titulos/Exist";
            return await ApiRequests.Exist<TituloViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Titulos/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
