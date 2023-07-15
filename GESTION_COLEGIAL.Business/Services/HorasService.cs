using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las horas.
    /// </summary>
    public class HorasService
    {
        /// <summary>
        /// Obtiene una lista de horas de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos HoraViewModel.</returns>
        public async Task<IEnumerable<HoraViewModel>> ListAsync()
        {
            string url = "Horas/ListAsync";
            IEnumerable<HoraViewModel> apiUrl = await ApiRequests.ListAsync<HoraViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una hora por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la hora.</param>
        /// <returns>El objeto HoraViewModel encontrado.</returns>
        public async Task<HoraViewModel> Find(int id)
        {
            string url = "Horas/FindAsync";
            HoraViewModel apiUrl = await ApiRequests.FindAsync<HoraViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva hora de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HoraViewModel a crear.</param>
        /// <returns>True si la hora se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(HoraViewModel model)
        {
            string url = "Horas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una hora existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HoraViewModel a editar.</param>
        /// <returns>True si la hora se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(HoraViewModel model)
        {
            string url = "Horas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de hora existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de hora a verificar.</param>
        /// <returns>El objeto HoraViewModel si existe, de lo contrario null.</returns>
        public async Task<HoraViewModel> Exist(string value)
        {
            string url = "Horas/ExistAsync";
            return await ApiRequests.ExistAsync<HoraViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una hora por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la hora a eliminar.</param>
        /// <returns>True si la hora se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Horas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}