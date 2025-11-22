using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las modalidades.
    /// </summary>
    public class ModalidadesService
    {
        /// <summary>
        /// Obtiene una lista de modalidades de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos ModalidadViewModel.</returns>
        public async Task<IEnumerable<ModalidadViewModel>> ListAsync()
        {
            string url = "Modalidades/ListAsync";
            IEnumerable<ModalidadViewModel> apiUrl = await ApiRequests.ListAsync<ModalidadViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una modalidad por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la modalidad.</param>
        /// <returns>El objeto ModalidadViewModel encontrado.</returns>
        public async Task<ModalidadViewModel> Find(int id)
        {
            string url = "Modalidades/FindAsync";
            ModalidadViewModel apiUrl = await ApiRequests.FindAsync<ModalidadViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene los detalles completos de una modalidad por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la modalidad.</param>
        /// <returns>El objeto ModalidadViewModel con todos los detalles.</returns>
        public async Task<ModalidadDetailViewModel> Detail(int id)
        {
            string url = "Modalidades/DetailAsync";
            ModalidadDetailViewModel apiUrl = await ApiRequests.FindAsync<ModalidadDetailViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva modalidad de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ModalidadViewModel a crear.</param>
        /// <returns>True si la modalidad se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(ModalidadViewModel model)
        {
            string url = "Modalidades/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una modalidad existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ModalidadViewModel a editar.</param>
        /// <returns>True si la modalidad se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(ModalidadViewModel model)
        {
            string url = "Modalidades/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de modalidad existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de modalidad a verificar.</param>
        /// <returns>El objeto ModalidadViewModel si existe, de lo contrario null.</returns>
        public async Task<ModalidadViewModel> Exist(string value)
        {
            string url = "Modalidades/ExistAsync";
            return await ApiRequests.ExistAsync<ModalidadViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una modalidad por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la modalidad a eliminar.</param>
        /// <returns>True si la modalidad se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Modalidades/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}