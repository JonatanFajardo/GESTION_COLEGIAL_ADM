using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los estados.
    /// </summary>
    public class EstadosService
    {
        /// <summary>
        /// Obtiene una lista de estados de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos EstadoViewModel.</returns>
        public async Task<IEnumerable<EstadoViewModel>> ListAsync()
        {
            string url = "Estados/ListAsync";
            IEnumerable<EstadoViewModel> apiUrl = await ApiRequests.ListAsync<EstadoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un estado por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del estado.</param>
        /// <returns>El objeto EstadoViewModel encontrado.</returns>
        public async Task<EstadoViewModel> Find(int id)
        {
            string url = "Estados/FindAsync";
            EstadoViewModel apiUrl = await ApiRequests.FindAsync<EstadoViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo estado de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto EstadoViewModel a crear.</param>
        /// <returns>True si el estado se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(EstadoViewModel model)
        {
            string url = "Estados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un estado existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto EstadoViewModel a editar.</param>
        /// <returns>True si el estado se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(EstadoViewModel model)
        {
            string url = "Estados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de estado existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de estado a verificar.</param>
        /// <returns>El objeto EstadoViewModel si existe, de lo contrario null.</returns>
        public async Task<EstadoViewModel> Exist(string value)
        {
            string url = "Estados/ExistAsync";
            return await ApiRequests.ExistAsync<EstadoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un estado por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del estado a eliminar.</param>
        /// <returns>True si el estado se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Estados/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}