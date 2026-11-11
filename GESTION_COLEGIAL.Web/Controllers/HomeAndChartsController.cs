using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para gestionar la entidad Alumnos.
    /// </summary>
    [SessionManager("Home")]
    public class HomeAndChartsController : BaseController
    {
        private readonly HomeAndChartsService homeAndChartsService = new HomeAndChartsService();

        /// <summary>
        /// Renderiza la vista principal de la aplicaci�n.
        /// </summary>
        /// <returns>La vista principal (Index).</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Recupera y devuelve datos anal�ticos y gr�ficos del inicio de forma as�ncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con la informaci�n anal�tica de la p�gina de inicio.</returns>
        public async Task<ActionResult> HomeAndChartsList()
        {
            var result = await homeAndChartsService.HomeAndCharts();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la cantidad de alumnos inscritos por curso de forma as�ncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con el n�mero de alumnos por curso.</returns>
        public async Task<ActionResult> ObtenerCantidadAlumnosPorCursoList()
        {
            var result = await homeAndChartsService.ObtenerCantidadAlumnosPorCursoList();
            return AjaxResult(result);
        }

        /// <summary>
        /// Calcula y proporciona el promedio de rendimiento de los cursos durante los �ltimos a�os de forma as�ncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con los promedios de los cursos en los �ltimos a�os.</returns>
        public async Task<ActionResult> ObtenerPromedioCursoUltimosAnios()
        {
            var result = await homeAndChartsService.ObtenerPromedioCursoUltimosAnios();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene los datos de las tarjetas informativas que se muestran en la p�gina de inicio de forma as�ncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con las tarjetas de informaci�n para la p�gina de inicio.</returns>
        public async Task<ActionResult> CardsInHomeList()
        {
            var result = await homeAndChartsService.CardsInHomeList();
            return AjaxResult(result);
        }

    }
}
