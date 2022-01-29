using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CursosNivelesService
    {
        public async Task<IEnumerable<CursoNivelViewModel>> List()
        {
            string url = "CursosNiveles/List";
            IEnumerable<CursoNivelViewModel> apiUrl = await ApiRequests.List<CursoNivelViewModel>(url);
            return apiUrl;
        }

        public async Task<CursoNivelViewModel> Find(int id)
        {
            string url = "CursosNiveles/Find";
            CursoNivelViewModel apiUrl = await ApiRequests.Find<CursoNivelViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<CursoNivelViewModel> Exist(string value)
        {
            string url = "CursosNiveles/Exist";
            return await ApiRequests.Exist<CursoNivelViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "CursosNiveles/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
