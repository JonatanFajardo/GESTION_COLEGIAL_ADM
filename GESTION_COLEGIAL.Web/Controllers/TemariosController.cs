using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Temario y avance del programa (espejo de Skuul "syllabus"): la biblioteca de temarios (con copiar),
    /// el temario (temas por semana, avance por sección, imprimir) y el resumen de Dirección.
    /// Permisos:
    ///   "Temario y avance"     ver los temarios de las materias que imparte y marcar el avance de SUS secciones.
    ///   "Editar temarios"      crear, copiar y editar temarios (el docente, solo los de sus materias).
    ///   "Supervisar temarios"  ver y editar todos, marcar cualquier sección y el resumen de Dirección.
    /// La API recibe el usuario de la sesión y su procedimiento decide el alcance (nunca solo se ocultan botones).
    /// </summary>
    [SessionManager("Temario y avance")]
    public class TemariosController : BaseController
    {
        public const string PantallaEditar = "Editar temarios";
        public const string PantallaSupervisar = "Supervisar temarios";

        private readonly TemariosService _service = new TemariosService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(string pantalla) => EsAdmin() || TienePantalla(pantalla);
        private bool PuedeEditar() => Puede(PantallaEditar);
        private bool PuedeSupervisar() => Puede(PantallaSupervisar);

        private int UsuId() => int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : 0;

        /// <summary>Quién escribe: siempre desde la sesión (lo que mande el navegador se descarta).</summary>
        private T Escritor<T>(T model) where T : TemarioEscritorViewModel
        {
            model.Usuario = UsuId();
            return model;
        }

        private static object Error(string mensaje, string campo = null) => new { success = false, field = campo, message = mensaje };

        private static int AnioActual() => HoraHonduras.Ahora.Year;

        private static string TextoAviso(string aviso, int cantidad) => aviso switch
        {
            "creado" => "Se creó el temario. Agregue los temas por semana o traiga los de otro temario.",
            "copiado" => $"Se copió el temario con {cantidad} temas. Revise las semanas y ajuste lo que cambie este período.",
            "traidos" => $"Se agregaron {cantidad} temas al final del temario.",
            "editado" => "Se guardaron los datos del temario.",
            _ => null
        };

        // ───────────────────────────── Biblioteca ─────────────────────────────

        public async Task<ActionResult> Index()
        {
            int usu = UsuId();
            return View(new TemariosIndexViewModel
            {
                PuedeEditar = PuedeEditar(),
                PuedeSupervisar = PuedeSupervisar(),
                AnioActual = AnioActual(),
                Niveles = await _service.OpcionesAsync(usu, "nivel"),
                Semestres = await _service.OpcionesAsync(usu, "semestre"),
                Anios = await _service.OpcionesAsync(usu, "anio")
            });
        }

        /// <summary>Los temarios que la persona puede leer; la pantalla filtra en el navegador.</summary>
        [HttpGet]
        public async Task<ActionResult> ListAsync() => AjaxResult(await _service.ListAsync(UsuId()));

        /// <summary>
        /// Opciones de los filtros en cascada (Nivel → Grado → Curso → Materia). Con anio salen del horario
        /// de ese año (formulario "Nuevo temario"); sin anio, de todos los años (filtros de la biblioteca).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> OpcionesAsync(string tipo, int? nivId, int? cunId, int? curId, int? anio)
        {
            if (tipo == "grado" && !(nivId > 0)) return AjaxResult(new object[0]);
            if ((tipo == "curso" || tipo == "materia") && !(cunId > 0)) return AjaxResult(new object[0]);
            return AjaxResult(await _service.OpcionesAsync(UsuId(), tipo, nivId, cunId, curId, anio));
        }

        private static object ValidarNuevo(TemarioGuardarViewModel m)
        {
            if (m.Tem_Anio < 2000 || m.Tem_Anio > 2100) return Error("Indique el año del temario.", "tm-anio");
            if (m.Sem_Id <= 0) return Error("Elija el semestre.", "tm-semestre");
            if (m.Cun_Id <= 0) return Error("Elija el grado.", "tm-grado");
            if (m.Mat_Id <= 0) return Error("Elija la materia.", "tm-materia");
            return ValidarDatos(m);
        }

        private static object ValidarDatos(TemarioGuardarViewModel m)
        {
            if (string.IsNullOrWhiteSpace(m.Tem_Nombre)) return Error("Escriba el nombre del temario.", "tm-nombre");
            if (m.Tem_Nombre.Trim().Length > 150) return Error("El nombre admite hasta 150 caracteres.", "tm-nombre");
            if ((m.Tem_Descripcion ?? "").Trim().Length > 2000) return Error("La descripción admite hasta 2000 caracteres.", "tm-descripcion");
            if (!m.Tem_FechaInicio.HasValue) return Error("Indique cuándo empiezan las clases del semestre.", "tm-inicio");
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearAsync([FromBody] TemarioGuardarViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para crear temarios."));
            if (model == null) return Json(Error("No llegaron datos."));
            var problema = ValidarNuevo(model);
            if (problema != null) return Json(problema);
            if (model.Tem_FechaInicio.Value.Year != model.Tem_Anio)
                return Json(Error("El inicio de clases debe ser una fecha del mismo año del temario.", "tm-inicio"));

            model.Tem_Id = 0;
            TemarioResultadoViewModel r = await _service.CrearAsync(Escritor(model));
            if (r == null || !r.Ok || r.Tem_Id == 0) return Json(Error(r?.Mensaje ?? "No se pudo crear el temario."));
            return Json(new { success = true, id = r.Tem_Id, redirect = Url.Action("Temario", new { id = r.Tem_Id, aviso = "creado" }) });
        }

        /// <summary>
        /// Copiar un temario (la "biblioteca"): sin destino crea uno nuevo para otro semestre o año; con destino
        /// trae los temas al final de ese temario.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CopiarAsync([FromBody] TemarioCopiarViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para copiar temarios."));
            if (model == null || model.Tem_IdOrigen <= 0) return Json(Error("Elija el temario que quiere copiar.", "tm-origen"));
            if (!model.Tem_IdDestino.HasValue)
            {
                if (!(model.Tem_Anio >= 2000 && model.Tem_Anio <= 2100)) return Json(Error("Indique el año del nuevo temario.", "tm-copia-anio"));
                if (!(model.Sem_Id > 0)) return Json(Error("Elija el semestre del nuevo temario.", "tm-copia-semestre"));
                if (!model.Tem_FechaInicio.HasValue) return Json(Error("Indique cuándo empiezan las clases.", "tm-copia-inicio"));
                if (model.Tem_FechaInicio.Value.Year != model.Tem_Anio)
                    return Json(Error("El inicio de clases debe ser una fecha del mismo año del temario.", "tm-copia-inicio"));
                if ((model.Tem_Nombre ?? "").Trim().Length > 150) return Json(Error("El nombre admite hasta 150 caracteres.", "tm-copia-nombre"));
            }

            TemarioResultadoViewModel r = await _service.CopiarAsync(Escritor(model));
            if (r == null || !r.Ok || r.Tem_Id == 0) return Json(Error(r?.Mensaje ?? "No se pudo copiar el temario."));
            string aviso = model.Tem_IdDestino.HasValue ? "traidos" : "copiado";
            return Json(new { success = true, id = r.Tem_Id, cantidad = r.Cantidad, redirect = Url.Action("Temario", new { id = r.Tem_Id, aviso, n = r.Cantidad }) });
        }

        // ───────────────────────────── El temario ─────────────────────────────

        /// <summary>/temarios/temario/5?tab=avance</summary>
        public async Task<ActionResult> Temario(int id, string tab = null, string aviso = null, int n = 0)
        {
            int usu = UsuId();
            TemarioViewModel t = await _service.FindAsync(id, usu);
            if (t == null) return RedirectToAction("Index");
            return View(new TemarioPaginaViewModel
            {
                Temario = t,
                PuedeCrear = PuedeEditar(),
                Tab = tab == "avance" ? "avance" : "temas",
                Aviso = TextoAviso(aviso, n),
                Hoy = HoraHonduras.Ahora.ToString("yyyy-MM-dd"),
                Semestres = await _service.OpcionesAsync(usu, "semestre"),
                Anios = await _service.OpcionesAsync(usu, "anio")
            });
        }

        /// <summary>Temas, secciones y marcas de avance del temario (la pantalla los pinta y los recarga tras cada cambio).</summary>
        [HttpGet]
        public async Task<ActionResult> DatosAsync(int id)
        {
            int usu = UsuId();
            TemarioViewModel t = await _service.FindAsync(id, usu);
            if (t == null) return Json(new { ok = false });
            var temas = _service.TemasAsync(id, usu);
            var grupos = _service.GruposAsync(id, usu);
            var avance = _service.AvanceAsync(id, usu);
            await Task.WhenAll(temas, grupos, avance);
            return Json(new
            {
                ok = true,
                temario = new { t.Tem_Id, t.SemanaActual, t.Temas, t.Secciones, t.AvancePct, t.SeccionesAtrasadas, t.PuedeEditar, t.PuedeSupervisar },
                temas = temas.Result,
                grupos = grupos.Result,
                avance = avance.Result.Select(a => new
                {
                    a.Tet_Id, a.Cur_Id, a.Sec_Id, a.Mda_Id, a.Tav_Estado,
                    Tav_Fecha = a.Tav_Fecha?.ToString("yyyy-MM-dd"), a.Tav_Nota, a.Usuario
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarAsync([FromBody] TemarioGuardarViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para editar temarios."));
            if (model == null || model.Tem_Id <= 0) return Json(Error("No llegaron datos."));
            var problema = ValidarDatos(model);
            if (problema != null) return Json(problema);

            TemarioResultadoViewModel r = await _service.EditarAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudieron guardar los cambios del temario."));
            return Json(new { success = true, redirect = Url.Action("Temario", new { id = model.Tem_Id, aviso = "editado" }) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarAsync([FromBody] TemarioEliminarViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para quitar temarios."));
            if (model == null || model.Tem_Id <= 0) return Json(Error("No llegaron datos."));
            TemarioResultadoViewModel r = await _service.EliminarAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo quitar el temario."));
            return Json(new { success = true, redirect = Url.Action("Index") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarTemaAsync([FromBody] TemarioTemaGuardarViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para editar temarios."));
            if (model == null || (model.Tet_Id <= 0 && model.Tem_Id <= 0)) return Json(Error("No llegaron datos."));
            if (model.Tet_Semana < 1 || model.Tet_Semana > 30) return Json(Error("La semana debe ser un número del 1 al 30.", "tm-tema-semana"));
            if (string.IsNullOrWhiteSpace(model.Tet_Titulo)) return Json(Error("Escriba el título del tema.", "tm-tema-titulo"));
            if (model.Tet_Titulo.Trim().Length > 200) return Json(Error("El título admite hasta 200 caracteres.", "tm-tema-titulo"));
            if ((model.Tet_Objetivos ?? "").Trim().Length > 2000) return Json(Error("Los objetivos admiten hasta 2000 caracteres.", "tm-tema-objetivos"));
            if ((model.Tet_Contenido ?? "").Trim().Length > 4000) return Json(Error("El contenido admite hasta 4000 caracteres.", "tm-tema-contenido"));
            if ((model.Tet_Recursos ?? "").Trim().Length > 1000) return Json(Error("Los recursos admiten hasta 1000 caracteres.", "tm-tema-recursos"));

            TemarioResultadoViewModel r = await _service.GuardarTemaAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo guardar el tema."));
            return Json(new { success = true, id = r.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoverTemaAsync([FromBody] TemarioTemaAccionViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para editar temarios."));
            if (model == null || model.Tet_Id <= 0 || (model.Direccion != "arriba" && model.Direccion != "abajo")) return Json(Error("No llegaron datos."));
            TemarioResultadoViewModel r = await _service.MoverTemaAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo mover el tema."));
            return Json(new { success = true, movido = r.Cantidad > 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarTemaAsync([FromBody] TemarioTemaAccionViewModel model)
        {
            if (!PuedeEditar()) return Json(Error("No tiene permiso para editar temarios."));
            if (model == null || model.Tet_Id <= 0) return Json(Error("No llegaron datos."));
            TemarioResultadoViewModel r = await _service.QuitarTemaAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo quitar el tema."));
            return Json(new { success = true });
        }

        /// <summary>Marcar el avance: basta ver la pantalla; el procedimiento exige que dé esa materia a esa sección (o supervisar).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarcarAvanceAsync([FromBody] TemarioAvanceGuardarViewModel model)
        {
            if (model == null || model.Tet_Id <= 0 || model.Cur_Id <= 0 || model.Sec_Id <= 0 || model.Mda_Id <= 0)
                return Json(Error("No llegó el tema o la sección."));
            if (!CatalogoTemario.EstadosAvance.Contains(model.Tav_Estado))
                return Json(Error("Elija si el tema se dio, se dio en parte o se omitió.", "tm-av-estado"));
            if (model.Tav_Estado != "omitido")
            {
                if (!model.Tav_Fecha.HasValue) return Json(Error("Indique la fecha en que se dio el tema.", "tm-av-fecha"));
                if (model.Tav_Fecha.Value.Date > HoraHonduras.Ahora.Date) return Json(Error("La fecha no puede ser futura.", "tm-av-fecha"));
            }
            else model.Tav_Fecha = null;
            if ((model.Tav_Nota ?? "").Trim().Length > 1000) return Json(Error("La nota de clase admite hasta 1000 caracteres.", "tm-av-nota"));

            TemarioResultadoViewModel r = await _service.MarcarAvanceAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo guardar el avance."));
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarAvanceAsync([FromBody] TemarioAvanceGuardarViewModel model)
        {
            if (model == null || model.Tet_Id <= 0 || model.Cur_Id <= 0 || model.Sec_Id <= 0 || model.Mda_Id <= 0)
                return Json(Error("No llegó el tema o la sección."));
            TemarioResultadoViewModel r = await _service.QuitarAvanceAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo quitar la marca de avance."));
            return Json(new { success = true });
        }

        /// <summary>Hoja tamaño carta: /temarios/imprimir/5</summary>
        public async Task<ActionResult> Imprimir(int id)
        {
            int usu = UsuId();
            TemarioViewModel t = await _service.FindAsync(id, usu);
            if (t == null) return RedirectToAction("Index");
            var modelo = new TemarioImprimirViewModel
            {
                Temario = t,
                Temas = await _service.TemasAsync(id, usu),
                Impreso = HoraHonduras.Ahora
            };
            try
            {
                var inst = new InstitutoConfigService().Load(RutasApp.MapPath("~/App_Data/instituto-config.json"));
                modelo.Institucion = inst?.Nombre;
                modelo.Direccion = inst?.Direccion;
            }
            catch { /* sin membrete: la hoja sigue sirviendo */ }
            return View(modelo);
        }

        // ───────────────────────────── Resumen de Dirección ─────────────────────────────

        public async Task<ActionResult> Resumen()
        {
            if (!PuedeSupervisar()) return RedirectToAction("SinAcceso", "Account");
            int usu = UsuId();
            return View(new TemariosResumenPaginaViewModel
            {
                PuedeEditar = PuedeEditar(),
                AnioActual = AnioActual(),
                Niveles = await _service.OpcionesAsync(usu, "nivel"),
                Semestres = await _service.OpcionesAsync(usu, "semestre"),
                Anios = await _service.OpcionesAsync(usu, "anio")
            });
        }

        [HttpGet]
        public async Task<ActionResult> ResumenAsync(int? anio, int? semId)
        {
            if (!PuedeSupervisar()) return AjaxResult(new object[0]);
            return AjaxResult(await _service.ResumenAsync(UsuId(), anio, semId));
        }

        [HttpGet]
        public async Task<ActionResult> SinTemarioAsync(int? anio, int? semId)
        {
            if (!PuedeSupervisar()) return AjaxResult(new object[0]);
            return AjaxResult(await _service.SinTemarioAsync(UsuId(), anio, semId));
        }
    }
}
