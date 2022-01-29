using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{

    public class CursosController : BaseController
    {
        CursosService cursosService = new CursosService();

        // GET: Cursos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            var result = await cursosService.List();
            return AjaxResult(result);
        }

        public async Task<ActionResult> Create()
        {
            var model = new CursoViewModel();
            var load = await Load(model);
            return View(load);
        }

        public async Task<ActionResult> Find(int id)
        {
            var result = cursosService.Find(id);
            return View("Create", result);
        }

        [HttpPost]
        public async Task<ActionResult> Save(CursoViewModel model)
        {
            if (model.Cur_Id == 0)
            {
                Boolean createResult = await cursosService.Create(model);
                //Validamos error
                if (createResult)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return RedirectToAction("Index");
            }
            else
            {
                Boolean createResult = await cursosService.Create(model);
                //Validamos error
                if (createResult)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return RedirectToAction("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CursoViewModel model)
        {
            string url = "Cursos/Remove";
            bool result = await cursosService.Delete(model.Cur_Id);

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
        public async Task<CursoViewModel> Load(CursoViewModel model)
        {
            var result = await cursosService.Load(model);
            return result;
        }
    }
}