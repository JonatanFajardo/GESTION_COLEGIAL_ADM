using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EmpleadosService
    {
        public async Task<IEnumerable<EmpleadoViewModel>> ListAsync()
        {
            string url = "Empleados/ListAsync";
            IEnumerable<EmpleadoViewModel> apiUrl = await ApiRequests.ListAsync<EmpleadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EmpleadoViewModel> Find(int id)
        {
            string url = "Empleados/FindAsync";
            EmpleadoViewModel apiUrl = await ApiRequests.FindAsync<EmpleadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EmpleadoViewModel model)
        {
            string url = "Empleados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(EmpleadoViewModel model)
        {
            string url = "Empleados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<EmpleadoViewModel> Exist(string value)
        {
            string url = "Empleados/ExistAsync";
            return await ApiRequests.ExistAsync<EmpleadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Empleados/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        public async Task<EmpleadoViewModel> Dropdown(EmpleadoViewModel model)
        {
            // Direcciones.
            string urlTitulos = "Empleados/TitulosDropdown";
            string urlCargos = "Empleados/CargosDropdown";
            // Instancias.
            var titulosDropdown = await ApiRequests.DropdownAsync<TituloViewModel>(urlTitulos);
            var cargosDropdown = await ApiRequests.DropdownAsync<CargoViewModel>(urlCargos);
            // Cargando en el modelo.
            model.LoadDropDownList(titulosDropdown, cargosDropdown);
            return model;
        }
    }
}
