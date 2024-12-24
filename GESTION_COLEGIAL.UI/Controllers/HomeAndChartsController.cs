using GESTION_COLEGIAL.Business.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para gestionar la entidad Alumnos.
    /// </summary>
    public class HomeAndChartsController : BaseController
    {
        private readonly HomeAndChartsService homeAndChartsService = new HomeAndChartsService();

        /// <summary>
        /// Renderiza la vista principal de la aplicación.
        /// </summary>
        /// <returns>La vista principal (Index).</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Recupera y devuelve datos analíticos y gráficos del inicio de forma asíncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con la información analítica de la página de inicio.</returns>
        public async Task<ActionResult> HomeAndChartsList()
        {
            var result = await homeAndChartsService.HomeAndCharts();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la cantidad de alumnos inscritos por curso de forma asíncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con el número de alumnos por curso.</returns>
        public async Task<ActionResult> ObtenerCantidadAlumnosPorCursoList()
        {
            var result = await homeAndChartsService.ObtenerCantidadAlumnosPorCursoList();
            return AjaxResult(result);
        }

        /// <summary>
        /// Calcula y proporciona el promedio de rendimiento de los cursos durante los últimos años de forma asíncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con los promedios de los cursos en los últimos años.</returns>
        public async Task<ActionResult> ObtenerPromedioCursoUltimosAnios()
        {
            var result = await homeAndChartsService.ObtenerPromedioCursoUltimosAnios();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene los datos de las tarjetas informativas que se muestran en la página de inicio de forma asíncrona.
        /// </summary>
        /// <returns>Un resultado en formato Ajax con las tarjetas de información para la página de inicio.</returns>
        public async Task<ActionResult> CardsInHomeList()
        {
            var result = await homeAndChartsService.CardsInHomeList();
            return AjaxResult(result);
        }

    }
}
