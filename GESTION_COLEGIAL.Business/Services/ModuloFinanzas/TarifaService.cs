using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las tarifas.
    /// </summary>
    public class TarifaService
    {
        /// <summary>
        /// Obtiene una lista de tarifas de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos TarifaListViewModel.</returns>
        public async Task<IEnumerable<TarifaListViewModel>> ListAsync()
        {
            string url = "Tarifas/ListAsync";
            IEnumerable<TarifaListViewModel> apiUrl = await ApiRequests.ListAsync<TarifaListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una tarifa por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la tarifa.</param>
        /// <returns>El objeto TarifaFindViewModel encontrado.</returns>
        public async Task<TarifaFindViewModel> Find(int id)
        {
            string url = "Tarifas/FindAsync";
            TarifaFindViewModel apiUrl = await ApiRequests.FindAsync<TarifaFindViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva tarifa de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto TarifaFindViewModel a crear.</param>
        /// <returns>True si la tarifa se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(TarifaFindViewModel model)
        {
            string url = "Tarifas/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una tarifa existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto TarifaFindViewModel a editar.</param>
        /// <returns>True si la tarifa se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(TarifaFindViewModel model)
        {
            string url = "Tarifas/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Elimina una tarifa por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la tarifa a eliminar.</param>
        /// <returns>True si la tarifa se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Tarifas/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Obtiene un modelo TarifaViewModel con las listas desplegables (dropdowns) cargadas.
        /// </summary>
        /// <param name="model">El modelo TarifaViewModel al que se le cargarán las listas desplegables.</param>
        /// <returns>El modelo TarifaViewModel con las listas desplegables cargadas.</returns>
        public async Task<TarifaViewModel> Dropdown(TarifaViewModel model)
        {
            // Obtener las URL para cargar las listas desplegables.
            string urlConceptosPago = "Tarifas/ConceptosPagoDropdown";
            string urlNivelesEducativos = "Tarifas/NivelesEducativosDropdown";
            string urlCursosNiveles = "Tarifas/CursosNivelesDropdown";

            // Obtener las listas desplegables de forma asincrónica.
            var conceptosPagoDropdown = await ApiRequests.DropdownAsync<ConceptoPagoListViewModel>(urlConceptosPago);
            var nivelesEducativosDropdown = await ApiRequests.DropdownAsync<NivelEducativoViewModel>(urlNivelesEducativos);
            var cursosNivelesDropdown = await ApiRequests.DropdownAsync<CursoNivelViewModel>(urlCursosNiveles);

            // Cargar las listas desplegables en el modelo TarifaViewModel.
            model.LoadDropDownList(conceptosPagoDropdown, nivelesEducativosDropdown, cursosNivelesDropdown);

            return model;
        }
    }
}
