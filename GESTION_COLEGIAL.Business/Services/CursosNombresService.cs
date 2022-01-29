using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CursosNombresService
    {
        public async Task<IEnumerable<CursoNombreViewModel>> List()
        {
            string url = "CursosNombres/List";
            IEnumerable<CursoNombreViewModel> apiUrl = await ApiRequests.List<CursoNombreViewModel>(url);
            return apiUrl;
        }

        public async Task<CursoNombreViewModel> Find(int id)
        {
            string url = "CursosNombres/Find";
            CursoNombreViewModel apiUrl = await ApiRequests.Find<CursoNombreViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(CursoNombreViewModel model)
        {
            string url = "CursosNombres/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(CursoNombreViewModel model)
        {
            string url = "CursosNombres/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<CursoNombreViewModel> Exist(string value)
        {
            string url = "CursosNombres/Exist";
            return await ApiRequests.Exist<CursoNombreViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "CursosNombres/Remove";
            return await ApiRequests.Delete(url, id);
        }

        public async Task<CursoNombreViewModel> Load(CursoNombreViewModel model)
        {
            // Direcciones.
            string urlNivelesEducativos = "CursosNombres/NivelesEducativosDropdown";
            // Instancias.
            var nivelesEducativosDropdown = await ApiRequests.Dropdown<NivelEducativoViewModel>(urlNivelesEducativos);
            // Cargando en el modelo.
            model.LoadDropDownList(nivelesEducativosDropdown);
            return model;
        }
    }
}
