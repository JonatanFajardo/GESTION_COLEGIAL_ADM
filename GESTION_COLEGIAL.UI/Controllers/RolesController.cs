using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Listado de roles")]
    public class RolesController : BaseController
    {
        private readonly RolesService rolesService = new RolesService();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            var result = await rolesService.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await rolesService.Find(id);
            return AjaxResult(result, true);
        }

        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await rolesService.Detail(id);
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(RolViewModel model)
        {
            if (model.Rol_Id == 0)
            {
                bool result = await rolesService.Create(model);
                if (result)
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
            }
            else
            {
                bool result = await rolesService.Edit(model);
                if (result)
                    return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
                return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExistAsync(int? Rol_Id, string Rol_Descripcion)
        {
            ValidationModal validationModal = new ValidationModal();
            validationModal.SendMessage = Rol_Descripcion;
            validationModal.BlankSpaces();
            validationModal.SpecialCharacters();
            if (validationModal.RequestMessage != null)
                return Json(validationModal.RequestMessage);

            var result = await rolesService.Exist(Rol_Descripcion);
            if (result != null)
            {
                int? firstValue = result.Rol_Id;
                return (firstValue == Rol_Id) ? Json(true) : Json(msjExist);
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(RolViewModel model)
        {
            bool result = await rolesService.Delete(model.Rol_Id);
            if (result)
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        // Obtener todas las pantallas disponibles
        public async Task<ActionResult> PantallasListAsync()
        {
            var result = await rolesService.PantallasListAsync();
            return AjaxResult(result);
        }

        // Obtener pantallas asignadas a un rol
        public async Task<ActionResult> PantallasByRolAsync(int id)
        {
            var result = await rolesService.PantallasByRolAsync(id);
            return AjaxResult(result);
        }

        // Guardar pantallas asignadas a un rol
        [HttpPost]
        public async Task<ActionResult> SavePantallasAsync(int rolId, string pantallaIds)
        {
            var model = new RolConPantallasViewModel
            {
                Rol_Id = rolId,
                PantallaIds = new List<int>()
            };

            if (!string.IsNullOrEmpty(pantallaIds))
            {
                model.PantallaIds = pantallaIds.Split(',')
                    .Where(x => int.TryParse(x, out _))
                    .Select(int.Parse)
                    .ToList();
            }

            bool result = await rolesService.SavePantallasAsync(model);
            if (result)
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
