using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio de Secciones por Año + profesor guía (consume la API externa SeccionesAnio).
    /// Fase 4 — Clases y profesores.
    /// </summary>
    public class SeccionesAnioService
    {
        /// <summary>Lista las secciones vivas de un año lectivo (filtra por ?value={aniId}).</summary>
        public async Task<IEnumerable<SeccionAnioViewModel>> ListAsync(int aniId)
        {
            string url = "SeccionesAnio/ListAsync?value=" + aniId;
            return await ApiRequests.ListAsync<SeccionAnioViewModel>(url);
        }

        public async Task<SeccionAnioViewModel> Find(int id)
        {
            string url = "SeccionesAnio/FindAsync";
            return await ApiRequests.FindAsync<SeccionAnioViewModel>(url, id);
        }

        public async Task<Boolean> Create(SeccionAnioViewModel model)
        {
            string url = "SeccionesAnio/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(SeccionAnioViewModel model)
        {
            string url = "SeccionesAnio/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "SeccionesAnio/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
