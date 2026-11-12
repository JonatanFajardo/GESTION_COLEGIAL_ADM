using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Puerta de entrada de Configuración (patrón "School setup" de Skuul): una página-índice con
    /// tarjetas agrupadas que llevan a cada hub. No tiene datos propios ni toca los hubs:
    /// cada hub sigue exigiendo su propio permiso; aquí solo se ocultan las tarjetas sin permiso.
    /// La lista de hubs vive en Models/CatalogoConfiguracion.cs (la usa también el buscador).
    /// </summary>
    [SessionManager]
    public class ConfiguracionController : BaseController
    {
        public ActionResult Index()
        {
            var esAdmin = string.Equals(Session[SesionUsuario.RoleName] as string, "admin", StringComparison.OrdinalIgnoreCase);

            var modelo = new ConfiguracionViewModel
            {
                Grupos = CatalogoConfiguracion.Visibles(esAdmin, TienePantalla)
            };

            return View(modelo);
        }
    }
}
