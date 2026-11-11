using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador de Años Lectivos (capa multi-año, hueco #1).
    /// </summary>
    [SessionManager("Listado de años lectivos")]
    public class AniosLectivosController : BaseController
    {
        private readonly AniosLectivosService aniosLectivosService = new AniosLectivosService();
        private readonly EnfoqueAnioService enfoqueAnioService = new EnfoqueAnioService();

        // GET: AniosLectivos
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Wizard de configuración del año lectivo (5 pasos, espejo de Skuul).
        /// Sin id = nuevo (paso 1 vacío). Con id = reanuda la configuración del borrador.
        /// </summary>
        public async Task<ActionResult> Setup(int? id, int step = 1)
        {
            AnioLectivoViewModel model;
            if (id.HasValue && id.Value > 0)
            {
                model = await aniosLectivosService.Find(id.Value) ?? new AnioLectivoViewModel { Ani_Id = 0, Ani_Estado = 1 };
            }
            else
            {
                model = new AnioLectivoViewModel { Ani_Id = 0, Ani_Estado = 1 };
            }

            if (step < 1) step = 1;
            if (step > 5) step = 5;
            ViewBag.Step = step;
            return View("Setup", model);
        }

        /// <summary>Paso 1 del wizard: crea el borrador (o lo actualiza) y avanza al paso 2.</summary>
        [HttpPost]
        public async Task<ActionResult> SetupStep1(AnioLectivoViewModel model)
        {
            if (model.Ani_Estado == 0) model.Ani_Estado = 1; // Borrador por defecto

            if (model.Ani_Id == 0)
            {
                int newId = await aniosLectivosService.CreateReturnId(model);
                return RedirectToAction("Setup", new { id = newId, step = 2 });
            }

            await aniosLectivosService.Edit(model);
            return RedirectToAction("Setup", new { id = model.Ani_Id, step = 2 });
        }

        /// <summary>Paso 2 del wizard: guarda el enfoque de enseñanza (upsert) del año.</summary>
        [HttpPost]
        public async Task<ActionResult> SetupStep2(EnfoqueAnioViewModel model)
        {
            bool error = await enfoqueAnioService.Set(model);   // true = error (boolean invertido)
            return Json(new { success = !error });
        }

        /// <summary>Último paso: publica el año (estado → Abierto).</summary>
        [HttpPost]
        public async Task<ActionResult> PublishAsync(int id)
        {
            var model = await aniosLectivosService.Find(id);
            if (model != null)
            {
                model.Ani_Estado = 3; // Abierto
                await aniosLectivosService.Edit(model);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await aniosLectivosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await aniosLectivosService.Find(id);
            return AjaxResult(result, true);
        }

        public async Task<ActionResult> ActivoAsync()
        {
            var result = await aniosLectivosService.Activo();
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(AnioLectivoViewModel model)
        {
            if (model.Ani_Id == 0)
            {
                bool result = await aniosLectivosService.Create(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await aniosLectivosService.Edit(model);
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Ani_Id, string Ani_Anio)
        {
            var result = await aniosLectivosService.Exist(Ani_Anio);
            if (result != null)
            {
                int? firstValue = result.Ani_Id;
                return (firstValue == Ani_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(AnioLectivoViewModel model)
        {
            bool result = await aniosLectivosService.Delete(model.Ani_Id);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        [HttpPost]
        public async Task<ActionResult> SetActivoAsync(int id)
        {
            bool result = await aniosLectivosService.SetActivo(id);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
