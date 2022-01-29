using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NivelesEducativosService
    {
        public async Task<IEnumerable<NivelEducativoViewModel>> List()
        {
            string url = "NivelesEducativos/List";
            IEnumerable<NivelEducativoViewModel> apiUrl = await ApiRequests.List<NivelEducativoViewModel>(url);
            return apiUrl;
        }

        public async Task<NivelEducativoViewModel> Find(int id)
        {
            string url = "NivelesEducativos/Find";
            NivelEducativoViewModel apiUrl = await ApiRequests.Find<NivelEducativoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(NivelEducativoViewModel model)
        {
            string url = "NivelesEducativos/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<NivelEducativoViewModel> Exist(string value)
        {
            string url = "NivelesEducativos/Exist";
            return await ApiRequests.Exist<NivelEducativoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "NivelesEducativos/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
