using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EmpleadosService
    {
        public async Task<IEnumerable<EmpleadoViewModel>> List()
        {
            string url = "Empleados/List";
            IEnumerable<EmpleadoViewModel> apiUrl = await ApiRequests.List<EmpleadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EmpleadoViewModel> Find(int id)
        {
            string url = "Empleados/Find";
            EmpleadoViewModel apiUrl = await ApiRequests.Find<EmpleadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EmpleadoViewModel model)
        {
            string url = "Empleados/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(EmpleadoViewModel model)
        {
            string url = "Empleados/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<EmpleadoViewModel> Exist(string value)
        {
            string url = "Empleados/Exist";
            return await ApiRequests.Exist<EmpleadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Empleados/Remove";
            return await ApiRequests.Delete(url, id);
        }

        public async Task<EmpleadoViewModel> Dropdown(EmpleadoViewModel model)
        {
            // Direcciones.
            string urlTitulos = "Empleados/TitulosDropdown";
            string urlCargos = "Empleados/CargosDropdown";
            // Instancias.
            var titulosDropdown = await ApiRequests.Dropdown<TituloViewModel>(urlTitulos);
            var cargosDropdown = await ApiRequests.Dropdown<CargoViewModel>(urlCargos);
            // Cargando en el modelo.
            model.LoadDropDownList(titulosDropdown, cargosDropdown);
            return model;
        }
    }
}
