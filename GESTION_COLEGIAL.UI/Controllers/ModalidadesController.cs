
using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using GESTION_COLEGIAL.UI.Helpers;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ModalidadesController : BaseController
    {
        // GET: Modalidades
        public ActionResult Index()
        {
            return View();
        }

        // GET: Modalidades/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Modalidades/List";
            var resultSerialize = await SendHttpClient.Get<ModalidadViewModel>(url);
            return Json(new { data = resultSerialize }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //GET: Modalidades/Create
        public async Task<ActionResult> Create(ModalidadViewModel model)
        {
            if (model.Mda_Id == 0)
            {
                string url = "https://localhost:44341/api/Modalidades/Create";
                bool result = await SendHttpClient.Post(url, model);
                if (result == true)
                {
                    ShowController(AlertMessageType.Error);
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                ShowController(AlertMessageType.Success);
                //return View("Index");
                //return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string url = "https://localhost:44341/api/Modalidades/Edit";
                bool result = await SendHttpClient.Put(url, model);
                if (result == true) //2
                {
                    ShowController(AlertMessageType.Error);
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                ShowController(AlertMessageType.Success);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                //return View("Index");
                //return Json(new { data = true }, JsonRequestBehavior.AllowGet);
            }

        }

        //[AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<ActionResult> Exist(int? Mda_Id, string Mda_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Mda_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            string url = "https://localhost:44341/api/Modalidades/Exist";
            var result = await SendHttpClient.Exist<ModalidadViewModel>(url, Mda_Descripcion);
            if (result != null)
            {
                int? firstValue = result.First().Mda_Id;
                return (firstValue == Mda_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        // GET: Modalidades/Edit/5
        public async Task<ActionResult> Find(int id)
        {
            string url = "https://localhost:44341/api/Modalidades/Find";
            ModalidadViewModel resultSerialize = await SendHttpClient.Find<ModalidadViewModel>(url, id);
            return Json(new { item = resultSerialize , success = true}, JsonRequestBehavior.AllowGet);
        }

        // POST: Modalidades/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Modalidades/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            string url = "Modalidades/Remove";
            bool result = await SendHttpClient.Delete(url, id);
            if (result == true) 
            {
                //ShowController(AlertMessageType.Error);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            //ShowController(AlertMessageType.Success);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // POST: Modalidades/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
