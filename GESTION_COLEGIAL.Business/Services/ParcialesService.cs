using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona métodos para interactuar con la API relacionados con los parciales.
    /// </summary>
    public class ParcialesService
    {
        /// <summary>
        /// Obtiene una colección de parciales de la API de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos ParcialViewModel.</returns>
        public async Task<IEnumerable<ParcialViewModel>> ListAsync()
        {
            string url = "Modalidades/ListAsync";
            IEnumerable<ParcialViewModel> apiUrl = await ApiRequests.ListAsync<ParcialViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un parcial por su ID en la API de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del parcial a buscar.</param>
        /// <returns>El objeto ParcialViewModel encontrado.</returns>
        public async Task<ParcialViewModel> Find(int id)
        {
            string url = "Modalidades/FindAsync";
            ParcialViewModel apiUrl = await ApiRequests.FindAsync<ParcialViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo parcial en la API de forma asíncrona.
        /// </summary>
        /// <param name="model">Objeto ParcialViewModel con los datos del parcial a crear.</param>
        /// <returns>true si el parcial se creó correctamente, false en caso contrario.</returns>
        public async Task<Boolean> Create(ParcialViewModel model)
        {
            string url = "Modalidades/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un parcial existente en la API de forma asíncrona.
        /// </summary>
        /// <param name="model">Objeto ParcialViewModel con los datos actualizados del parcial.</param>
        /// <returns>true si el parcial se editó correctamente, false en caso contrario.</returns>
        public async Task<Boolean> Edit(ParcialViewModel model)
        {
            string url = "Modalidades/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un parcial con el valor especificado en la API de forma asíncrona.
        /// </summary>
        /// <param name="value">Valor a verificar en el parcial.</param>
        /// <returns>El objeto ParcialViewModel encontrado o null si no existe.</returns>
        public async Task<ParcialViewModel> Exist(string value)
        {
            string url = "Modalidades/ExistAsync";
            return await ApiRequests.ExistAsync<ParcialViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un parcial por su ID en la API de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del parcial a eliminar.</param>
        /// <returns>true si el parcial se eliminó correctamente, false en caso contrario.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Modalidades/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
