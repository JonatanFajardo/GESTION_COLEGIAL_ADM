using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los niveles educativos.
    /// </summary>
    public class NivelesEducativosService
    {
        /// <summary>
        /// Obtiene una lista de niveles educativos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos NivelEducativoViewModel.</returns>
        public async Task<IEnumerable<NivelEducativoViewModel>> ListAsync()
        {
            string url = "NivelesEducativos/ListAsync";
            IEnumerable<NivelEducativoViewModel> apiUrl = await ApiRequests.ListAsync<NivelEducativoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un nivel educativo por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del nivel educativo.</param>
        /// <returns>El objeto NivelEducativoViewModel encontrado.</returns>
		public async Task<NivelEducativoViewModel> Find(int id)
		{
			string url = "NivelesEducativos/FindAsync";
			NivelEducativoViewModel apiUrl = await ApiRequests.FindAsync<NivelEducativoViewModel>(url, id);
			return apiUrl;
		}

		/// <summary>
		/// Obtiene el detalle de un nivel educativo por su identificador.
		/// </summary>
		/// <param name="id">El identificador del nivel educativo.</param>
		/// <returns>El detalle del nivel educativo solicitado.</returns>
		public async Task<NivelEducativoDetailViewModel> Detail(int id)
        {
            string url = "NivelesEducativos/DetailAsync";
            NivelEducativoDetailViewModel apiUrl = await ApiRequests.FindAsync<NivelEducativoDetailViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo nivel educativo de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto NivelEducativoViewModel a crear.</param>
        /// <returns>True si el nivel educativo se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un nivel educativo existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto NivelEducativoViewModel a editar.</param>
        /// <returns>True si el nivel educativo se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de nivel educativo existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de nivel educativo a verificar.</param>
        /// <returns>El objeto NivelEducativoViewModel si existe, de lo contrario null.</returns>
        public async Task<NivelEducativoViewModel> Exist(string value)
        {
            string url = "NivelesEducativos/ExistAsync";
            return await ApiRequests.ExistAsync<NivelEducativoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un nivel educativo por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del nivel educativo a eliminar.</param>
        /// <returns>True si el nivel educativo se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "NivelesEducativos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
