using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EmpleadosController : BaseController
    {
        private readonly EmpleadosService empleadosService = new EmpleadosService();

        // GET: Empleados
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateAsync()
        {
            var model = new EmpleadoViewModel();
            var load = await Load(model);
            return View(load);
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await empleadosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await empleadosService.Find(id);
            var load = await Load(result);

            return View("CreateAsync", load);
        }

        [HttpPost]
        public async Task<ActionResult> Save(EmpleadoViewModel model)
        {
            if (model.Emp_Id == 0)
            {
                bool result = await empleadosService.Create(model);

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
                bool result = await empleadosService.Edit(model);

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
        public async Task<ActionResult> DeleteAsync(EmpleadoViewModel model)
        {
            bool result = await empleadosService.Delete(model.Emp_Id);

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
            return await empleadosService.Dropdown(model);
        }
    }
}