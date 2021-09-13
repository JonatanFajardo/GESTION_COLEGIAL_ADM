﻿using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CargosController : BaseController
    {
        // GET: Cargos
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> List()
        {
            string url = "Cargos/List";
            var result = await CatalogsService.List<CargoViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CargoViewModel model)
        {
            if (model.Car_Id == 0)
            {
                string url = "Cargos/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false);
                }

                return AjaxResult(true);
            }
            else
            {
                string url = "Cargos/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false);
                }

                return AjaxResult(true);
            }

        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Cargos/Find";
            var result = await CatalogsService.Find<CargoViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Car_Id, string Car_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Car_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Cargos/Exist";
            var result = await CatalogsService.Exist<CargoViewModel>(url, Car_Descripcion);

            //Validamos error
            if (result != null)
            {
                int? firstValue = result.First().Car_Id;
                return (firstValue == Car_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            string url = "Cargos/Remove";
            bool result = await CatalogsService.Delete(url, id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false);
            }
            return AjaxResult(true);
        }
    }
}