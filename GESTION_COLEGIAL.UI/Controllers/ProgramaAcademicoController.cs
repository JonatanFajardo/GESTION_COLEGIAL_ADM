using GESTION_COLEGIAL.UI.Filters;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub unificado que consolida los 4 catálogos que definen la oferta educativa
    /// (Grados Académicos, Cursos, Materias, Duraciones) en una sola pantalla con
    /// tabs, vista de cards/tabla y modales unificados.
    /// El CRUD real vive en los controladores originales — este hub sólo orquesta la UI.
    /// Orden de cascada: Grado Académico → Curso → Materia → Duración.
    /// </summary>
    [SessionManager("Programa Académico")]
    public class ProgramaAcademicoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
