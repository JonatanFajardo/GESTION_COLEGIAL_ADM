using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio de ofertas de asignatura por año (Fase 5). Consume la API externa OfertasAnio.
    /// </summary>
    public class OfertasAnioService
    {
        /// <summary>Lista las ofertas de un año lectivo (filtra por ?value={aniId}).</summary>
        public async Task<IEnumerable<OfertaAnioViewModel>> ListAsync(int aniId)
        {
            string url = "OfertasAnio/ListAsync";
            return await ApiRequests.FindAllAsync<OfertaAnioViewModel>(url, aniId);
        }

        public async Task<OfertaAnioViewModel> Find(int id)
        {
            string url = "OfertasAnio/FindAsync";
            return await ApiRequests.FindAsync<OfertaAnioViewModel>(url, id);
        }

        public async Task<Boolean> Create(OfertaAnioViewModel model)
        {
            string url = "OfertasAnio/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(OfertaAnioViewModel model)
        {
            string url = "OfertasAnio/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "OfertasAnio/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
