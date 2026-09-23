using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    // NOTA: La pantalla-dashboard "Pagos del Día" (vista Index) fue eliminada por
    // duplicidad/bajo valor. Este controlador se conserva únicamente como endpoint de
    // datos (ListAsync): ArqueosCajaAdm (urlPagosDia) y ConciliacionController consumen
    // el "esperado del sistema" por fecha desde aquí. No re-agregar la vista Index.
    [SessionManager("Pagos del Día")]
    public class PagosDelDiaController : BaseController
    {
        private readonly PagosDelDiaService _service = new PagosDelDiaService();

        public async Task<ActionResult> ListAsync(DateTime? fecha = null)
        {
            var result = await _service.ListAsync(fecha ?? DateTime.Today);
            return AjaxResult(result);
        }
    }
}
