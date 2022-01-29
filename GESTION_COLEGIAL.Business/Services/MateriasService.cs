using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class MateriasService
    {
        public async Task<IEnumerable<MateriaViewModel>> List()
        {
            string url = "Materias/List";
            IEnumerable<MateriaViewModel> apiUrl = await ApiRequests.List<MateriaViewModel>(url);
            return apiUrl;
        }

        public async Task<MateriaViewModel> Find(int id)
        {
            string url = "Materias/Find";
            MateriaViewModel apiUrl = await ApiRequests.Find<MateriaViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(MateriaViewModel model)
        {
            string url = "Materias/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(MateriaViewModel model)
        {
            string url = "Materias/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<MateriaViewModel> Exist(string value)
        {
            string url = "Materias/Exist";
            return await ApiRequests.Exist<MateriaViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Materias/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
