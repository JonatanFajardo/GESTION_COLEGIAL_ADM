using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Escala de notas y umbrales del cuadro de honor. Llama a la API EscalaNotas/*.
    /// </summary>
    public class EscalaNotasService
    {
        public async Task<IEnumerable<EscalaNotaViewModel>> ListAsync()
        {
            return await ApiRequests.ListAsync<EscalaNotaViewModel>("EscalaNotas/ListAsync");
        }

        public async Task<EscalaNotaViewModel> Find(int id)
        {
            return await ApiRequests.FindAsync<EscalaNotaViewModel>("EscalaNotas/FindAsync", id);
        }

        public async Task<EscalaNotaDetailViewModel> Detail(int id)
        {
            return await ApiRequests.FindAsync<EscalaNotaDetailViewModel>("EscalaNotas/DetailAsync", id);
        }

        public async Task<Boolean> Create(EscalaNotaViewModel model)
        {
            return await ApiRequests.CreateAsync("EscalaNotas/CreateAsync", model);
        }

        public async Task<Boolean> Edit(EscalaNotaViewModel model)
        {
            return await ApiRequests.EditAsync("EscalaNotas/EditAsync", model);
        }

        public async Task<Boolean> Delete(int id)
        {
            return await ApiRequests.DeleteAsync("EscalaNotas/RemoveAsync", id);
        }

        /// <summary>Devuelve la fila que choca de rango, o null si el rango está libre.</summary>
        public async Task<EscalaNotaViewModel> Solapa(decimal desde, decimal hasta, int? id)
        {
            string url = $"EscalaNotas/SolapaAsync?desde={desde}&hasta={hasta}";
            if (id.HasValue && id.Value > 0) url += $"&id={id.Value}";
            return await ApiRequests.GetSingleAsync<EscalaNotaViewModel>(url);
        }

        public async Task<CuadroHonorConfigViewModel> ConfigGet()
        {
            return await ApiRequests.GetSingleAsync<CuadroHonorConfigViewModel>("EscalaNotas/ConfigAsync");
        }

        public async Task<Boolean> ConfigSet(CuadroHonorConfigViewModel model)
        {
            return await ApiRequests.EditAsync("EscalaNotas/GuardarConfigAsync", model);
        }
    }
}
