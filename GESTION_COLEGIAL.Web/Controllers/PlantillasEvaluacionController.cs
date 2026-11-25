using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Plantillas de evaluación (notas ponderadas, espejo de RosarioSIS "Assignment Types" y de las plantillas de
    /// Skuul). Dirección define, por año y nivel (y si hace falta por materia), los tipos de evaluación con su peso
    /// en % (suman 100) con los que se calcula la nota de cada parcial desde las actividades del docente. Pestaña
    /// "Uso en los cuadernos": qué materias y secciones ya califican con actividades (las notas faltantes se ven
    /// en Cierre de parciales).
    /// Permiso: "Plantillas de evaluación" (admin, director). La API lo vuelve a revisar en el procedimiento.
    /// </summary>
    [SessionManager("Plantillas de evaluación")]
    public class PlantillasEvaluacionController : BaseController
    {
        private readonly EvaluacionesService _service = new EvaluacionesService();
        private readonly SemestresService _semestres = new SemestresService();
        private readonly ParcialesService _parciales = new ParcialesService();

        private static object Error(string mensaje) => new { success = false, message = mensaje };

        /// <summary>/plantillasevaluacion?anio=2026 (sin año: el año lectivo activo).</summary>
        public async Task<ActionResult> Index(int? anio)
        {
            int usu = GetUsuarioId();
            var aniosTask = _service.OpcionesAsync(usu, "anio");
            var semTask = _semestres.ListAsync();
            var parTask = _parciales.ListAsync();
            await Task.WhenAll(aniosTask, semTask, parTask);

            var anios = aniosTask.Result;
            int actual = anios.FirstOrDefault(a => a.EsActual)?.Id ?? DateTime.Today.Year;
            int elegido = anio.HasValue && anios.Any(a => a.Id == anio.Value) ? anio.Value : actual;
            if (!anios.Any(a => a.Id == elegido))
                anios.Insert(0, new EvaluacionOpcionViewModel { Id = elegido, Texto = elegido.ToString(), EsActual = elegido == actual });

            return View(new PlantillasEvaluacionIndexViewModel
            {
                Anios = anios,
                Anio = elegido,
                AnioActual = actual,
                Semestres = (semTask.Result ?? Enumerable.Empty<SemestreViewModel>())
                    .Select(s => new EvaluacionOpcionViewModel { Id = s.Sem_Id, Texto = (s.Sem_Descripcion ?? "").Trim() }).ToList(),
                Parciales = (parTask.Result ?? Enumerable.Empty<ParcialViewModel>())
                    .Select(p => new EvaluacionOpcionViewModel { Id = p.Pac_Id, Texto = (p.Pac_Descripcion ?? "").Trim() }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult> ListAsync(int anio) => AjaxResult(await _service.PlantillasAsync(anio, GetUsuarioId()));

        /// <summary>tipo = nivel | materia (del año y nivel, según el horario).</summary>
        [HttpGet]
        public async Task<ActionResult> OpcionesAsync(string tipo, int? anio, int? nivId)
        {
            if (tipo != "nivel" && tipo != "materia") return AjaxResult(Array.Empty<object>());
            return AjaxResult(await _service.OpcionesAsync(GetUsuarioId(), tipo, anio, nivId));
        }

        [HttpGet]
        public async Task<ActionResult> AvanceAsync(int anio, int semId, int pacId) =>
            AjaxResult(await _service.AvanceAsync(anio, semId, pacId, GetUsuarioId()));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarAsync([FromBody] PlantillaEvaluacionGuardarViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            model.Nombre = string.IsNullOrWhiteSpace(model.Nombre) ? null : model.Nombre.Trim();
            model.Descripcion = string.IsNullOrWhiteSpace(model.Descripcion) ? null : model.Descripcion.Trim();
            if (model.Nombre == null) return Json(Error("Escriba el nombre de la plantilla."));
            if (model.Nombre.Length > 100) return Json(Error("El nombre admite hasta 100 caracteres."));
            if (model.Descripcion != null && model.Descripcion.Length > 500) return Json(Error("La descripción admite hasta 500 caracteres."));
            if (model.Anio < 2000 || model.Anio > 2100) return Json(Error("Elija el año lectivo."));
            if (string.IsNullOrWhiteSpace(model.Tipos) || model.Tipos.Length > 20_000) return Json(Error("Agregue los tipos de evaluación."));
            if (model.Niv_Id <= 0) model.Niv_Id = null;
            if (model.Mat_Id <= 0) model.Mat_Id = null;
            model.Usuario = GetUsuarioId();
            bool nueva = model.Ple_Id <= 0;

            var r = await _service.GuardarPlantillaAsync(model);
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo guardar la plantilla."));
            string mensaje = nueva ? "Plantilla creada." : "Plantilla guardada.";
            if (r.Sincronizados > 0) mensaje += $" Se aplicó a {r.Sincronizados} cuaderno(s) con el parcial abierto.";
            if (r.Conservados > 0) mensaje += $" {r.Conservados} cuaderno(s) de parciales cerrados conservan sus pesos.";
            if (r.NotasEscritas > 0) mensaje += $" Se recalcularon {r.NotasEscritas} nota(s) del parcial.";
            return Json(new { success = true, message = mensaje, id = r.Id, aviso = r.AvisoNotas });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarAsync([FromBody] PlantillaEvaluacionQuitarViewModel model)
        {
            if (model == null || model.Ple_Id <= 0) return Json(Error("Elija la plantilla."));
            model.Usuario = GetUsuarioId();
            var r = await _service.QuitarPlantillaAsync(model);
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo quitar la plantilla."));
            return Json(new { success = true, message = "Plantilla quitada." });
        }
    }
}
