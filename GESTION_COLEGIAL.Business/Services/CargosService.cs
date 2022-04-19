using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CargosService
    {
        public async Task<IEnumerable<CargoViewModel>> ListAsync()
        {
            string url = "Cargos/ListAsync";
            IEnumerable<CargoViewModel> apiUrl = await ApiRequests.ListAsync<CargoViewModel>(url);
            return apiUrl;
        }

        public async Task<CargoViewModel> Find(int id)
        {
            string url = "Cargos/FindAsync";
            CargoViewModel apiUrl = await ApiRequests.FindAsync<CargoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(CargoViewModel model)
        {
            string url = "Cargos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(CargoViewModel model)
        {
            string url = "Cargos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<CargoViewModel> Exist(string value)
        {
            string url = "Cargos/ExistAsync";
            return await ApiRequests.ExistAsync<CargoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Cargos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
