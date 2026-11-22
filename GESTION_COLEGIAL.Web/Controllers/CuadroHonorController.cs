using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Cuadro de honor: ranking de los mejores alumnos del año en dos niveles (Honor / Honor
    /// Superior) según los umbrales de la Escala de notas. Lo usa dirección.
    /// </summary>
    [SessionManager("Cuadro de honor")]
    public class CuadroHonorController : BaseController
    {
        private readonly EscalaNotasService _service = new EscalaNotasService();
        private readonly NivelesEducativosService _niveles = new NivelesEducativosService();
        private readonly SeccionesService _secciones = new SeccionesService();

        public async Task<ActionResult> Index()
        {
            ViewBag.AnioActual = DateTime.Now.Year;
            var cfg = await _service.ConfigGet();
            ViewBag.UmbralHonor = cfg?.Chc_UmbralHonor ?? 90;
            ViewBag.UmbralSuperior = cfg?.Chc_UmbralHonorSuperior ?? 95;
            return View();
        }

        public async Task<ActionResult> DropdownsAsync()
        {
            var nivelesTask = _niveles.ListAsync();
            var seccionesTask = _secciones.ListAsync();
            await Task.WhenAll(nivelesTask, seccionesTask);

            return Json(new
            {
                niveles = nivelesTask.Result.Select(n => new { id = n.Niv_Id, texto = n.Niv_Descripcion }),
                secciones = seccionesTask.Result.Select(s => new { id = s.Sec_Id, texto = s.Sec_Descripcion })
            });
        }

        // GET: CuadroHonor/ListadoAsync?anio=&nivId=&secId=
        public async Task<ActionResult> ListadoAsync(int anio, int? nivId, int? secId)
        {
            var result = await _service.CuadroHonor(anio, nivId, secId);
            return AjaxResult(result);
        }
    }
}
