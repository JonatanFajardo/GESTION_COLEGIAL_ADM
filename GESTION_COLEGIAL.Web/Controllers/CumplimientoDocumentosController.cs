using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Cumplimiento de Documentos")]
    public class CumplimientoDocumentosController : BaseController
    {
        private readonly CumplimientoDocumentosService _service  = new CumplimientoDocumentosService();
        private readonly EmpleadosService              _empleados = new EmpleadosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> EmpleadosDropdownAsync()
        {
            var list = await _empleados.ListAsync();
            return Json(list.Select(e => new { id = e.Emp_Id, texto = e.Emp_Nombre }));
        }

        /// <summary>
        /// Devuelve el JSON directamente como Content para evitar la doble
        /// serialización JavaScriptSerializer + JObject (Newtonsoft) que corrompe
        /// los nombres de propiedades PascalCase.
        /// </summary>
        public async Task<ActionResult> ListAsync(int? Emp_Id = null)
        {
            string json = await _service.ListRawAsync(Emp_Id);
            return Content(json, "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> MarcarEntregadoAsync(int Emp_Id, string Documento, bool EsEntregado,
            string FechaEntrega, string Doc_FechaVencimiento, string Observacion)
        {
            bool err = await _service.MarcarEntregadoAsync(new
            {
                Emp_Id,
                Documento,
                EsEntregado,
                FechaEntrega = string.IsNullOrWhiteSpace(FechaEntrega) ? (string)null : FechaEntrega,
                Doc_FechaVencimiento = string.IsNullOrWhiteSpace(Doc_FechaVencimiento) ? (string)null : Doc_FechaVencimiento,
                Observacion,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessUpdate);
        }
    }
}
