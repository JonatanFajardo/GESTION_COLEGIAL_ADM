using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Collections.Generic;
using System.Linq;
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
        /// Muestra la vista del índice.
        /// </summary>
        /// <returns>La vista del índice.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>El resultado en formato Ajax.</returns>
        public async Task<ActionResult> HomeAndChartsList()
        {
            var result = await homeAndChartsService.HomeAndCharts();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>El resultado en formato Ajax.</returns>
        public async Task<ActionResult> ObtenerCantidadAlumnosPorCursoList()
        {
            var result = await homeAndChartsService.ObtenerCantidadAlumnosPorCursoList();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>El resultado en formato Ajax.</returns>
        public async Task<ActionResult> ObtenerPromedioCursoUltimosAnios()
        {
            var result = await homeAndChartsService.ObtenerPromedioCursoUltimosAnios();
            return AjaxResult(result);
        }
    }
}
