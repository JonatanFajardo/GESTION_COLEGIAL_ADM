using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub consolidado para catálogos globales de tipo enum:
    /// Estados, Parentescos, Títulos y Cargos.
    ///
    /// A diferencia del hub de Estructura Académica (que usa una tabla por tab),
    /// esta pantalla usa UNA SOLA tabla unificada con filtro por chips
    /// (los catálogos son triviales y casi nunca cambian).
    ///
    /// El CRUD se hace contra los controladores dueños de cada catálogo:
    ///   /Estados/*, /Parentescos/*, /Titulos/*, /Cargos/*
    /// Este controller solo renderiza la vista principal.
    /// </summary>
    [SessionManager("Catálogos Comunes")]
    public class CatalogosComunesController : BaseController
    {
        /// <summary>
        /// Vista principal del hub de catálogos comunes.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}
