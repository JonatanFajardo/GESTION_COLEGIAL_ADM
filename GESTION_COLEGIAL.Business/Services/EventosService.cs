using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los eventos.
    /// </summary>
    public class EventosService
    {
        /// <summary>
        /// Obtiene una lista de modalidades de eventos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos ModalidadViewModel.</returns>
        public async Task<IEnumerable<ModalidadViewModel>> ListAsync()
        {
            string url = "Eventos/ListAsync";
            IEnumerable<ModalidadViewModel> apiUrl = await ApiRequests.ListAsync<ModalidadViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una modalidad de evento por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la modalidad de evento.</param>
        /// <returns>El objeto ModalidadViewModel encontrado.</returns>
        public async Task<ModalidadViewModel> Find(int id)
        {
            string url = "Eventos/FindAsync";
            ModalidadViewModel apiUrl = await ApiRequests.FindAsync<ModalidadViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva modalidad de evento de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ModalidadViewModel a crear.</param>
        /// <returns>True si la modalidad de evento se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(ModalidadViewModel model)
        {
            string url = "Eventos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una modalidad de evento existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ModalidadViewModel a editar.</param>
        /// <returns>True si la modalidad de evento se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(ModalidadViewModel model)
        {
            string url = "Eventos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de modalidad de evento existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de modalidad de evento a verificar.</param>
        /// <returns>El objeto ModalidadViewModel si existe, de lo contrario null.</returns>
        public async Task<ModalidadViewModel> Exist(string value)
        {
            string url = "Eventos/ExistAsync";
            return await ApiRequests.ExistAsync<ModalidadViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una modalidad de evento por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la modalidad de evento a eliminar.</param>
        /// <returns>True si la modalidad de evento se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Eventos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}