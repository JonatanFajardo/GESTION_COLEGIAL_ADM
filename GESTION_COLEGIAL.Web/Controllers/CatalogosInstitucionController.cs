using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub unificado que consolida los 5 catálogos base de Institución
    /// (Modalidades, Niveles Educativos, Parciales, Secciones, Semestres)
    /// en una sola pantalla con tabs y vista de cards/tabla.
    /// El CRUD real vive en los controladores originales — este hub sólo orquesta la UI.
    /// </summary>
    [SessionManager("Estructura Académica")]
    public class CatalogosInstitucionController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
