﻿using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EmpleadosController : BaseController
    {
        // GET: Empleados
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> List()
        //{
        //    string url = "NivelesEducativos/List";
        //    var result = await CatalogsService.List<NivelEducativoViewModel>(url);
        //    return AjaxResult(result);
        //}

        public async Task<ActionResult> Create()
        {
            var model = new EmpleadoViewModel();
            var load = await Load(model);
            return View(load);
        }

        public async Task<ActionResult> List()
        {
            string url = "Empleados/List";
            var result = await CatalogsService.List<EmpleadoViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Empleados/Find"; 
            var result = await CatalogsService.Find<EmpleadoViewModel>(url, id);
            //var model = _mapper.Map<EmpleadoViewModel>(result);
            var load = await Load(result);

            return View("Create", load);
        }

        [HttpPost]
        public async Task<ActionResult> Save(EmpleadoViewModel model)
        {            
            if (model.Emp_Id == 0)
            {
                string url = "Empleados/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return RedirectToAction("Index");
            }
            else
            {
                string url = "Empleados/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(EmpleadoViewModel model)
        {
            string url = "Empleados/Remove";
            bool result = await CatalogsService.Delete(url, model.Emp_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        /// <summary>
        /// Carga informacion.
        /// </summary>
        /// <remarks>
        /// Carga los dropdown y los check list
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<EmpleadoViewModel> Load(EmpleadoViewModel model)
        {
            // Direcciones.
            string urlTitulos = "Empleados/TitulosDropdown";
            string urlCargos = "Empleados/CargosDropdown";
            // Instancias.
            var titulosDropdown = await CatalogsService.Dropdown<TituloViewModel>(urlTitulos);
            var cargosDropdown = await CatalogsService.Dropdown<CargoViewModel>(urlCargos);
            // Cargando en el modelo.
            model.LoadDropDownList(titulosDropdown, cargosDropdown);
            return model;
        }
    }
}