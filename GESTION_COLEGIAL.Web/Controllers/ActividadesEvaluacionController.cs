using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Actividades del cuaderno de notas (notas ponderadas, script 089 de la API). El docente crea actividades
    /// (examen, tarea, trabajo...) por sección, materia y parcial, pone los puntos de cada alumno y la nota del
    /// parcial se calcula con los pesos de la plantilla de evaluación y se escribe en las notas de siempre.
    /// Mismo permiso que el cuaderno ("Cuaderno de notas": admin, director, Docente). El procedimiento de la API
    /// vuelve a revisar el permiso, que el grupo sea del horario del docente y que el parcial esté abierto.
    /// </summary>
    [SessionManager("Cuaderno de notas")]
    public class ActividadesEvaluacionController : BaseController
    {
        private readonly EvaluacionesService _service = new EvaluacionesService();

        private static object Error(string mensaje) => new { success = false, message = mensaje };

        private ActionResult Responder(EvaluacionRespuestaViewModel r, string general, string exito) =>
            r == null || !r.Ok
                ? Json(Error(r?.Mensaje ?? general))
                : Json(new
                {
                    success = true, message = exito, id = r.Id, cevId = r.Cev_Id, cantidad = r.Cantidad, omitidas = r.Omitidas,
                    notasEscritas = r.NotasEscritas, notasFallidas = r.NotasFallidas, avisoNotas = r.AvisoNotas
                });

        /// <summary>GET /actividadesevaluacion/cuadernoasync?horId=&amp;pacId= -> todo el cuaderno (JSON).</summary>
        [HttpGet]
        public async Task<ActionResult> CuadernoAsync(int horId, int pacId)
        {
            if (horId <= 0 || pacId <= 0) return Json(Error("Elija la materia, la sección y el parcial."));
            string json = await _service.CuadernoRawAsync(horId, pacId, GetUsuarioId());
            JToken token;
            try { token = JToken.Parse(json); }
            catch { return Json(Error("No se pudo leer el cuaderno de actividades.")); }
            if (token is not JObject obj)
                return Json(Error("No se pudo cargar el cuaderno de actividades. Intente de nuevo."));
            if (obj["Error"] != null)
                return Json(Error((string)obj["Message"] ?? "No se pudo cargar el cuaderno de actividades."));
            return Content(json, "application/json");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarActividadAsync([FromBody] ActividadEvaluacionGuardarViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            model.Nombre = string.IsNullOrWhiteSpace(model.Nombre) ? null : model.Nombre.Trim();
            model.Descripcion = string.IsNullOrWhiteSpace(model.Descripcion) ? null : model.Descripcion.Trim();
            if (model.Nombre == null) return Json(Error("Escriba el nombre de la actividad."));
            if (model.Nombre.Length > 150) return Json(Error("El nombre de la actividad admite hasta 150 caracteres."));
            if (model.Descripcion != null && model.Descripcion.Length > 500) return Json(Error("La descripción admite hasta 500 caracteres."));
            if (model.PuntosMax <= 0 || model.PuntosMax > 1000 || decimal.Round(model.PuntosMax, 2) != model.PuntosMax)
                return Json(Error("Los puntos máximos deben ser mayores que 0 y hasta 1000 (máximo 2 decimales)."));
            if (model.Act_Id <= 0 && (model.Hor_Id <= 0 || model.Pac_Id <= 0))
                return Json(Error("Elija la materia, la sección y el parcial."));
            if (model.Act_Id <= 0 && !(model.Cet_Id > 0) && !(model.Plt_Id > 0))
                return Json(Error("Elija el tipo de evaluación de la actividad."));
            if (model.Act_Id > 0 && !(model.Cet_Id > 0))
                return Json(Error("Elija el tipo de evaluación de la actividad."));
            model.Usuario = GetUsuarioId();
            bool nueva = model.Act_Id <= 0;
            var r = await _service.GuardarActividadAsync(model);
            string exito = nueva
                ? (r != null && r.Cantidad > 1 ? $"Actividad creada en {r.Cantidad} secciones." : "Actividad creada.")
                : "Actividad guardada.";
            if (r != null && r.Ok && r.Omitidas > 0)
                exito += $" En {r.Omitidas} sección(es) no se creó (ya tenían una actividad con ese nombre o no se pudo).";
            return Responder(r, "No se pudo guardar la actividad.", exito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarActividadAsync([FromBody] ActividadEvaluacionQuitarViewModel model)
        {
            if (model == null || model.Act_Id <= 0) return Json(Error("Elija la actividad."));
            model.Usuario = GetUsuarioId();
            return Responder(await _service.QuitarActividadAsync(model), "No se pudo quitar la actividad.", "Actividad quitada.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarPuntosAsync([FromBody] PuntosEvaluacionGuardarViewModel model)
        {
            if (model == null || model.Cev_Id <= 0 || string.IsNullOrWhiteSpace(model.Puntos))
                return Json(Error("No hay cambios para guardar."));
            if (model.Puntos.Length > 400_000) return Json(Error("Son demasiados cambios a la vez: guarde por partes."));
            model.Usuario = GetUsuarioId();
            return Responder(await _service.GuardarPuntosAsync(model), "No se pudieron guardar los puntos.", "Puntos guardados.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarNotasAsync(int cevId)
        {
            if (cevId <= 0) return Json(Error("Elija el cuaderno."));
            return Responder(await _service.ActualizarNotasAsync(cevId, GetUsuarioId()), "No se pudieron actualizar las notas del parcial.",
                "Notas del parcial actualizadas.");
        }
    }
}
