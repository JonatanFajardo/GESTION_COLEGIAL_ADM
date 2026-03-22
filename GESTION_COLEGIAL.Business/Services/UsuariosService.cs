using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class UsuariosService
    {
        public async Task<IEnumerable<UsuarioViewModel>> ListAsync()
        {
            string url = "Usuarios/ListAsync";
            IEnumerable<UsuarioViewModel> apiUrl = await ApiRequests.ListAsync<UsuarioViewModel>(url);
            return apiUrl;
        }

        public async Task<UsuarioViewModel> Find(int id)
        {
            string url = "Usuarios/FindAsync";
            UsuarioViewModel apiUrl = await ApiRequests.FindAsync<UsuarioViewModel>(url, id);
            return apiUrl;
        }

        public async Task<UsuarioDetailViewModel> Detail(int id)
        {
            string url = "Usuarios/DetailAsync";
            UsuarioDetailViewModel apiUrl = await ApiRequests.FindAsync<UsuarioDetailViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(UsuarioViewModel model)
        {
            string url = "Usuarios/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(UsuarioViewModel model)
        {
            string url = "Usuarios/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<UsuarioViewModel> Exist(string value)
        {
            string url = "Usuarios/ExistAsync";
            return await ApiRequests.ExistAsync<UsuarioViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Usuarios/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        public async Task<IEnumerable<RolDropdownViewModel>> RolesDropdownAsync()
        {
            string url = "Usuarios/RolesDropdownAsync";
            IEnumerable<RolDropdownViewModel> apiUrl = await ApiRequests.ListAsync<RolDropdownViewModel>(url);
            return apiUrl;
        }
    }
}
