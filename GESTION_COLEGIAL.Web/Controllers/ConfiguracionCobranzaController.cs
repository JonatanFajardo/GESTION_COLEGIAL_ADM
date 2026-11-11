using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub unificado que consolida los 5 catálogos base de Cobranza
    /// (Conceptos de Pago, Tarifas, Descuentos, Formas de Pago, Estados de Pago)
    /// en una sola pantalla con tabs y vista de cards/tabla.
    /// El CRUD real vive en los controladores originales — este hub sólo orquesta la UI.
    /// </summary>
    [SessionManager("Configuración de Cobranza")]
    public class ConfiguracionCobranzaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
