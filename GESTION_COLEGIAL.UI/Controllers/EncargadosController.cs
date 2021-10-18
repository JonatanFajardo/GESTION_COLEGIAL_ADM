using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class EncargadosController : BaseController
    {
        // GET: Encargados
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            string url = "Encargados/List";
            var result = await CatalogsService.List<EncargadoViewModel>(url);
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            string url = "Encargados/Find";
            var result = await CatalogsService.Find<EncargadoViewModel>(url, id);
            return View("Create", result);
        }

        [HttpPost]
        public async Task<ActionResult> Save(EncargadoViewModel model)
        {
            if (model.Enc_Id == 0)
            {
                string url = "Encargados/Create";
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
                string url = "Encargados/Edit";
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

        [HttpPost]
        public async Task<ActionResult> Delete(EncargadoViewModel model)
        {
            string url = "Encargados/Remove";
            bool result = await CatalogsService.Delete(url, model.Enc_Id);

            //Validamos error
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}