using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Directorio de Contacto")]
    public class DirectorioContactoController : BaseController
    {
        private readonly AlumnosDirectorioService _directorio = new AlumnosDirectorioService();
        private readonly EncargadosService _encargados = new EncargadosService();

        public ActionResult Index() => View();

        public async Task<ActionResult> ListAlumnosAsync(string Cur_Id = null, string Sec_Id = null)
        {
            var result = await _directorio.ListAsync(Cur_Id, Sec_Id);
            return AjaxResult(result);
        }

        public async Task<ActionResult> ListEncargadosAsync()
        {
            var result = await _encargados.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> CursosDropdown()
        {
            var result = await _directorio.CursosDropdown();
            return AjaxResult(result);
        }

        public async Task<ActionResult> SeccionesDropdown(string Cur_Id = null)
        {
            var result = await _directorio.SeccionesDropdown(Cur_Id);
            return AjaxResult(result);
        }
    }
}
