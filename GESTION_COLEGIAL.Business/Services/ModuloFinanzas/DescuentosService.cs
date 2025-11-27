using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los descuentos.
    /// </summary>
    public class DescuentosService
    {
        /// <summary>
        /// Obtiene una lista de descuentos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos DescuentoListViewModel.</returns>
        public async Task<IEnumerable<DescuentoListViewModel>> ListAsync()
        {
            string url = "Descuentos/ListAsync";
            IEnumerable<DescuentoListViewModel> apiUrl = await ApiRequests.ListAsync<DescuentoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un descuento por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del descuento.</param>
        /// <returns>El objeto DescuentoFindViewModel encontrado.</returns>
		public async Task<DescuentoFindViewModel> Find(int id)
		{
			string url = "Descuentos/FindAsync";
			DescuentoFindViewModel apiUrl = await ApiRequests.FindAsync<DescuentoFindViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de un descuento por su identificador.
		/// </summary>
		/// <param name="id">El identificador del descuento.</param>
		/// <returns>El detalle del descuento solicitado.</returns>
		public async Task<DescuentoDetailViewModel> Detail(int id)
		{
			string url = "Descuentos/DetailAsync";
			DescuentoDetailViewModel apiUrl = await ApiRequests.FindAsync<DescuentoDetailViewModel>(url, id);
			return apiUrl;
		}

        /// <summary>
        /// Crea un nuevo descuento de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto DescuentoFindViewModel a crear.</param>
        /// <returns>True si el descuento se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(DescuentoFindViewModel model)
        {
            string url = "Descuentos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un descuento existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto DescuentoFindViewModel a editar.</param>
        /// <returns>True si el descuento se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(DescuentoFindViewModel model)
        {
            string url = "Descuentos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un descuento existe de forma asíncrona.
        /// </summary>
        /// <param name="value">La descripción del descuento a verificar.</param>
        /// <returns>El objeto DescuentoFindViewModel si existe, de lo contrario null.</returns>
        public async Task<DescuentoFindViewModel> Exist(string value)
        {
            string url = "Descuentos/ExistAsync";
            return await ApiRequests.ExistAsync<DescuentoFindViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un descuento por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del descuento a eliminar.</param>
        /// <returns>True si el descuento se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Descuentos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
