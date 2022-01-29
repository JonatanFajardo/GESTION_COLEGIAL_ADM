using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NotasService
    {
        public async Task<IEnumerable<NotaViewModel>> List()
        {
            string url = "Notas/List";
            IEnumerable<NotaViewModel> apiUrl = await ApiRequests.List<NotaViewModel>(url);
            return apiUrl;
        }

        public async Task<NotaViewModel> Find(int id)
        {
            string url = "Notas/Find";
            NotaViewModel apiUrl = await ApiRequests.Find<NotaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(NotaViewModel model)
        {
            string url = "Notas/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(NotaViewModel model)
        {
            string url = "Notas/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<NotaViewModel> Exist(string value)
        {
            string url = "Notas/Exist";
            return await ApiRequests.Exist<NotaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Notas/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
