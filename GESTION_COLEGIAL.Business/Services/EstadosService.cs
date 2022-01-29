using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EstadosService
    {
        public async Task<IEnumerable<EstadoViewModel>> List()
        {
            string url = "Estados/List";
            IEnumerable<EstadoViewModel> apiUrl = await ApiRequests.List<EstadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EstadoViewModel> Find(int id)
        {
            string url = "Estados/Find";
            EstadoViewModel apiUrl = await ApiRequests.Find<EstadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EstadoViewModel model)
        {
            string url = "Estados/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(EstadoViewModel model)
        {
            string url = "Estados/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<EstadoViewModel> Exist(string value)
        {
            string url = "Estados/Exist";
            return await ApiRequests.Exist<EstadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Estados/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
