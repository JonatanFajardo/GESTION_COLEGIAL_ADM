using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace GESTION_COLEGIAL.UI.Controllers
{
    public class ModalidadesController : Controller
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




        [HttpPost]
        // GET: Modalidades/Create
        public async Task<ActionResult> Create(ModalidadViewModel model)
        {

            //if (!ModelState.IsValid)
            //{

            //}
            String url = "https://localhost:44341/api/Modalidades/Create";
            await SendHttpClient.Post(url, model);
            return null;





            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:64189/api/student");

            //    //HTTP POST
            //    var response = client.PostAsync("api/AgentCollection", new StringContent(new JavaScriptSerializer().Serialize(user), Encoding.UTF8, "application/json")).Result;

            //    var postJob = client.PostAsJsonAsync< ModalidadViewModel > "model", model);
            //    postJob.Wait();

            //    var result = postTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        return RedirectToAction("Index");
            //    }
            //}

            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");



        }

        //// POST: Modalidades/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

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
