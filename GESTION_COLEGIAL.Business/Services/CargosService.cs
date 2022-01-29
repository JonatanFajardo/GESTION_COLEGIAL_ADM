using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CargosService
    {
        public async Task<IEnumerable<CargoViewModel>> List()
        {
            string url = "Cargoses/List";
            IEnumerable<CargoViewModel> apiUrl = await ApiRequests.List<CargoViewModel>(url);
            return apiUrl;
        }

        public async Task<CargoViewModel> Find(int id)
        {
            string url = "Cargoses/Find";
            CargoViewModel apiUrl = await ApiRequests.Find<CargoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(CargoViewModel model)
        {
            string url = "Cargoses/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(CargoViewModel model)
        {
            string url = "Cargoses/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<CargoViewModel> Exist(string value)
        {
            string url = "Cargoses/Exist";
            return await ApiRequests.Exist<CargoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Cargoses/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
