using GESTION_COLEGIAL.Business.DTOs;
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
    public class CargosService : BaseService
    {
        /// <summary>
        /// Obtiene una lista de cargos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cargos.</returns>
        public async Task<IEnumerable<CargoViewModel>> ListAsync()
        {
            const string url = "Cargos/ListAsync";
            var dtos = await ApiRequests.ListAsync<CargoListDto>(url);
            return Mapper.Map<IEnumerable<CargoViewModel>>(dtos);
        }

        /// <summary>
        /// Busca un cargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del cargo.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cargo encontrado.</returns>
        public async Task<CargoViewModel> Find(int id)
        {
            const string url = "Cargos/FindAsync";
            var dto = await ApiRequests.FindAsync<CargoDetailDto>(url, id);
            return Mapper.Map<CargoViewModel>(dto);
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
            const string url = "Cargos/ExistAsync";
            var dto = await ApiRequests.ExistAsync<CargoFindDto>(url, value);
            return Mapper.Map<CargoViewModel>(dto);
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