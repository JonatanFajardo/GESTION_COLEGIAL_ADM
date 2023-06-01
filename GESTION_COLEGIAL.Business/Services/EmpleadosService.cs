using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EmpleadosService
    {
        /// <summary>
        /// Obtiene una lista de empleados de forma asincrónica.
        /// </summary>
        /// <returns>Una colección de objetos EmpleadoViewModel.</returns>
        public async Task<IEnumerable<EmpleadoViewModel>> ListAsync()
        {
            string url = "Empleados/ListAsync";
            IEnumerable<EmpleadoViewModel> apiUrl = await ApiRequests.ListAsync<EmpleadoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Encuentra un empleado por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del empleado a encontrar.</param>
        /// <returns>El objeto EmpleadoViewModel encontrado, o null si no se encuentra.</returns>
        public async Task<EmpleadoViewModel> Find(int id)
        {
            string url = "Empleados/FindAsync";
            EmpleadoViewModel apiUrl = await ApiRequests.FindAsync<EmpleadoViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo empleado de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto EmpleadoViewModel a crear.</param>
        /// <returns>true si la creación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Create(EmpleadoViewModel model)
        {
            string url = "Empleados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un empleado existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto EmpleadoViewModel a editar.</param>
        /// <returns>true si la edición fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Edit(EmpleadoViewModel model)
        {
            string url = "Empleados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un empleado existe de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>El objeto EmpleadoViewModel si existe, de lo contrario null.</returns>
        public async Task<EmpleadoViewModel> Exist(string value)
        {
            string url = "Empleados/ExistAsync";
            return await ApiRequests.ExistAsync<EmpleadoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un empleado por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del empleado a eliminar.</param>
        /// <returns>true si la eliminación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Empleados/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Obtiene un modelo EmpleadoViewModel con las listas desplegables (dropdowns) cargadas.
        /// </summary>
        /// <param name="model">El modelo EmpleadoViewModel al que se le cargarán las listas desplegables.</param>
        /// <returns>El modelo EmpleadoViewModel con las listas desplegables cargadas.</returns>
        public async Task<EmpleadoViewModel> Dropdown(EmpleadoViewModel model)
        {
            // Obtener las URL para cargar las listas desplegables.
            string urlTitulos = "Empleados/TitulosDropdown";
            string urlCargos = "Empleados/CargosDropdown";

            // Obtener las listas desplegables de forma asincrónica.
            var titulosDropdown = await ApiRequests.DropdownAsync<TituloViewModel>(urlTitulos);
            var cargosDropdown = await ApiRequests.DropdownAsync<CargoViewModel>(urlCargos);

            // Cargar las listas desplegables en el modelo EmpleadoViewModel.
            model.LoadDropDownList(titulosDropdown, cargosDropdown);

            return model;
        }
    }
}