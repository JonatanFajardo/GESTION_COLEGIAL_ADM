using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Arqueo de caja")]
    public class ArqueosCajaAdmController : BaseController
    {
        private readonly ArqueosCajaAdmService _service = new ArqueosCajaAdmService();

        public ActionResult Index() => View();

        public async Task<ActionResult> LastAsync()
        {
            var result = await _service.LastAsync(GetUsuarioId());
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(decimal Arq_MontoInicial, decimal Arq_MontoFinal,
            decimal Arq_TotalEfectivo, decimal Arq_TotalTarjeta, string Arq_Observacion)
        {
            bool err = await _service.CreateAsync(new {
                Arq_MontoInicial, Arq_MontoFinal, Arq_TotalEfectivo,
                Arq_TotalTarjeta, Arq_Observacion, UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
