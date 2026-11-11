using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;

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

        // NOTA: La firma se alineó al contrato real de la API (ArqueoCajaRequest):
        //   Usu_Id, Arq_Fecha, Arq_TotalEfectivo, Arq_TotalTransferencia, Arq_TotalTarjeta,
        //   Arq_Observaciones, UsuarioRegistra.
        // La versión anterior enviaba Arq_MontoInicial/Arq_MontoFinal/Arq_Observacion, campos
        // que la API y la tabla finanza.tbArqueosCaja NO tienen (se ignoraban silenciosamente),
        // y omitía Usu_Id, Arq_Fecha y Arq_TotalTransferencia.
        [HttpPost]
        public async Task<ActionResult> CreateAsync(DateTime Arq_Fecha, decimal Arq_TotalEfectivo,
            decimal Arq_TotalTransferencia, decimal Arq_TotalTarjeta, string Arq_Observaciones)
        {
            int usuId = GetUsuarioId();
            bool err = await _service.CreateAsync(new {
                Usu_Id = usuId,
                Arq_Fecha,
                Arq_TotalEfectivo,
                Arq_TotalTransferencia,
                Arq_TotalTarjeta,
                Arq_Observaciones,
                UsuarioRegistra = usuId
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
