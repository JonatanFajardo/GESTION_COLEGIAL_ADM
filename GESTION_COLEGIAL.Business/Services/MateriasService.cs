using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MateriasService
    {
        public async Task<IEnumerable<MateriaViewModel>> ListAsync()
        {
            string url = "Materias/ListAsync";
            IEnumerable<MateriaViewModel> apiUrl = await ApiRequests.ListAsync<MateriaViewModel>(url);
            return apiUrl;
        }

        public async Task<MateriaViewModel> Find(int id)
        {
            string url = "Materias/FindAsync";
            MateriaViewModel apiUrl = await ApiRequests.FindAsync<MateriaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(MateriaViewModel model)
        {
            string url = "Materias/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(MateriaViewModel model)
        {
            string url = "Materias/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<MateriaViewModel> Exist(string value)
        {
            string url = "Materias/ExistAsync";
            return await ApiRequests.ExistAsync<MateriaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Materias/Remove";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
