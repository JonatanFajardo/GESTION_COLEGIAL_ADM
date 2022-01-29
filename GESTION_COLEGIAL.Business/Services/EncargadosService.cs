using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EncargadosService
    {
        public async Task<IEnumerable<EncargadoViewModel>> List()
        {
            string url = "Encargados/List";
            IEnumerable<EncargadoViewModel> apiUrl = await ApiRequests.List<EncargadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EncargadoViewModel> Find(int id)
        {
            string url = "Encargados/Find";
            EncargadoViewModel apiUrl = await ApiRequests.Find<EncargadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EncargadoViewModel model)
        {
            string url = "Encargados/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(EncargadoViewModel model)
        {
            string url = "Encargados/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<EncargadoViewModel> Exist(string value)
        {
            string url = "Encargados/Exist";
            return await ApiRequests.Exist<EncargadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Encargados/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
