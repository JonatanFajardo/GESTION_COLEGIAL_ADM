using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Centro de anuncios")]
    public class CentroAnunciosController : BaseController
    {
        private readonly CentroAnunciosService _service = new CentroAnunciosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(string Anu_Titulo, string Anu_Contenido, string Anu_FechaExpiracion,
            string Anu_Publico, int? Anu_NivId, int? Anu_CunId, int? Anu_SecId)
        {
            bool err = await _service.CreateAsync(new
            {
                Anu_Titulo,
                Anu_Contenido,
                Anu_FechaExpiracion,
                Anu_Publico = string.IsNullOrWhiteSpace(Anu_Publico) ? "personal" : Anu_Publico,
                Anu_NivId,
                Anu_CunId,
                Anu_SecId,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        // Opciones de la cascada del público: nivel > grado > sección
        public async Task<ActionResult> OpcionesAsync(string tipo, int? padre)
        {
            var result = await _service.OpcionesAsync(tipo, padre);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int Anu_Id)
        {
            bool err = await _service.DeleteAsync(new
            {
                Anu_Id,
                UsuarioModifica = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessDelete);
        }
    }
}
