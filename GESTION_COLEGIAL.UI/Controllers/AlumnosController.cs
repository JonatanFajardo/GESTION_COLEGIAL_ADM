using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class AlumnosController : BaseController
    {
        AlumnosService alumnosService = new AlumnosService();
        // GET: Alumnos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var model = new AlumnoViewModel();
            var drop = await Dropdown(model);
            return View(drop);
        }

        public async Task<ActionResult> List()
        {
            var result = await alumnosService.List();
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            var result = await alumnosService.Find(id);
            //var model = _mapper.Map(result);
            var drop = await Dropdown(result);
            return View("Create", drop);
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
            var cursoNiveles = await alumnosService.CursoNivelesDropdown(id);
            return AjaxResult(cursoNiveles);
        }

        public JsonResult GetModalidades(int id)
        {
            var modalidades = alumnosService.ModalidadesDropdown(id);
            return Json(modalidades, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCursos(int id)
        {
            var cursos = alumnosService.CursosDropdown(id);
            return Json(cursos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSecciones(int id)
        {
            var secciones = alumnosService.SeccionesDropdown(id);
            return Json(secciones, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(AlumnoViewModel model)
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