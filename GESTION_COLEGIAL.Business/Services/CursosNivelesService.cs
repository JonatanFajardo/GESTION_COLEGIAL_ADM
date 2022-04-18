using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CursosNivelesService
    {
        public async Task<IEnumerable<CursoNivelViewModel>> ListAsync()
        {
            string url = "CursosNiveles/ListAsync";
            IEnumerable<CursoNivelViewModel> apiUrl = await ApiRequests.ListAsync<CursoNivelViewModel>(url);
            return apiUrl;
        }

        public async Task<CursoNivelViewModel> Find(int id)
        {
            string url = "CursosNiveles/FindAsync";
            CursoNivelViewModel apiUrl = await ApiRequests.FindAsync<CursoNivelViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<CursoNivelViewModel> Exist(string value)
        {
            string url = "CursosNiveles/ExistAsync";
            return await ApiRequests.ExistAsync<CursoNivelViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "CursosNiveles/Remove";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
