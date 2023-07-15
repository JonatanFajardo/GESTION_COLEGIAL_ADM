using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DuracionesService
    {
        /// <summary>
        /// Obtiene una lista de duraciones de forma asincrónica.
        /// </summary>
        /// <returns>Una colección de objetos DuracionViewModel.</returns>
        public async Task<IEnumerable<DuracionViewModel>> ListAsync()
        {
            string url = "Duraciones/ListAsync";
            IEnumerable<DuracionViewModel> apiUrl = await ApiRequests.ListAsync<DuracionViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Encuentra una duración por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador de la duración a encontrar.</param>
        /// <returns>El objeto DuracionViewModel encontrado, o null si no se encuentra.</returns>
        public async Task<DuracionViewModel> Find(int id)
        {
            string url = "Duraciones/FindAsync";
            DuracionViewModel apiUrl = await ApiRequests.FindAsync<DuracionViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva duración de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto DuracionViewModel a crear.</param>
        /// <returns>true si la creación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Create(DuracionViewModel model)
        {
            string url = "Duraciones/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una duración existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto DuracionViewModel a editar.</param>
        /// <returns>true si la edición fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Edit(DuracionViewModel model)
        {
            string url = "Duraciones/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si una duración existe de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>El objeto DuracionViewModel si existe, de lo contrario null.</returns>
        public async Task<DuracionViewModel> Exist(string value)
        {
            string url = "Duraciones/ExistAsync";
            return await ApiRequests.ExistAsync<DuracionViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una duración por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador de la duración a eliminar.</param>
        /// <returns>true si la eliminación fue exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Duraciones/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}