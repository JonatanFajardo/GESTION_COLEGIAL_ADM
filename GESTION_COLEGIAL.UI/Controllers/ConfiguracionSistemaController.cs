using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Configuración del Sistema")]
    public class ConfiguracionSistemaController : BaseController
    {
        private readonly ConfiguracionService _service = new ConfiguracionService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAsync(int Con_Id, string Con_Valor)
        {
            var body = new
            {
                Con_Id = Con_Id,
                Con_Valor = Con_Valor,
                UsuarioModifica = GetUsuarioId()
            };
            bool error = await _service.UpdateAsync(body);
            if (error)
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
