using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CursosController : BaseController
    {
        // GET: Cursos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var model = new CursoViewModel();
            var load = await Load(model);
            return View(load);
        }

        public async Task<ActionResult> List()
        {
            string url = "Cursos/List";
            var result = await CatalogsService.List<CursoViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Cursos/Find";
            string urlModalidades = "Cursos/CursosModalidadesFind";
            string urlMaterias = "Cursos/CursosMateriasFind";
            string urlCursosNiveles = "Cursos/CursosNivelesFind";
            string urlSecciones = "Cursos/CursosSeccionesFind";
            var result = await CatalogsService.Find<CursoViewModel>(url, id);

            var modalidades = await CatalogsService.FindAll<ModalidadViewModel>(urlModalidades, id); 
            var secciones = await CatalogsService.FindAll<SeccionViewModel>(urlSecciones, id); 
            var cursosniveles = await CatalogsService.FindAll<CursoNivelViewModel>(urlCursosNiveles, id); 
            var materias = await CatalogsService.FindAll<MateriaViewModel>(urlMaterias, id); 
            //object ob = ;
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

            //ObjectExtension.EqualCheckBoxProperties<ModalidadViewModel>(modalidades, "Mda_Id", result.ModalidadesCheckList, "Value", "Selected");
            //ObjectExtension.EqualsBoolean(modalidades.Select(prop => Convert.ToInt32(prop.Mda_Id)).ToArray(), modalidades.Select(prop => Convert.ToInt32(prop.Mda_Id)).ToArray());
            //ObjectExtension.GetValueProperty(modalidades, "Mda_Id");

            //result.ModalidadesCheckList = modalidades.Select(x => new SelectListItem()
            //{
            //    Text = x.Mda_Descripcion,
            //    Value = x.Mda_Id.ToString(),
            //    Selected = true
            //}).ToList();
            //result.MateriasCheckList = (IList<SelectListItem>)await CatalogsService.FindAll<MateriaViewModel>(urlMaterias, id);
            //result.CursoNivelesCheckList = (IList<SelectListItem>)await CatalogsService.FindAll<CursoNivelViewModel>(urlCursosNiveles, id);
            //result.SeccionesCheckList = (IList<SelectListItem>)await CatalogsService.FindAll<SeccionViewModel>(urlSecciones, id);
            return View("Create", load);
        }


        //public Boolean GetIsValid(object obj, string name)
        //{
        //    obj1.
        //}



        [HttpPost]
        public async Task<ActionResult> Save(CursoViewModel model)
        {
            model.Modalidades = model.ModalidadesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.CursoNiveles = model.CursoNivelesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.Materias = model.MateriasCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.Secciones = model.SeccionesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
            model.SeccionesCheckList = null;
            model.ModalidadesCheckList = null;
            model.CursoNivelesCheckList = null;
            model.MateriasCheckList = null;
            if (model.Cur_Id == 0)
            {
                string url = "Cursos/Create";
                string urlModalidades = "Cursos/CursosModalidadesCreate";
                string urlCursosNiveles = "Cursos/CursosNivelesCreate";
                string urlMaterias = "Cursos/CursosMateriasCreate";
                string urlSecciones = "Cursos/CursosSeccionesCreate";
                bool result = await CatalogsService.Create(url, model);
                bool resultModalidades = await CatalogsService.Create(urlModalidades, model);
                bool resultCursosNiveles = await CatalogsService.Create(urlCursosNiveles, model);
                bool resultMaterias = await CatalogsService.Create(urlMaterias, model);
                bool resultSecciones = await CatalogsService.Create(urlSecciones, model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return View("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return View("Index");
            }
            else
            {
                string url = "Cursos/Edit";
                string urlModalidades = "Cursos/CursosModalidadesEdit";
                string urlCursosNiveles = "Cursos/CursosNivelesEdit";
                string urlMaterias = "Cursos/CursosMateriasEdit";
                string urlSecciones = "Cursos/CursosSeccionesEdit";
                bool result = await CatalogsService.Edit(url, model);
                bool resultModalidades = await CatalogsService.Edit(urlModalidades, model);
                bool resultCursosNiveles = await CatalogsService.Edit(urlCursosNiveles, model);
                bool resultMaterias = await CatalogsService.Edit(urlMaterias, model);
                bool resultSecciones = await CatalogsService.Edit(urlSecciones, model);
                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return View("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CursoViewModel model)
        {
            string url = "Cursos/Remove";
            bool result = await CatalogsService.Delete(url, model.Cur_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        public async Task<CursoViewModel> Load(CursoViewModel model)
        {
            // Direcciones.
            string urlNivelesEducativos = "Cursos/NivelesEducativosDropdown";
            string urlAulas = "Cursos/AulasDropdown";
            string urlCursosNombres = "Cursos/CursosNombresDropdown";
            string urlModalidades = "Cursos/ModalidadesList";
            string urlMaterias = "Cursos/MateriasList";
            string urlCursosNiveles = "Cursos/CursosNivelesList";
            string urlSecciones = "Cursos/SeccionesList";
            // Instancias.
            var nivelesEducativosDropdown = await CatalogsService.Dropdown<NivelEducativoViewModel>(urlNivelesEducativos);
            var aulasDropdown = await CatalogsService.Dropdown<AulaViewModel>(urlAulas);
            var cursosNombresDropdown = await CatalogsService.Dropdown<CursoNombreViewModel>(urlCursosNombres);
            var modalidadesList = await CatalogsService.CheckList<ModalidadViewModel>(urlModalidades);
            var materiasList = await CatalogsService.CheckList<MateriaViewModel>(urlMaterias);
            var cursosNivelesList = await CatalogsService.CheckList<CursoNivelViewModel>(urlCursosNiveles);
            var seccionesList = await CatalogsService.CheckList<SeccionViewModel>(urlSecciones);
            // Cargando en el modelo.
            model.LoadDropDownList(nivelesEducativosDropdown, aulasDropdown, cursosNombresDropdown);
            model.LoadCheckList(modalidadesList, seccionesList, cursosNivelesList, materiasList);
            return model;
        }
    }
}