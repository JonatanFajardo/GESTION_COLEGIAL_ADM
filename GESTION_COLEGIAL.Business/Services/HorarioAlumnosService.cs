using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class HorarioAlumnosService
    {
        public async Task<IEnumerable<HorarioAlumnoViewModel>> List()
        {
            string url = "HorarioAlumnos/List";
            IEnumerable<HorarioAlumnoViewModel> apiUrl = await ApiRequests.List<HorarioAlumnoViewModel>(url);
            return apiUrl;
        }

        public async Task<HorarioAlumnoViewModel> Find(int id)
        {
            string url = "HorarioAlumnos/Find";
            HorarioAlumnoViewModel apiUrl = await ApiRequests.Find<HorarioAlumnoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<HorarioAlumnoViewModel> Exist(string value)
        {
            string url = "HorarioAlumnos/Exist";
            return await ApiRequests.Exist<HorarioAlumnoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "HorarioAlumnos/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
