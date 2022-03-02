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
            //AlumnoViewModel alumnoViewModel = new AlumnoViewModel();
            //alumnoViewModel.Niv_Id = result.Niv_Id,
            //    alumnoViewModel.Niv_Descripcion = result.Niv_Descripcion,
            //    alumnoViewModel.Cun_Id = result.Cun_Id,
            //    alumnoViewModel.Cun_Descripcion = result.Cun_Descripcion,
            //    alumnoViewModel.Mda_Id = result.Mda_Id,
            //    alumnoViewModel.Mda_Descripcion = result.Mda_Descripcion,
            //    alumnoViewModel.Cur_Id = result.Cur_Id,
            //    alumnoViewModel.Cur_Nombre = result.Cur_Nombre,
            //    alumnoViewModel.Sec_Id = result.Sec_Id,
            //    alumnoViewModel.Sec_Descripcion = result.Sec_Descripcion,
            //    alumnoViewModel.Est_Id = result.Est_Id,
            //    alumnoViewModel.Est_Descripcion = result.Est_Descripcion,
            //    alumnoViewModel.Alu_Id = result.Alu_Id,
            //    alumnoViewModel.Per.Per_Identidad = result.Per_Id.,
            //    alumnoViewModel.Per.Per_PrimerNombre = result.Per_PrimerNombre,
            //    alumnoViewModel.Per.Per_SegundoNombre = result.Per_SegundoNombre,
            //    alumnoViewModel.Per.Per_ApellidoPaterno = result.Per_ApellidoPaterno,
            //    alumnoViewModel.Per.Per_ApellidoMaterno = result.Per_ApellidoMaterno,
            //    alumnoViewModel.Per.Per_FechaNacimiento = result.Per_FechaNacimiento,
            //    alumnoViewModel.Per.Per_CorreoElectronico = result.Per_CorreoElectronico,
            //    alumnoViewModel.Per.Per_Telefono = result.Per_Telefono,
            //    alumnoViewModel.Per.Per_Direccion = result.Per_Direccion,
            //    alumnoViewModel.Per.Per_Sexo = result.Per_Sexo,
            //    alumnoViewModel.Per.Per_EsEliminado = result.Per_EsEliminado
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
