using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Pase de lista")]
    public class AsistenciaAdmController : BaseController
    {
        private readonly AsistenciaAdmService _service  = new AsistenciaAdmService();
        private readonly HorariosService      _horarios = new HorariosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var horarios = await _horarios.ListAsync();
            return Json(new
            {
                horarios = horarios.Select(h => new {
                    id    = h.Hor_Id,
                    texto = h.Dia_Descripcion + " " + h.Hor_HoraInicioDescripcion + " — " + h.Mat_Nombre + " (" + h.Sec_Descripcion + ")"
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ListAsync(int Hor_Id, string Fecha)
        {
            var result = await _service.ListAsync(Hor_Id, Fecha);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> MarcarAsync(int? Asi_Id, int Hor_Id, int Alu_Id,
            string Asi_Fecha, string Asi_Estado, string Asi_Observacion)
        {
            bool err;
            if (Asi_Id == null || Asi_Id == 0)
                err = await _service.CreateAsync(new { Hor_Id, Alu_Id, Asi_Fecha, Asi_Estado, Asi_Observacion, UsuarioRegistra = GetUsuarioId() });
            else
                err = await _service.EditAsync(new { Asi_Id, Asi_Estado, Asi_Observacion, UsuarioModifica = GetUsuarioId() });

            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }
    }
}
