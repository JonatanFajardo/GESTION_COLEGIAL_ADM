using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class ParentescosService
    {
        public async Task<IEnumerable<ParentescoViewModel>> List()
        {
            string url = "Parentescos/List";
            IEnumerable<ParentescoViewModel> apiUrl = await ApiRequests.List<ParentescoViewModel>(url);
            return apiUrl;
        }

        public async Task<ParentescoViewModel> Find(int id)
        {
            string url = "Parentescos/Find";
            ParentescoViewModel apiUrl = await ApiRequests.Find<ParentescoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(ParentescoViewModel model)
        {
            string url = "Parentescos/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(ParentescoViewModel model)
        {
            string url = "Parentescos/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<ParentescoViewModel> Exist(string value)
        {
            string url = "Parentescos/Exist";
            return await ApiRequests.Exist<ParentescoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Parentescos/Remove";
            return await ApiRequests.Delete(url, id);
        }
    }
}
