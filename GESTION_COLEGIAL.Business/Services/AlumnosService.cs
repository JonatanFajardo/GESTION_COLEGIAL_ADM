using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AlumnosService
    {
        public async Task<IEnumerable<AlumnoViewModel>> List()
        {
            string url = "Alumnos/List";
            IEnumerable<AlumnoViewModel> apiUrl = await ApiRequests.List<AlumnoViewModel>(url);
            return apiUrl;
        }

        public async Task<AlumnoViewModel> Find(int id)
        {
            string url = "Alumnos/Find";
            AlumnoViewModel apiUrl = await ApiRequests.Find<AlumnoViewModel>(url, id);
            return apiUrl;
        }

        public async Task<Boolean> Create(AlumnoViewModel model)
        {
            string url = "Alumnos/Create";
            return await ApiRequests.Create(url, model);
        }

        public async Task<Boolean> Edit(AlumnoViewModel model)
        {
            string url = "Alumnos/Edit";
            return await ApiRequests.Edit(url, model);
        }

        public async Task<AlumnoViewModel> Exist(string value)
        {
            string url = "Alumnos/Exist";
            return await ApiRequests.Exist<AlumnoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Alumnos/Remove";
            return await ApiRequests.Delete(url, id);
        }

        public async Task<AlumnoViewModel> Dropdown(AlumnoViewModel model)
        {
            string urlNivelesEducativos = "Alumnos/NivelesEducativosDropdown";
            string urlEstados = "Alumnos/EstadosDropdown";
            var nivelesEducativosDropdown = await ApiRequests.Dropdown<NivelEducativoViewModel>(urlNivelesEducativos);
            var estadosDropdown = await ApiRequests.Dropdown<EstadoViewModel>(urlEstados);
            model.LoadDropDownList(nivelesEducativosDropdown, estadosDropdown);
            return model;
        }
        public async Task<IEnumerable<CursoNivelDropViewModel>> CursoNivelesDropdown(int id)
        {
            try
            {
                string urlCursosNiveles = "Alumnos/CursosNivelesDropdown";
                var cursosNivelesDropdown = await ApiRequests.Dropdown<CursoNivelDropViewModel>(urlCursosNiveles, id);
                return cursosNivelesDropdown;
            }
            catch (Exception error)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ModalidadViewModel>> ModalidadesDropdown(int id)
        {
            string urlModalidades = "Alumnos/ModalidadesDropdown";
            var modalidadesDropdown = await ApiRequests.Dropdown<ModalidadViewModel>(urlModalidades, id);
            return modalidadesDropdown;
        }

        public async Task<IEnumerable<CursoViewModel>> CursosDropdown(int id)
        {
            string urlCursos = "Alumnos/CursosDropdown";
            var cursosDropdown = await ApiRequests.Dropdown<CursoViewModel>(urlCursos, id);
            return cursosDropdown;
        }

        public async Task<IEnumerable<SeccionViewModel>> SeccionesDropdown(int id)
        {
            string urlSecciones = "Alumnos/SeccionesDropdown";
            var seccionesDropdown = await ApiRequests.Dropdown<SeccionViewModel>(urlSecciones, id);
            return seccionesDropdown;
        }

    }
}
