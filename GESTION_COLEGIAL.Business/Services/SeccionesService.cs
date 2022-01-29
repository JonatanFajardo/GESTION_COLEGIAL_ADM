using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class SeccionesService
    {
        public async Task<IEnumerable<SeccionViewModel>> List()
        {
            string url = "Secciones/List";
            IEnumerable<SeccionViewModel> apiUrl = await ApiRequests.List<SeccionViewModel>(url);
            return apiUrl;
        }

        public async Task<SeccionViewModel> Find(int id)
        {
            string url = "Secciones/Find";
            SeccionViewModel apiUrl = await ApiRequests.Find<SeccionViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(SeccionViewModel model)
        {
            string url = "Secciones/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(SeccionViewModel model)
        {
            string url = "Secciones/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<SeccionViewModel> Exist(string value)
        {
            string url = "Secciones/Exist";
            return await ApiRequests.Exist<SeccionViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Secciones/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
