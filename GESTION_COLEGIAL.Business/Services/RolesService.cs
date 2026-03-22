using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class RolesService
    {
        public async Task<IEnumerable<RolViewModel>> ListAsync()
        {
            string url = "Roles/ListAsync";
            return await ApiRequests.ListAsync<RolViewModel>(url);
        }

        public async Task<RolViewModel> Find(int id)
        {
            string url = "Roles/FindAsync";
            return await ApiRequests.FindAsync<RolViewModel>(url, id);
        }

        public async Task<RolDetailViewModel> Detail(int id)
        {
            string url = "Roles/DetailAsync";
            return await ApiRequests.FindAsync<RolDetailViewModel>(url, id);
        }

        public async Task<Boolean> Create(RolViewModel model)
        {
            string url = "Roles/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(RolViewModel model)
        {
            string url = "Roles/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<RolViewModel> Exist(string value)
        {
            string url = "Roles/ExistAsync";
            return await ApiRequests.ExistAsync<RolViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Roles/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        public async Task<IEnumerable<PantallaItemViewModel>> PantallasListAsync()
        {
            string url = "Roles/PantallasListAsync";
            return await ApiRequests.ListAsync<PantallaItemViewModel>(url);
        }

        public async Task<IEnumerable<PantallaIdViewModel>> PantallasByRolAsync(int rolId)
        {
            string url = "Roles/PantallasByRolAsync";
            return await ApiRequests.DropdownAsync<PantallaIdViewModel>(url, rolId);
        }

        public async Task<Boolean> SavePantallasAsync(RolConPantallasViewModel model)
        {
            string url = "Roles/SavePantallasAsync";
            return await ApiRequests.CreateAsync(url, model);
        }
    }
}
