using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los conceptos de pago.
    /// </summary>
    public class ConceptosPagoService
    {
        /// <summary>
        /// Obtiene una lista de conceptos de pago de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos ConceptoPagoListViewModel.</returns>
        public async Task<IEnumerable<ConceptoPagoListViewModel>> ListAsync()
        {
            string url = "ConceptosPago/ListAsync";
            IEnumerable<ConceptoPagoListViewModel> apiUrl = await ApiRequests.ListAsync<ConceptoPagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un concepto de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del concepto de pago.</param>
        /// <returns>El objeto ConceptoPagoFindViewModel encontrado.</returns>
		public async Task<ConceptoPagoFindViewModel> Find(int id)
		{
			string url = "ConceptosPago/FindAsync";
			ConceptoPagoFindViewModel apiUrl = await ApiRequests.FindAsync<ConceptoPagoFindViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de un concepto de pago por su identificador.
		/// </summary>
		/// <param name="id">El identificador del concepto de pago.</param>
		/// <returns>El detalle solicitado.</returns>
		public async Task<ConceptoPagoDetailViewModel> Detail(int id)
		{
			string url = "ConceptosPago/DetailAsync";
			ConceptoPagoDetailViewModel apiUrl = await ApiRequests.FindAsync<ConceptoPagoDetailViewModel>(url, id);
			return apiUrl;
		}

        /// <summary>
        /// Crea un nuevo concepto de pago de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ConceptoPagoFindViewModel a crear.</param>
        /// <returns>True si el concepto de pago se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(ConceptoPagoFindViewModel model)
        {
            string url = "ConceptosPago/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un concepto de pago existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto ConceptoPagoFindViewModel a editar.</param>
        /// <returns>True si el concepto de pago se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(ConceptoPagoFindViewModel model)
        {
            string url = "ConceptosPago/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un concepto de pago existe de forma asíncrona.
        /// </summary>
        /// <param name="value">La descripción del concepto de pago a verificar.</param>
        /// <returns>El objeto ConceptoPagoFindViewModel si existe, de lo contrario null.</returns>
        public async Task<ConceptoPagoFindViewModel> Exist(string value)
        {
            string url = "ConceptosPago/ExistAsync";
            return await ApiRequests.ExistAsync<ConceptoPagoFindViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un concepto de pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del concepto de pago a eliminar.</param>
        /// <returns>True si el concepto de pago se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "ConceptosPago/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
