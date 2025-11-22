using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los estados de pago.
    /// </summary>
    public class EstadosPagoService
    {
        /// <summary>
        /// Obtiene una lista de estados de pago de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos EstadoPagoListViewModel.</returns>
        public async Task<IEnumerable<EstadoPagoListViewModel>> ListAsync()
        {
            string url = "EstadosPago/ListAsync";
            IEnumerable<EstadoPagoListViewModel> apiUrl = await ApiRequests.ListAsync<EstadoPagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un estado de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del estado de pago.</param>
        /// <returns>El objeto EstadoPagoFindViewModel encontrado.</returns>
		public async Task<EstadoPagoFindViewModel> Find(int id)
		{
			string url = "EstadosPago/FindAsync";
			EstadoPagoFindViewModel apiUrl = await ApiRequests.FindAsync<EstadoPagoFindViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de un estado de pago por su identificador.
		/// </summary>
		/// <param name="id">El identificador del estado de pago.</param>
		/// <returns>El detalle del estado de pago solicitado.</returns>
		public async Task<EstadoPagoDetailViewModel> Detail(int id)
		{
			string url = "EstadosPago/DetailAsync";
			EstadoPagoDetailViewModel apiUrl = await ApiRequests.FindAsync<EstadoPagoDetailViewModel>(url, id);
			return apiUrl;
		}

        /// <summary>
        /// Crea un nuevo estado de pago de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto EstadoPagoFindViewModel a crear.</param>
        /// <returns>True si el estado de pago se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(EstadoPagoFindViewModel model)
        {
            string url = "EstadosPago/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un estado de pago existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto EstadoPagoFindViewModel a editar.</param>
        /// <returns>True si el estado de pago se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(EstadoPagoFindViewModel model)
        {
            string url = "EstadosPago/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un estado de pago existe de forma asíncrona.
        /// </summary>
        /// <param name="value">La descripción del estado de pago a verificar.</param>
        /// <returns>El objeto EstadoPagoFindViewModel si existe, de lo contrario null.</returns>
        public async Task<EstadoPagoFindViewModel> Exist(string value)
        {
            string url = "EstadosPago/ExistAsync";
            return await ApiRequests.ExistAsync<EstadoPagoFindViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un estado de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del estado de pago a eliminar.</param>
        /// <returns>True si el estado de pago se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "EstadosPago/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
