using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class CursosNombresController : BaseController
    {
        CursosNombresService cursosNombresService = new CursosNombresService();
        // GET: CursosNombres
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            var result = await cursosNombresService.List();
            return AjaxResult(result);
        }

        public async Task<ActionResult> Find(int id)
        {
            var result = await cursosNombresService.Find(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CursoNombreViewModel model)
        {
            if (model.Cno_Id == 0)
            {
                bool result = await cursosNombresService.Create(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await cursosNombresService.Edit(model);

                //Validamos error
                if (result)
                {
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                }

                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Exist(int? Cno_Id, string Cno_Descripcion)
        {
            //Validaciones.
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Cno_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
            {
                return Json(validationModal.RequestMessage);
            }

            //Envío de datos.
            var result = await cursosNombresService.Exist(Cno_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Cno_Id;
                return (firstValue == Cno_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CursoNombreViewModel model)
        {
            bool result = await cursosNombresService.Delete(model.Cno_Id);

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
        public async Task<CursoNombreViewModel> Load(CursoNombreViewModel model)
        {
            var result = await cursosNombresService.Load(model);
            return model;
        }

    }
}