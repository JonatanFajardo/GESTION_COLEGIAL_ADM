using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las notas.
    /// </summary>
    public class NotasService
    {
        /// <summary>
        /// Obtiene una lista de notas de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos NotaViewModel.</returns>
        public async Task<IEnumerable<NotaViewModel>> ListAsync()
        {
            string url = "Notas/ListAsync";
            IEnumerable<NotaViewModel> apiUrl = await ApiRequests.ListAsync<NotaViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una nota por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la nota.</param>
        /// <returns>El objeto NotaViewModel encontrado.</returns>
        public async Task<NotaViewModel> Find(int id)
        {
            string url = "Notas/FindAsync";
            NotaViewModel apiUrl = await ApiRequests.FindAsync<NotaViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva nota de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto NotaViewModel a crear.</param>
        /// <returns>True si la nota se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(NotaViewModel model)
        {
            string url = "Notas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una nota existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto NotaViewModel a editar.</param>
        /// <returns>True si la nota se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(NotaViewModel model)
        {
            string url = "Notas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de nota existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de nota a verificar.</param>
        /// <returns>El objeto NotaViewModel si existe, de lo contrario null.</returns>
        public async Task<NotaViewModel> Exist(string value)
        {
            string url = "Notas/ExistAsync";
            return await ApiRequests.ExistAsync<NotaViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una nota por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la nota a eliminar.</param>
        /// <returns>True si la nota se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Notas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}