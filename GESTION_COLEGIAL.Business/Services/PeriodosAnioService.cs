using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio de Períodos del año lectivo (consume la API externa PeriodosAnio).
    /// </summary>
    public class PeriodosAnioService
    {
        /// <summary>Lista los períodos de un año lectivo (GET ?value=aniId).</summary>
        public async Task<IEnumerable<PeriodoAnioViewModel>> ListAsync(int anioId)
        {
            string url = "PeriodosAnio/ListAsync";
            return await ApiRequests.FindAllAsync<PeriodoAnioViewModel>(url, anioId);
        }

        public async Task<PeriodoAnioViewModel> Find(int id)
        {
            string url = "PeriodosAnio/FindAsync";
            return await ApiRequests.FindAsync<PeriodoAnioViewModel>(url, id);
        }

        public async Task<Boolean> Create(PeriodoAnioViewModel model)
        {
            string url = "PeriodosAnio/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>Crea el período y devuelve el nuevo Id.</summary>
        public async Task<int> CreateReturnId(PeriodoAnioViewModel model)
        {
            string url = "PeriodosAnio/CreateAsync";
            return await ApiRequests.PostAsyncWithResponse<int>(url, model);
        }

        public async Task<Boolean> Edit(PeriodoAnioViewModel model)
        {
            string url = "PeriodosAnio/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "PeriodosAnio/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
