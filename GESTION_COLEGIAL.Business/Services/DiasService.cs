using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DiasService
    {
        /// <summary>
        /// Obtiene una lista de días de forma asincrónica.
        /// </summary>
        /// <returns>Una colección de objetos DiaViewModel.</returns>
        public async Task<IEnumerable<DiaViewModel>> ListAsync()
        {
            string url = "Dias/ListAsync";
            IEnumerable<DiaViewModel> apiUrl = await ApiRequests.ListAsync<DiaViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Encuentra un día por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del día a encontrar.</param>
        /// <returns>El objeto DiaViewModel encontrado, o null si no se encuentra.</returns>
        public async Task<DiaViewModel> Find(int id)
        {
            string url = "Dias/FindAsync";
            DiaViewModel apiUrl = await ApiRequests.FindAsync<DiaViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo día de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto DiaViewModel a crear.</param>
        /// <returns>true si la creación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Create(DiaViewModel model)
        {
            string url = "Dias/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un día existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto DiaViewModel a editar.</param>
        /// <returns>true si la edición fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Edit(DiaViewModel model)
        {
            string url = "Dias/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un día existe de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>El objeto DiaViewModel si existe, de lo contrario null.</returns>
        public async Task<DiaViewModel> Exist(string value)
        {
            string url = "Dias/ExistAsync";
            return await ApiRequests.ExistAsync<DiaViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un día por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del día a eliminar.</param>
        /// <returns>true si la eliminación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Dias/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}