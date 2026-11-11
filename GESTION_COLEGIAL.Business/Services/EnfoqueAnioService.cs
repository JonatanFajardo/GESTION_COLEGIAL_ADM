using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio del Enfoque de enseñanza por año lectivo (consume la API externa EnfoqueAnio).
    /// </summary>
    public class EnfoqueAnioService
    {
        /// <summary>Devuelve el enfoque del año (por Ani_Id), o null si aún no se ha definido.</summary>
        public async Task<EnfoqueAnioViewModel> Find(int aniId)
        {
            string url = "EnfoqueAnio/FindAsync";
            return await ApiRequests.FindAsync<EnfoqueAnioViewModel>(url, aniId);
        }

        /// <summary>Guarda el enfoque del año (upsert). Devuelve true si hubo error, false si fue exitoso.</summary>
        public async Task<Boolean> Set(EnfoqueAnioViewModel model)
        {
            string url = "EnfoqueAnio/SetAsync";
            return await ApiRequests.CreateAsync(url, model);
        }
    }
}
