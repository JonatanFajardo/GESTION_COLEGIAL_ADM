using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HorarioAlumnosService
    {
        public async Task<IEnumerable<HorarioAlumnoViewModel>> ListAsync()
        {
            string url = "HorarioAlumnos/ListAsync";
            IEnumerable<HorarioAlumnoViewModel> apiUrl = await ApiRequests.ListAsync<HorarioAlumnoViewModel>(url);
            return apiUrl;
        }

        public async Task<HorarioAlumnoViewModel> Find(int id)
        {
            string url = "HorarioAlumnos/FindAsync";
            HorarioAlumnoViewModel apiUrl = await ApiRequests.FindAsync<HorarioAlumnoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<HorarioAlumnoViewModel> Exist(string value)
        {
            string url = "HorarioAlumnos/ExistAsync";
            return await ApiRequests.ExistAsync<HorarioAlumnoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "HorarioAlumnos/Remove";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
