using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class AlumnosController : BaseController
    {
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
            string url = "Alumnos/List";
            var result = await CatalogsService.List<AlumnoViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Alumnos/Find";
            var result = await CatalogsService.Find<AlumnoViewModel>(url, id);
            //var model = _mapper.Map<AlumnoViewModel>(result);
            var drop = await Dropdown(result);
            return View("Create", drop);
        }

        [HttpPost]
        public async Task<ActionResult> Save(AlumnoViewModel model)
        {
            if (model.Alu_Id == 0)
            {
                string url = "Alumnos/Create";
                bool result = await CatalogsService.Create(url, model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return View("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
                return View("Index");
            }
            else
            {
                string url = "Alumnos/Edit";
                bool result = await CatalogsService.Edit(url, model);

                //Validamos error
                if (result)
                {
                    AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
                    return View("Index");
                }
                AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
                return View("Index");
            }
        }

        public async Task<AlumnoViewModel> Dropdown(AlumnoViewModel model)
        {
            string urlCursos = "Alumnos/CursosDropdown";
            string urlEstados = "Alumnos/EstadosDropdown";
            var cursosDropdown = await CatalogsService.Dropdown<CursoViewModel>(urlCursos);
            var estadosDropdown = await CatalogsService.Dropdown<EstadoViewModel>(urlEstados);
            model.LoadDropDownList(cursosDropdown, estadosDropdown);
            return model;
        }

        [HttpPost]
        public async Task<ActionResult> Delete(AlumnoViewModel model)
        {
            string url = "Alumnos/Remove";
            bool result = await CatalogsService.Delete(url, model.Alu_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}