using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CursosService
    {
        public async Task<IEnumerable<CursoViewModel>> List()
        {
            string url = "Cursos/List";
            IEnumerable<CursoViewModel> apiUrl = await ApiRequests.List<CursoViewModel>(url);
            return apiUrl;
        }

        public async Task<CursoViewModel> Find(int id)
        {
            // Url de peticion
            string url = "Cursos/Find";
            string urlModalidades = "Cursos/CursosModalidadesFind";
            string urlMaterias = "Cursos/CursosMateriasFind";
            string urlCursosNiveles = "Cursos/CursosNivelesFind";
            string urlSecciones = "Cursos/CursosSeccionesFind";
            // Envio y recepcion de la peticion
            var result = await ApiRequests.Find<CursoViewModel>(url, id);
            var modalidades = await ApiRequests.FindAll<ModalidadViewModel>(urlModalidades, id);
            var secciones = await ApiRequests.FindAll<SeccionViewModel>(urlSecciones, id);
            var cursosniveles = await ApiRequests.FindAll<CursoNivelViewModel>(urlCursosNiveles, id);
            var materias = await ApiRequests.FindAll<MateriaViewModel>(urlMaterias, id);

            var load = await Load(result);

            load.ModalidadesCheckList = load.ModalidadesCheckList.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value.ToString(),
                Selected = modalidades.Any(y => y.Mda_Id.ToString() == x.Value.ToString()) ? true : false
            }).ToList();

            load.SeccionesCheckList = load.SeccionesCheckList.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value.ToString(),
                Selected = secciones.Any(y => y.Sec_Id.ToString() == x.Value.ToString()) ? true : false
            }).ToList();

            load.CursoNivelesCheckList = load.CursoNivelesCheckList.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value.ToString(),
                Selected = cursosniveles.Any(y => y.Cun_Id.ToString() == x.Value.ToString()) ? true : false
            }).ToList();

            load.MateriasCheckList = load.MateriasCheckList.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value.ToString(),
                Selected = materias.Any(y => y.Mat_Id.ToString() == x.Value.ToString()) ? true : false
            }).ToList();
            return load;
        }

        public async Task<Boolean> Create(CursoViewModel model)
        {


            model.Modalidades = model.ModalidadesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.CursoNiveles = model.CursoNivelesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.Materias = model.MateriasCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.Secciones = model.SeccionesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.SeccionesCheckList = null;
            model.ModalidadesCheckList = null;
            model.CursoNivelesCheckList = null;
            model.MateriasCheckList = null;
            // Direcciones.
            string url = "Cursos/Create";
            string urlModalidades = "Cursos/CursosModalidadesCreate";
            string urlCursosNiveles = "Cursos/CursosNivelesCreate";
            string urlMaterias = "Cursos/CursosMateriasCreate";
            string urlSecciones = "Cursos/CursosSeccionesCreate";
            // Envio y recepcion de la peticion
            bool result = await ApiRequests.Create(url, model);
            bool resultModalidades = await ApiRequests.Create(urlModalidades, model);
            bool resultCursosNiveles = await ApiRequests.Create(urlCursosNiveles, model);
            bool resultMaterias = await ApiRequests.Create(urlMaterias, model);
            bool resultSecciones = await ApiRequests.Create(urlSecciones, model);
            // Comprobando el resultado de las peticiones.
            Boolean[] results = { result, resultModalidades, resultCursosNiveles, resultMaterias, resultSecciones };
            return ValidationResults.BooleanListIsTrue(results);
        }

        public async Task<Boolean> Edit(CursoViewModel model)
        {
            // Direcciones.
            string url = "Cursos/Edit";
            string urlModalidades = "Cursos/CursosModalidadesEdit";
            string urlCursosNiveles = "Cursos/CursosNivelesEdit";
            string urlMaterias = "Cursos/CursosMateriasEdit";
            string urlSecciones = "Cursos/CursosSeccionesEdit";
            // Envio y recepcion de la peticion
            bool result = await ApiRequests.Edit(url, model);
            bool resultModalidades = await ApiRequests.Edit(urlModalidades, model);
            bool resultCursosNiveles = await ApiRequests.Edit(urlCursosNiveles, model);
            bool resultMaterias = await ApiRequests.Edit(urlMaterias, model);
            bool resultSecciones = await ApiRequests.Edit(urlSecciones, model);
            // Comprobando el resultado de las peticiones.
            Boolean[] results = { result, resultModalidades, resultCursosNiveles, resultMaterias, resultSecciones };
            return ValidationResults.BooleanListIsTrue(results);
        }

        public async Task<CursoViewModel> Exist(string value)
        {
            string url = "Cursos/Exist";
            return await ApiRequests.Exist<CursoViewModel>(url, value);
        }

        public async Task<Boolean> Delete(int id)
        {
            string url = "Cursos/Remove";
            return await ApiRequests.Delete(url, id);
        }

        public async Task<CursoViewModel> Load(CursoViewModel model)
        {
            // Direcciones.
            string urlNivelesEducativos = "Cursos/NivelesEducativosDropdown";
            string urlModalidades = "Cursos/ModalidadesList";
            string urlMaterias = "Cursos/MateriasList";
            string urlCursosNiveles = "Cursos/CursosNivelesList";
            string urlSecciones = "Cursos/SeccionesList";
            // Instancias.
            var nivelesEducativosDropdown = await ApiRequests.Dropdown<NivelEducativoViewModel>(urlNivelesEducativos);
            var modalidadesList = await ApiRequests.CheckList<ModalidadViewModel>(urlModalidades);
            var materiasList = await ApiRequests.CheckList<MateriaViewModel>(urlMaterias);
            var cursosNivelesList = await ApiRequests.CheckList<CursoNivelViewModel>(urlCursosNiveles);
            var seccionesList = await ApiRequests.CheckList<SeccionViewModel>(urlSecciones);
            // Cargando en el modelo.
            model.LoadDropDownList(nivelesEducativosDropdown);
            model.LoadCheckList(modalidadesList, seccionesList, cursosNivelesList, materiasList);
            return model;
        }
    }
}
