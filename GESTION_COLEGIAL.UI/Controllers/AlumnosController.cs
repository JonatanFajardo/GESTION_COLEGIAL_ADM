using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class AlumnosController : BaseController
    {
        private readonly AlumnosService alumnosService = new AlumnosService();

        // GET: Alumnos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateAsync()
        {
            var model = new AlumnoViewModel();
            var drop = await Dropdown(model);
            return View(drop);
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await alumnosService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await alumnosService.Find(id);
            var drop = await Dropdown(result);
            return View("CreateAsync", drop);
        }

        [HttpPost]
        public async Task<ActionResult> Save(AlumnoViewModel model)
        {
            if (model.Alu_Id == 0)
            {
                bool result = await alumnosService.Create(model);

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
                bool result = await alumnosService.Edit(model);

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

        public async Task<AlumnoViewModel> Dropdown(AlumnoViewModel model)
        {
            return await alumnosService.Dropdown(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetCursosNiveles(int id)
        {
            var result = await alumnosService.CursoNivelesDropdown(id);
            IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
            {
                Value = x.Cun_Id.ToString(),
                Text = x.Cun_Descripcion
            }).ToList();
            return AjaxResult(resultToSelectListItem);
        }

        public async Task<ActionResult> GetModalidades(int id)
        {
            var result = await alumnosService.ModalidadesDropdown(id);
            IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
            {
                Value = x.Mda_Id.ToString(),
                Text = x.Mda_Descripcion
            }).ToList();
            return AjaxResult(resultToSelectListItem);
        }

        public async Task<ActionResult> GetCursos(int id)
        {
            var result = await alumnosService.CursosDropdown(id);
            IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
            {
                Value = x.Cur_Id.ToString(),
                Text = x.Cur_Nombre
            }).ToList();
            return AjaxResult(resultToSelectListItem);
        }

        public async Task<ActionResult> GetSecciones(int id)
        {
            var result = await alumnosService.SeccionesDropdown(id);
            //Validamos error
            if (result == null)
            {
                AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error al procesar la solicitud");
            }
            IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
            {
                Value = x.Sec_Id.ToString(),
                Text = x.Sec_Descripcion
            }).ToList();
            return AjaxResult(resultToSelectListItem);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(AlumnoViewModel model)
        {
            bool result = await alumnosService.Delete(model.Alu_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}