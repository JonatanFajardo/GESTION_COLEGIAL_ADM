using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las formas de pago.
    /// </summary>
    public class FormasPagoService
    {
        /// <summary>
        /// Obtiene una lista de formas de pago de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos FormaPagoListViewModel.</returns>
        public async Task<IEnumerable<FormaPagoListViewModel>> ListAsync()
        {
            string url = "FormasPago/ListAsync";
            IEnumerable<FormaPagoListViewModel> apiUrl = await ApiRequests.ListAsync<FormaPagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una forma de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la forma de pago.</param>
        /// <returns>El objeto FormaPagoFindViewModel encontrado.</returns>
		public async Task<FormaPagoFindViewModel> Find(int id)
		{
			string url = "FormasPago/FindAsync";
			FormaPagoFindViewModel apiUrl = await ApiRequests.FindAsync<FormaPagoFindViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de una forma de pago por su identificador.
		/// </summary>
		/// <param name="id">El identificador de la forma de pago.</param>
		/// <returns>El detalle de la forma de pago solicitada.</returns>
		public async Task<FormaPagoDetailViewModel> Detail(int id)
		{
			string url = "FormasPago/DetailAsync";
			FormaPagoDetailViewModel apiUrl = await ApiRequests.FindAsync<FormaPagoDetailViewModel>(url, id);
			return apiUrl;
		}

        /// <summary>
        /// Crea una nueva forma de pago de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto FormaPagoFindViewModel a crear.</param>
        /// <returns>True si la forma de pago se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(FormaPagoFindViewModel model)
        {
            string url = "FormasPago/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una forma de pago existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto FormaPagoFindViewModel a editar.</param>
        /// <returns>True si la forma de pago se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(FormaPagoFindViewModel model)
        {
            string url = "FormasPago/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si una forma de pago existe de forma asíncrona.
        /// </summary>
        /// <param name="value">La descripción de la forma de pago a verificar.</param>
        /// <returns>El objeto FormaPagoFindViewModel si existe, de lo contrario null.</returns>
        public async Task<FormaPagoFindViewModel> Exist(string value)
        {
            string url = "FormasPago/ExistAsync";
            return await ApiRequests.ExistAsync<FormaPagoFindViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una forma de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la forma de pago a eliminar.</param>
        /// <returns>True si la forma de pago se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "FormasPago/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
