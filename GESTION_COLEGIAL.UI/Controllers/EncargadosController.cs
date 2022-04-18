﻿using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EncargadosController : BaseController
    {
        public string count;
        EncargadosService encargadosService = new EncargadosService();
        public EncargadosController()
        {
            //MyMethodAsync();
        }

        public async void MyMethodAsync()
        {
            //count = await encargadosService.Counts();
        }

        // GET: Encargados
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await encargadosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            var result = await encargadosService.Find(id);
            return View("CreateAsync", result);
        }

        [HttpPost]
        public async Task<ActionResult> Save(EncargadoViewModel model)
        {
            if (model.Enc_Id == 0)
            {
                bool result = await encargadosService.Create(model);

                //Validamos error
                if (result)
                {
                    //AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                //AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return RedirectToAction("Index");
            }
            else
            {
                bool result = await encargadosService.Edit(model);

                //Validamos error
                if (result)
                {
                    //AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                //AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(EncargadoViewModel model)
        {
            bool result = await encargadosService.Delete(model.Enc_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }


    }
}