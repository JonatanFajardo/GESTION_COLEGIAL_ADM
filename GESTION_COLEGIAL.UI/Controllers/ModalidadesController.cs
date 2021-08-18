using GESTION_COLEGIAL.UI.Helpers;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;

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
            //if (!ModelState.IsValid)
            //{
            //}
            
            string url = "https://localhost:44341/api/Modalidades/List";

            var httpclient = new HttpClient();
            //var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
            //var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.GetAsync(url);//

            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
            var resultSerialize = JsonConvert.DeserializeObject<ModalidadViewModel>(content);

            //var result = await SendHttpClient.Get<ModalidadViewModel>(url);
            //var resultSerialize = JsonConvert.DeserializeObject<ModalidadViewModel>(result);

            //if (result == null)
            //{
            //    Show(AlertMessageType.Warning, "No se pudo acceder a los datos.");
            //}
            return Json(new { data = resultSerialize });
            //return View("Index");

            
        }

        //[HttpPost]
        //public ActionResult Create()
        //{

        //    string str = string.Format("Alert('{0}')", "gola");
        //    JavaScriptResult result = new JavaScriptResult();
        //    result.Script = str;
        //    return result;
        //}
        public ActionResult Alert()
        {

            //alertMessage.Show();
            ShowController(AlertMessageType.Warning);
            //ViewBag.JavaScriptFunction = string.Format("alertConfig.alert('{0}', '{1}');", "success", "Ingresado :p");
            return View("Index");
        }
        //[HttpPost]
        // GET: Modalidades/Create
        //public async Task<ActionResult> Create(ModalidadViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //    }

        //    string url = "https://localhost:44341/api/Modalidades/Create";
        //    await SendHttpClient.Post(url, model);
        //    return View("Index");

        //    //using (var client = new HttpClient())
        //    //{
        //    //    client.BaseAddress = new Uri("http://localhost:64189/api/student");

        //    //    //HTTP POST
        //    //    var response = client.PostAsync("api/AgentCollection", new StringContent(new JavaScriptSerializer().Serialize(user), Encoding.UTF8, "application/json")).Result;

        //    //    var postJob = client.PostAsJsonAsync< ModalidadViewModel > "model", model);
        //    //    postJob.Wait();

        //    //    var result = postTask.Result;
        //    //    if (result.IsSuccessStatusCode)
        //    //    {
        //    //        return RedirectToAction("Index");
        //    //    }
        //    //}

        //    //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");



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
