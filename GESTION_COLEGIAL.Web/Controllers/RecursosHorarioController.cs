using GESTION_COLEGIAL.UI.Filters;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Hub consolidado que agrupa los 3 catálogos operativos usados para armar
    /// los horarios semanales: Aulas, Días y Horas.
    ///
    /// A diferencia de otros hubs del proyecto (Estructura Académica) que usan
    /// un layout de tabs, esta pantalla se presenta como accordion vertical
    /// porque son sólo 3 secciones — el accordion permite ver más de una
    /// sección al mismo tiempo si el usuario lo desea.
    ///
    /// Reutiliza los endpoints existentes:
    ///   /Aulas/{ListAsync|FindAsync|CreateAsync|DeleteAsync}
    ///   /Dias/{ListAsync|FindAsync|CreateAsync|DeleteAsync}
    ///   /Horas/{ListAsync|FindAsync|CreateAsync|DeleteAsync}
    /// </summary>
    [SessionManager("Recursos de Horario")]
    public class RecursosHorarioController : BaseController
    {
        // GET: RecursosHorario
        public ActionResult Index()
        {
            return View();
        }
    }
}
