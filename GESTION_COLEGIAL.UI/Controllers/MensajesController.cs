using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Mensajes a Encargados")]
    public class MensajesController : BaseController
    {
        private readonly MensajesService _service = new MensajesService();
        private readonly AlumnosService  _alumnos = new AlumnosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> DropdownsAsync()
        {
            var alumnos = await _alumnos.ListAsync();
            return Json(new
            {
                alumnos = alumnos.Select(a => new { id = a.Alu_Id, texto = a.Alu_Nombre })
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ListAsync(string Alu_Id = null)
        {
            var result = await _service.ListAsync(Alu_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int Alu_Id, string Msg_Asunto, string Msg_Contenido)
        {
            bool err = await _service.CreateAsync(new { Alu_Id, Msg_Asunto, Msg_Contenido, UsuarioEnvia = GetUsuarioId() });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int Msg_Id)
        {
            bool err = await _service.DeleteAsync(new { Msg_Id, UsuarioModifica = GetUsuarioId() });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
