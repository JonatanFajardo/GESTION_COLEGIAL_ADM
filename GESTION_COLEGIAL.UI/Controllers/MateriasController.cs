﻿using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class MateriasController : BaseController
    {
        // GET: Materias
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("Create");
        }

        public async Task<ActionResult> List()
        {
            string url = "Materias/List";
            var result = await CatalogsService.List<ModalidadViewModel>(url);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ModalidadViewModel model)
        {
            if (model.Mat_Id == 0)
            {
                string url = "Materias/Create";
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
                string url = "Materias/Edit";
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
            string url = "Materias/Find";
            var result = await CatalogsService.Find<ModalidadViewModel>(url, id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Mat_Id, string Mat_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Mat_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "Materias/Exist";
            var result = await CatalogsService.Exist<MateriaViewModel>(url, Mat_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Mat_Id;
                return (firstValue == Mat_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            string url = "Materias/Remove";
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