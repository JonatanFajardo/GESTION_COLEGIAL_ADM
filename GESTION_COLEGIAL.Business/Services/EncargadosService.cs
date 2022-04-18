using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GESTION_COLEGIAL.Business.Services
{
    public class EncargadosService
    {

        public async Task<IEnumerable<EncargadoViewModel>> ListAsync()
        {
            string url = "Encargados/ListAsync";
            IEnumerable<EncargadoViewModel> apiUrl = await ApiRequests.ListAsync<EncargadoViewModel>(url);
            return apiUrl;
        }

        public async Task<EncargadoViewModel> Find(int id)
        {
            string url = "Encargados/FindAsync";
            EncargadoViewModel apiUrl = await ApiRequests.FindAsync<EncargadoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(EncargadoViewModel model)
        {
            string url = "Encargados/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        public async Task<Boolean> Edit(EncargadoViewModel model)
        {
            string url = "Encargados/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        public async Task<EncargadoViewModel> Exist(string value)
        {
            string url = "Encargados/ExistAsync";
            return await ApiRequests.ExistAsync<EncargadoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Encargados/Remove";
            return await ApiRequests.DeleteAsync(url, id);
        }

        //public async Task<string> Count()
        //{
        //    //var list = await ListAsync();
        //    ////count = list.ToListAsync().Count.ToString();
        //    //return list.ToListAsync().Count.ToString();
        //    string url = "Encargados/ListAsync";
        //    IEnumerable<EncargadoViewModel> apiUrl = await ApiRequests.ListAsync<EncargadoViewModel>(url);
        //    return apiUrl;
        //}
    }
}
