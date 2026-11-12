using GESTION_COLEGIAL.Business.Models.Instituto;
using GESTION_COLEGIAL.Business.Services;
using System.Web;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Endpoint que expone/persiste la configuración institucional
    /// (nombre y dirección del instituto) usada en documentos oficiales.
    /// </summary>
    public class InstitutoConfigController : BaseController
    {
        private readonly InstitutoConfigService _service = new InstitutoConfigService();

        private string ConfigPath => RutasApp.MapPath("~/App_Data/instituto-config.json");

        [HttpGet]
        public ActionResult GetAsync()
        {
            var data = _service.Load(ConfigPath);
            return AjaxResult((object)data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAsync(InstitutoConfigViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            model.Nombre = (model.Nombre ?? string.Empty).Trim();
            model.Direccion = (model.Direccion ?? string.Empty).Trim();

            _service.Save(ConfigPath, model);
            return Json(new { success = true, data = model });
        }
    }
}
