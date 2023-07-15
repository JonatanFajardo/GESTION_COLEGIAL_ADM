using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de aulas.
    /// </summary>
    public class AulasService
    {
        /// <summary>
        /// Obtiene una lista de modalidades de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de modalidades.</returns>
        public async Task<IEnumerable<ModalidadViewModel>> ListAsync()
        {
            string url = "Aulas/ListAsync";
            IEnumerable<ModalidadViewModel> apiUrl = await ApiRequests.ListAsync<ModalidadViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una modalidad por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la modalidad.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la modalidad encontrada.</returns>
        public async Task<ModalidadViewModel> Find(int id)
        {
            string url = "Aulas/FindAsync";
            ModalidadViewModel apiUrl = await ApiRequests.FindAsync<ModalidadViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva modalidad de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la modalidad a crear.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se creó la modalidad correctamente.</returns>
        public async Task<Boolean> Create(ModalidadViewModel model)
        {
            string url = "Aulas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una modalidad existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la modalidad a editar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se editó la modalidad correctamente.</returns>
        public async Task<Boolean> Edit(ModalidadViewModel model)
        {
            string url = "Aulas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe una modalidad con el valor especificado de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la modalidad encontrada.</returns>
        public async Task<ModalidadViewModel> Exist(string value)
        {
            string url = "Aulas/ExistAsync";
            return await ApiRequests.ExistAsync<ModalidadViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una modalidad por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la modalidad a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó la modalidad correctamente.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Aulas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}