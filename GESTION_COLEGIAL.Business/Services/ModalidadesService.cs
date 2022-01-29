using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ModalidadesService
    {
        public async Task<IEnumerable<ModalidadViewModel>> List()
        {
            string url = "Modalidades/List";
            IEnumerable<ModalidadViewModel> apiUrl = await ApiRequests.List<ModalidadViewModel>(url);
            return apiUrl;
        }

        public async Task<ModalidadViewModel> Find(int id)
        {
            string url = "Modalidades/Find";
            ModalidadViewModel apiUrl = await ApiRequests.Find<ModalidadViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ModalidadViewModel model)
        {
            string url = "Modalidades/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(ModalidadViewModel model)
        {
            string url = "Modalidades/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<ModalidadViewModel> Exist(string value)
        {
            string url = "Modalidades/Exist";
            return await ApiRequests.Exist<ModalidadViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Modalidades/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
