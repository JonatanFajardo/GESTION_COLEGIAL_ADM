using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Cuaderno de notas")]
    public class NotasAdmController : BaseController
    {
        private readonly NotasAdmService  _service   = new NotasAdmService();
        private readonly ParcialesService _parciales = new ParcialesService();
        private readonly CierresParcialService _cierres = new CierresParcialService();

        public ActionResult Index() => View();

        // Horarios que el usuario puede capturar (la API/SP filtra: un docente solo ve los suyos;
        // dirección y administración ven todas las secciones) + catálogo de parciales.
        public async Task<ActionResult> DropdownsAsync()
        {
            var horariosTask  = _service.HorariosAsync(GetUsuarioId());
            var parcialesTask = _parciales.ListAsync();
            await Task.WhenAll(horariosTask, parcialesTask);

            var horarios  = horariosTask.Result ?? new System.Collections.Generic.List<dynamic>();
            var parciales = parcialesTask.Result;

            return Json(new
            {
                horarios = horarios.Select(h => new
                {
                    id     = (int)h.Hor_Id,
                    sem    = (int)h.Sem_Id,
                    semTxt = (string)h.Sem_Descripcion,
                    anio   = (int)h.Anio,
                    texto  = ((string)h.Cun_Descripcion).Trim() + " " + ((string)h.Sec_Descripcion).Trim() + " — " + ((string)h.Mat_Nombre).Trim()
                             + " (" + ((string)h.Cur_Nombre).Trim() + ", " + (int)h.Alumnos + " alumnos)"
                }),
                parciales = parciales.Select(p => new { id = p.Pac_Id, texto = p.Pac_Descripcion }),
            });
        }

        // GET: NotasAdm/ListAsync?Hor_Id=&Par_Id= -> alumnos activos de la sección con su nota.
        public async Task<ActionResult> ListAsync(int Hor_Id, int Par_Id)
        {
            string json = await _service.ListRawAsync(Hor_Id, Par_Id, GetUsuarioId());
            JToken token;
            try { token = JToken.Parse(json); }
            catch { return Json(new { success = false, message = "No se pudo leer la lista de alumnos." }); }

            if (token is JObject obj)
            {
                string msg = (string)obj["Message"] ?? (string)obj["message"] ?? "No se pudo cargar la lista de alumnos.";
                return Json(new { success = false, message = msg });
            }
            return Content(json, "application/json");
        }

        // Cierre de parciales (script 075): un parcial cerrado por Dirección es solo lectura.
        private static string MensajeCerrado(CierreParcialPeriodoViewModel p) =>
            $"El {p.Parcial?.ToLower()} del {p.Semestre?.ToLower()} de {p.Anio} está cerrado: ya no se pueden registrar, "
            + "cambiar ni quitar notas. Si hace falta una corrección, pida a Dirección que lo reabra.";

        // GET: NotasAdm/PeriodoAsync?anio=&semId=&pacId= -> ¿el parcial está cerrado?
        public async Task<ActionResult> PeriodoAsync(int anio, int semId, int pacId)
        {
            var p = await _cierres.PeriodoAsync(anio, semId, pacId);
            bool cerrado = p != null && p.EstaCerrado;
            return Json(new
            {
                cerrado,
                publicado = p?.Publicado ?? false,
                mensaje = cerrado ? MensajeCerrado(p) : null
            });
        }

        // Guarda la nota de un alumno: crea la que no existe o actualiza la existente.
        // El procedimiento revisa dueño del horario, alumno de la sección, rango 0-100 y parcial cerrado.
        [HttpPost]
        public async Task<ActionResult> GuardarNota(int Hor_Id, int Alu_Id, int Par_Id, decimal Not_Nota, int Anio, int Sem_Id)
        {
            if (Not_Nota < 0 || Not_Nota > 100 || Not_Nota != decimal.Truncate(Not_Nota))
                return Json(new { success = false, message = "La nota debe ser un número entero entre 0 y 100." });

            // Parcial cerrado: se rechaza aquí con el motivo (la API y el procedimiento lo vuelven a revisar).
            var periodo = await _cierres.PeriodoAsync(Anio, Sem_Id, Par_Id);
            if (periodo != null && periodo.EstaCerrado)
                return Json(new { success = false, cerrado = true, message = MensajeCerrado(periodo) });

            var r = await _service.GuardarAsync(new { Alu_Id, Hor_Id, Par_Id, Not_Nota, UsuarioRegistra = GetUsuarioId() });
            if (r.Error)
                return Json(new { success = false, cerrado = (r.Mensaje ?? "").Contains("está cerrado"), message = r.Mensaje });
            return Json(new { success = true });
        }
    }
}
