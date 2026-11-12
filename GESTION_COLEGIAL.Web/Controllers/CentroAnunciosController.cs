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
        public async Task<ActionResult> CreateAsync(string Anu_Titulo, string Anu_Contenido, string Anu_FechaExpiracion)
        {
            bool err = await _service.CreateAsync(new
            {
                Anu_Titulo,
                Anu_Contenido,
                Anu_FechaExpiracion,
                UsuarioRegistra = GetUsuarioId()
            });
            return err ? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
                       : AjaxResult(true,  AlertMessage.AlertMessageCustomType.SuccessInsert);
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
