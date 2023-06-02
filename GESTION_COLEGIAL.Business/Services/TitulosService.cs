using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase de servicio para interactuar con los endpoints de la API relacionados con los títulos.
    /// </summary>
    public class TitulosService
    {
        /// <summary>
        /// Recupera una lista de modelos de vista de títulos de forma asincrónica.
        /// </summary>
        /// <returns>Una colección enumerable de modelos de vista de títulos.</returns>
        public async Task<IEnumerable<TituloViewModel>> ListAsync()
        {
            string url = "Titulos/ListAsync";
            IEnumerable<TituloViewModel> apiUrl = await ApiRequests.ListAsync<TituloViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Recupera un modelo de vista de título específico de forma asincrónica por su ID.
        /// </summary>
        /// <param name="id">El ID del título.</param>
        /// <returns>El modelo de vista de título.</returns>
        public async Task<TituloViewModel> Find(int id)
        {
            string url = "Titulos/FindAsync";
            TituloViewModel apiUrl = await ApiRequests.FindAsync<TituloViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo título de forma asincrónica.
        /// </summary>
        /// <param name="model">El modelo de vista de título.</param>
        /// <returns>True si la creación es exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Create(TituloViewModel model)
        {
            string url = "Titulos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Actualiza un título existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El modelo de vista de título.</param>
        /// <returns>True si la actualización es exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Edit(TituloViewModel model)
        {
            string url = "Titulos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un título con el valor especificado de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>
        /// El modelo de vista de título si existe un título con el valor especificado,
        /// de lo contrario null.
        /// </returns>
        public async Task<TituloViewModel> Exist(string value)
        {
            string url = "Titulos/ExistAsync";
            return await ApiRequests.ExistAsync<TituloViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un título de forma asincrónica por su ID.
        /// </summary>
        /// <param name="id">El ID del título.</param>
        /// <returns>True si la eliminación es exitosa, de lo contrario false.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Titulos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
