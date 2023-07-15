using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de encargados.
    /// </summary>
    public class EncargadosService
    {
        /// <summary>
        /// Obtiene una lista de encargados de forma asincrónica.
        /// </summary>
        /// <returns>Una colección de objetos EncargadoViewModel.</returns>
        public async Task<IEnumerable<EncargadoViewModel>> ListAsync()
        {
            string url = "Encargados/ListAsync";
            IEnumerable<EncargadoViewModel> apiUrl = await ApiRequests.ListAsync<EncargadoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Encuentra un encargado por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del encargado a encontrar.</param>
        /// <returns>El objeto EncargadoViewModel encontrado, o null si no se encuentra.</returns>
        public async Task<EncargadoViewModel> Find(int id)
        {
            string url = "Encargados/FindAsync";
            EncargadoViewModel apiUrl = await ApiRequests.FindAsync<EncargadoViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo encargado de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto EncargadoViewModel a crear.</param>
        /// <returns>true si la creación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Create(EncargadoViewModel model)
        {
            string url = "Encargados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un encargado existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto EncargadoViewModel a editar.</param>
        /// <returns>true si la edición fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Edit(EncargadoViewModel model)
        {
            string url = "Encargados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un encargado existe de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>El objeto EncargadoViewModel si existe, de lo contrario null.</returns>
        public async Task<EncargadoViewModel> Exist(string value)
        {
            string url = "Encargados/ExistAsync";
            return await ApiRequests.ExistAsync<EncargadoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un encargado por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del encargado a eliminar.</param>
        /// <returns>true si la eliminación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Encargados/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}