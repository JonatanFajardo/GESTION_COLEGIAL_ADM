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
        /// Obtiene una lista de aulas de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de aulas.</returns>
        public async Task<IEnumerable<AulaViewModel>> ListAsync()
        {
            string url = "Aulas/ListAsync";
            IEnumerable<AulaViewModel> apiUrl = await ApiRequests.ListAsync<AulaViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un aula por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del aula.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el aula encontrada.</returns>
        public async Task<AulaViewModel> Find(int id)
        {
            string url = "Aulas/FindAsync";
            AulaViewModel apiUrl = await ApiRequests.FindAsync<AulaViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva aula de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del aula a crear.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se creó el aula correctamente.</returns>
        public async Task<Boolean> Create(AulaViewModel model)
        {
            string url = "Aulas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un aula existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del aula a editar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se editó el aula correctamente.</returns>
        public async Task<Boolean> Edit(AulaViewModel model)
        {
            string url = "Aulas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un aula con el valor especificado de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el aula encontrada.</returns>
        public async Task<AulaViewModel> Exist(string value)
        {
            string url = "Aulas/ExistAsync";
            return await ApiRequests.ExistAsync<AulaViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un aula por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del aula a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el aula correctamente.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Aulas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}