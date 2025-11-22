using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de cargos.
    /// </summary>
    public class CargosService
    {
        /// <summary>
        /// Obtiene una lista de cargos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cargos.</returns>
        public async Task<IEnumerable<CargoViewModel>> ListAsync()
        {
            string url = "Cargos/ListAsync";
            IEnumerable<CargoViewModel> apiUrl = await ApiRequests.ListAsync<CargoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un cargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del cargo.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cargo encontrado.</returns>
		public async Task<CargoViewModel> Find(int id)
		{
			string url = "Cargos/FindAsync";
			CargoViewModel apiUrl = await ApiRequests.FindAsync<CargoViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de un cargo específico.
		/// </summary>
		/// <param name="id">El identificador del cargo.</param>
		/// <returns>El detalle del cargo.</returns>
		public async Task<CargoDetailViewModel> Detail(int id)
        {
            string url = "Cargos/DetailAsync";
            CargoDetailViewModel apiUrl = await ApiRequests.FindAsync<CargoDetailViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo cargo de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del cargo a crear.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se creó el cargo correctamente.</returns>
        public async Task<Boolean> Create(CargoViewModel model)
        {
            string url = "Cargos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un cargo existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del cargo a editar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se editó el cargo correctamente.</returns>
        public async Task<Boolean> Edit(CargoViewModel model)
        {
            string url = "Cargos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un cargo con el valor especificado de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cargo encontrado.</returns>
        public async Task<CargoViewModel> Exist(string value)
        {
            string url = "Cargos/ExistAsync";
            return await ApiRequests.ExistAsync<CargoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un cargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del cargo a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el cargo correctamente.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Cargos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
