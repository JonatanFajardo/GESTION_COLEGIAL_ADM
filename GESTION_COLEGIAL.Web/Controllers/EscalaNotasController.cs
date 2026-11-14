using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Escala de notas: rangos de promedio → concepto para el boletín, más los umbrales del
    /// cuadro de honor. La ve y configura dirección (y admin).
    /// </summary>
    [SessionManager("Escala de notas")]
    public class EscalaNotasController : BaseController
    {
        private readonly EscalaNotasService _service = new EscalaNotasService();

        public ActionResult Index()
        {
            ViewBag.PuedeEditar = TienePantalla("Escala de notas");
            return View();
        }

        // ---- Escala (rangos) ----

        public async Task<ActionResult> ListAsync()
        {
            var result = await _service.ListAsync();
            return AjaxResult(result);
        }

        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await _service.Find(id);
            return AjaxResult(result, true);
        }

        public async Task<ActionResult> DetailAsync(int id)
        {
            var result = await _service.Detail(id);
            return AjaxResult(result, true);
        }

        /// <summary>Devuelve la fila que choca de rango (o null si está libre). Para validar en vivo.</summary>
        public async Task<ActionResult> SolapaAsync(decimal desde, decimal hasta, int? id)
        {
            var result = await _service.Solapa(desde, hasta, id);
            return Json(new { choca = result != null, concepto = result?.Esc_Concepto });
        }

        [HttpPost]
        public async Task<ActionResult> GuardarAsync(EscalaNotaViewModel model)
        {
            // La nota mínima no puede ser mayor que la máxima.
            if (model.Esc_NotaDesde > model.Esc_NotaHasta)
                return Json(new { success = false, message = "La nota mínima del rango no puede ser mayor que la máxima." });

            // El rango no puede solaparse con otro concepto.
            var choque = await _service.Solapa(model.Esc_NotaDesde, model.Esc_NotaHasta, model.Esc_Id);
            if (choque != null)
                return Json(new { success = false, message = $"El rango se solapa con el concepto \"{choque.Esc_Concepto}\" ({choque.Esc_NotaDesde:0.##}–{choque.Esc_NotaHasta:0.##})." });

            bool error = model.Esc_Id == 0
                ? await _service.Create(model)
                : await _service.Edit(model);

            return error
                ? Json(new { success = false, message = "No se pudo guardar el rango." })
                : Json(new { success = true, message = model.Esc_Id == 0 ? "Rango agregado." : "Rango actualizado." });
        }

        [HttpPost]
        public async Task<ActionResult> EliminarAsync(int id)
        {
            bool error = await _service.Delete(id);
            return error
                ? Json(new { success = false, message = "No se pudo eliminar el rango." })
                : Json(new { success = true, message = "Rango eliminado." });
        }

        // ---- Cuadro de honor (umbrales) ----

        public async Task<ActionResult> ConfigAsync()
        {
            var result = await _service.ConfigGet();
            return AjaxResult(result, true);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarConfigAsync(CuadroHonorConfigViewModel model)
        {
            if (model.Chc_UmbralHonor > model.Chc_UmbralHonorSuperior)
                return Json(new { success = false, message = "El umbral de honor no puede ser mayor que el de honor superior." });

            bool error = await _service.ConfigSet(model);
            return error
                ? Json(new { success = false, message = "No se pudo guardar la configuración." })
                : Json(new { success = true, message = "Umbrales del cuadro de honor guardados." });
        }
    }
}
