using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la guía del módulo financiero.
    /// Proporciona información paso a paso para nuevos usuarios del sistema.
    /// </summary>
    public class GuiaFinancieraController : BaseController
    {
        /// <summary>
        /// Acción para mostrar la vista principal de la guía financiera.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}