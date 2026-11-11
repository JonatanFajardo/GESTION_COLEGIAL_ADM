using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio del Año Lectivo (consume la API externa AniosLectivos).
    /// </summary>
    public class AniosLectivosService
    {
        public async Task<IEnumerable<AnioLectivoViewModel>> ListAsync()
        {
            string url = "AniosLectivos/ListAsync";
            return await ApiRequests.ListAsync<AnioLectivoViewModel>(url);
        }

        public async Task<AnioLectivoViewModel> Find(int id)
        {
            string url = "AniosLectivos/FindAsync";
            return await ApiRequests.FindAsync<AnioLectivoViewModel>(url, id);
        }

        public async Task<AnioLectivoViewModel> Activo()
        {
            string url = "AniosLectivos/ActivoAsync";
            return await ApiRequests.GetSingleAsync<AnioLectivoViewModel>(url);
        }

        public async Task<Boolean> Create(AnioLectivoViewModel model)
        {
            string url = "AniosLectivos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(AnioLectivoViewModel model)
        {
            string url = "AniosLectivos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>Crea el borrador y devuelve el nuevo Id (para el wizard).</summary>
        public async Task<int> CreateReturnId(AnioLectivoViewModel model)
        {
            string url = "AniosLectivos/CreateAsync";
            return await ApiRequests.PostAsyncWithResponse<int>(url, model);
        }

        public async Task<AnioLectivoViewModel> Exist(string value)
        {
            string url = "AniosLectivos/ExistAsync";
            return await ApiRequests.ExistAsync<AnioLectivoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "AniosLectivos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>Marca el año como el año de trabajo activo (PUT ?value=id).</summary>
        public async Task<Boolean> SetActivo(int id)
        {
            string url = "AniosLectivos/SetActivoAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
