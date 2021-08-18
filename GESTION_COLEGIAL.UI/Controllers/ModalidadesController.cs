
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
            string url = "https://localhost:44341/api/Modalidades/List";
            var resultSerialize = await SendHttpClient.Get<ModalidadViewModel>(url);
            return Json(new { data = resultSerialize }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //GET: Modalidades/Create
        public async Task<ActionResult> Create(ModalidadViewModel model)
        {
            if (!ModelState.IsValid)
            {
            }

            string url = "https://localhost:44341/api/Modalidades/Create";
            bool result = await SendHttpClient.Post(url, model);
            if (result == true)
            {
                ShowController(AlertMessageType.Error);
                return View("Index");
            } 

            ShowController(AlertMessageType.Success);
            return View("Index");           
        }


       // GET: Modalidades/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Modalidades/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
