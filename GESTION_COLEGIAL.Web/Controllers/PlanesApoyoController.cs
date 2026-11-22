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
    /// Plan de apoyo al alumno (espejo de Skuul "support plans"), en 3 pantallas: el directorio, "Nuevo plan"
    /// y el plan (datos, pasos, notas, historial, imprimir). Dirección abre el plan para un alumno en riesgo;
    /// el docente responsable registra el avance de lo que le toca.
    /// Permisos:
    ///   "Planes de apoyo"            ver (admin, director, Docente). Sin "Gestionar" solo se ven los planes
    ///                                propios (responsable, creador o con un paso asignado): se marcan SUS
    ///                                pasos y se escriben notas.
    ///   "Gestionar planes de apoyo"  ver todos, crear, editar, cambiar estado y pasos de cualquiera.
    ///   "Ver planes confidenciales"  leer los confidenciales aunque no sean propios.
    /// La API recibe estos permisos y su procedimiento vuelve a aplicarlos (nunca solo se ocultan botones).
    /// </summary>
    [SessionManager("Planes de apoyo")]
    public class PlanesApoyoController : BaseController
    {
        public const string PantallaGestionar = "Gestionar planes de apoyo";
        public const string PantallaConfidenciales = "Ver planes confidenciales";

        private readonly PlanesApoyoService _service = new PlanesApoyoService();
        private readonly AlumnosService _alumnos = new AlumnosService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(string pantalla) => EsAdmin() || TienePantalla(pantalla);
        private bool PuedeGestionar() => Puede(PantallaGestionar);
        private bool VerConfidenciales() => Puede(PantallaConfidenciales);

        private int? UsuId() =>
            int.TryParse(Session[SesionUsuario.UserId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        /// <summary>Quién escribe: siempre desde la sesión (lo que mande el navegador se descarta).</summary>
        private T Escritor<T>(T model) where T : PlanApoyoEscritorViewModel
        {
            model.Usuario = UsuId();
            model.PuedeGestionar = PuedeGestionar();
            model.VerConfidenciales = VerConfidenciales();
            return model;
        }

        private static string TextoAviso(string aviso) => aviso switch
        {
            "creado" => "Se abrió el plan como borrador. Agregue los pasos y actívelo cuando esté listo.",
            "activado" => "Se abrió el plan y quedó activo.",
            "editado" => "Se guardaron los cambios del plan.",
            "estado" => "Se cambió el estado del plan.",
            "paso" => "Se agregó el paso.",
            "hecho" => "Se marcó el paso como hecho.",
            "quitado" => "Se quitó el paso.",
            "nota" => "Se agregó la nota.",
            _ => null
        };

        private static object Error(string mensaje, string campo = null) => new { success = false, field = campo, message = mensaje };

        // ───────────────────────────── Directorio ─────────────────────────────

        /// <summary>/planesapoyo?estado=activo&amp;categoria=salud&amp;vencidos=true&amp;mios=true</summary>
        public async Task<ActionResult> Index(string estado, string categoria, bool vencidos = false, bool mios = false)
        {
            IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> niveles;
            try { niveles = (await _alumnos.Dropdown(new AlumnoViewModel())).NivelesEducativosList; }
            catch { niveles = null; }

            return View(new PlanesApoyoIndexViewModel
            {
                PuedeGestionar = PuedeGestionar(),
                Niveles = niveles ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                Estado = estado == "abiertos" || estado == "todos" || CatalogoPlanes.ExisteEstado(estado) ? estado : null,
                Categoria = CatalogoPlanes.ExisteCategoria(categoria) ? categoria : null,
                SoloVencidos = vencidos,
                SoloMios = mios
            });
        }

        /// <summary>Los planes que la persona puede leer; la pantalla filtra en el navegador.</summary>
        [HttpGet]
        public async Task<ActionResult> ListAsync() =>
            AjaxResult(await _service.ListAsync(PuedeGestionar(), VerConfidenciales(), UsuId()));

        [HttpGet]
        public async Task<ActionResult> ResumenAsync() =>
            AjaxResult(await _service.ResumenAsync(PuedeGestionar(), VerConfidenciales(), UsuId()));

        // Cascada Nivel → Grado → Sección del filtro (el grado con el mismo servicio que Matrícula).

        [HttpGet]
        public async Task<ActionResult> GradosAsync(int id)
        {
            try
            {
                var r = await _alumnos.CursoNivelesDropdown(id);
                return AjaxResult((r ?? Enumerable.Empty<CursoNivelDropViewModel>()).Select(x => new { Value = x.Cun_Id, Text = x.Cun_Descripcion }));
            }
            catch { return AjaxResult(new object[0]); }
        }

        [HttpGet]
        public async Task<ActionResult> SeccionesAsync(int id) =>
            AjaxResult((await _service.SeccionesAsync(id)).Select(x => new { Value = x.Sec_Id, Text = x.Sec_Descripcion }));

        // ───────────────────────────── Nuevo plan ─────────────────────────────

        /// <summary>/planesapoyo/nuevo?aluId=5&amp;categoria=conducta (llega precargado desde Analítica o Disciplina)</summary>
        public async Task<ActionResult> Nuevo(int? aluId, string categoria)
        {
            if (!PuedeGestionar()) return RedirectToAction("SinAcceso", "Account");
            DateTime hoy = HoraHonduras.Ahora.Date;
            string cat = (categoria ?? "").Trim().ToLowerInvariant().Replace("ó", "o").Replace("é", "e");
            if (cat == "adecuacion curricular") cat = "adecuacion";
            if (cat == "refuerzo academico") cat = "refuerzo";

            return View(new PlanApoyoNuevoPaginaViewModel
            {
                Usuarios = await _service.UsuariosAsync(),
                AluId = aluId > 0 ? aluId : null,
                Categoria = CatalogoPlanes.ExisteCategoria(cat) ? cat : null,
                UsuarioActual = UsuId(),
                Hoy = hoy.ToString("yyyy-MM-dd"),
                RevisionSugerida = hoy.AddDays(30).ToString("yyyy-MM-dd")
            });
        }

        /// <summary>Alumnos activos para el buscador de "Nuevo plan".</summary>
        [HttpGet]
        public async Task<ActionResult> AlumnosAsync()
        {
            if (!PuedeGestionar()) return AjaxResult(new object[0]);
            return AjaxResult(await _service.AlumnosAsync());
        }

        /// <summary>Planes abiertos de un alumno (aviso "ya tiene un plan abierto", sin bloquear).</summary>
        [HttpGet]
        public async Task<ActionResult> AbiertosAlumnoAsync(int aluId)
        {
            if (aluId <= 0) return AjaxResult(new object[0]);
            var r = await _service.AbiertosAsync(new[] { aluId }, PuedeGestionar(), VerConfidenciales(), UsuId());
            return AjaxResult(r.Select(p => new
            {
                p.Apo_Id,
                p.Apo_Titulo,
                Estado = CatalogoPlanes.Estado(p.Apo_Estado),
                Categoria = CatalogoPlanes.Categoria(p.Apo_Categoria),
                Url = Url.Action("Plan", new { id = p.Apo_Id })
            }));
        }

        /// <summary>
        /// Planes abiertos de varios alumnos ("1,2,3") para los botones "Crear plan" / "Ver plan" de la
        /// tabla de riesgo (Analítica académica). Solo para quien gestiona planes.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> AbiertosAsync(string ids)
        {
            if (!PuedeGestionar()) return Json(new { permitido = false, planes = new object[0] });
            var lista = (ids ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => int.TryParse(x.Trim(), out int v) ? v : 0).Where(v => v > 0).Take(500).ToList();
            var r = await _service.AbiertosAsync(lista, true, VerConfidenciales(), UsuId());
            return Json(new
            {
                permitido = true,
                urlNuevo = Url.Action("Nuevo"),
                planes = r.GroupBy(p => p.Alu_Id).Select(g => new
                {
                    aluId = g.Key,
                    apoId = g.First().Apo_Id,
                    titulo = g.First().Apo_Titulo,
                    estado = CatalogoPlanes.Estado(g.First().Apo_Estado),
                    cantidad = g.Count(),
                    url = Url.Action("Plan", new { id = g.First().Apo_Id })
                })
            });
        }

        /// <summary>Mismas reglas que la API; el campo con problema vuelve para marcarlo en el formulario.</summary>
        private static object ValidarDatos(PlanApoyoGuardarViewModel m, bool nuevo)
        {
            if (nuevo && m.Alu_Id <= 0) return Error("Elija el alumno.", "alumno");
            if (!CatalogoPlanes.ExisteCategoria(m.Apo_Categoria)) return Error("Elija la categoría del plan.", "categoria");
            if (string.IsNullOrWhiteSpace(m.Apo_Titulo)) return Error("Escriba el objetivo del plan.", "titulo");
            if (m.Apo_Titulo.Trim().Length > 150) return Error("El objetivo admite hasta 150 caracteres.", "titulo");
            if ((m.Apo_Resumen ?? "").Trim().Length > 5000) return Error("La descripción admite hasta 5000 caracteres.", "resumen");
            if (!m.Apo_FechaInicio.HasValue) return Error("Indique cuándo empieza el plan.", "inicio");
            if (!m.Apo_FechaRevision.HasValue) return Error("Indique cuándo se revisa el plan.", "revision");
            if (m.Apo_FechaRevision.Value.Date < m.Apo_FechaInicio.Value.Date) return Error("La revisión no puede ser antes del inicio del plan.", "revision");
            if (!m.Usu_IdResponsable.HasValue || m.Usu_IdResponsable.Value <= 0) return Error("Elija al responsable del plan.", "responsable");
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearAsync([FromBody] PlanApoyoGuardarViewModel model)
        {
            if (!PuedeGestionar()) return Json(Error("No tiene permiso para abrir planes de apoyo."));
            if (model == null) return Json(Error("No llegaron datos."));
            var problema = ValidarDatos(model, nuevo: true);
            if (problema != null) return Json(problema);

            model.Apo_Id = 0;
            model.Apo_Version = null;
            PlanApoyoResultadoViewModel r = await _service.CrearAsync(Escritor(model));
            if (r == null || !r.Ok || r.Id == 0) return Json(Error(r?.Mensaje ?? "No se pudo abrir el plan."));

            string aviso = r.Estado == "activo" ? "activado" : "creado";
            return Json(new { success = true, id = r.Id, redirect = Url.Action("Plan", new { id = r.Id, aviso }) });
        }

        // ───────────────────────────── El plan ─────────────────────────────

        private Task<PlanApoyoViewModel> Leer(int id) => _service.FindAsync(id, PuedeGestionar(), VerConfidenciales(), UsuId());

        /// <summary>/planesapoyo/plan/5</summary>
        public async Task<ActionResult> Plan(int id, string aviso = null)
        {
            PlanApoyoViewModel plan = await Leer(id);
            if (plan == null) return RedirectToAction("Index");

            bool gestionar = PuedeGestionar(), conf = VerConfidenciales();
            int? usu = UsuId();
            var modelo = new PlanApoyoPaginaViewModel
            {
                Plan = plan,
                PuedeGestionar = gestionar,
                Aviso = TextoAviso(aviso),
                Pasos = await _service.PasosAsync(id, gestionar, conf, usu),
                Notas = await _service.NotasAsync(id, gestionar, conf, usu),
                Historial = await _service.EstadosAsync(id, gestionar, conf, usu),
                Siguientes = gestionar ? CatalogoPlanes.SiguientesDe(plan.Apo_Estado) : new List<OpcionPlan>(),
                Hoy = HoraHonduras.Ahora.ToString("yyyy-MM-dd")
            };
            if (gestionar && plan.EstaAbierto) modelo.Usuarios = await _service.UsuariosAsync();
            return View(modelo);
        }

        /// <summary>Hoja tamaño carta: /planesapoyo/imprimir/5</summary>
        public async Task<ActionResult> Imprimir(int id)
        {
            PlanApoyoViewModel plan = await Leer(id);
            if (plan == null) return RedirectToAction("Index");
            bool gestionar = PuedeGestionar(), conf = VerConfidenciales();
            int? usu = UsuId();
            var modelo = new PlanApoyoImprimirViewModel
            {
                Plan = plan,
                Pasos = await _service.PasosAsync(id, gestionar, conf, usu),
                Notas = await _service.NotasAsync(id, gestionar, conf, usu),
                Historial = await _service.EstadosAsync(id, gestionar, conf, usu),
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

        /// <summary>El plan, si quien gestiona lo puede leer (para las escrituras de gestión).</summary>
        private async Task<PlanApoyoViewModel> PlanGestionable(int apoId)
        {
            if (!PuedeGestionar()) return null;
            return await Leer(apoId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarAsync([FromBody] PlanApoyoGuardarViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await PlanGestionable(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para cambiar este plan."));
            if (!plan.EstaAbierto) return Json(Error("Este plan ya terminó. Reábralo antes de cambiarlo."));
            var problema = ValidarDatos(model, nuevo: false);
            if (problema != null) return Json(problema);

            model.Alu_Id = plan.Alu_Id;
            PlanApoyoResultadoViewModel r = await _service.EditarAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudieron guardar los cambios del plan."));
            return Json(new { success = true, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "editado" }) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstadoAsync([FromBody] PlanApoyoCambiarEstadoViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await PlanGestionable(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para cambiar el estado de este plan."));

            OpcionPlan t = CatalogoPlanes.Transicion(plan.Apo_Estado, model.Estado);
            if (t == null)
                return Json(Error($"Un plan no puede pasar de {CatalogoPlanes.Estado(plan.Apo_Estado)} a {CatalogoPlanes.Estado(model.Estado)}.", "estado"));
            if (t.PideMotivo && string.IsNullOrWhiteSpace(model.Motivo)) return Json(Error("Escriba el motivo del cambio.", "motivo"));
            if ((model.Motivo ?? "").Trim().Length > 500) return Json(Error("El motivo admite hasta 500 caracteres.", "motivo"));

            PlanApoyoResultadoViewModel r = await _service.CambiarEstadoAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo cambiar el estado del plan.", "motivo"));
            return Json(new { success = true, estado = r.Estado, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "estado" }) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarPasoAsync([FromBody] PlanApoyoAgregarPasoViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await PlanGestionable(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para agregar pasos a este plan."));
            if (!plan.EstaAbierto) return Json(Error("Este plan ya terminó. Reábralo antes de agregar un paso.", "paso-descripcion"));
            if (string.IsNullOrWhiteSpace(model.Apa_Descripcion)) return Json(Error("Diga qué tiene que pasar.", "paso-descripcion"));
            if (model.Apa_Descripcion.Trim().Length > 500) return Json(Error("El paso admite hasta 500 caracteres.", "paso-descripcion"));
            if (!model.Usu_IdResponsable.HasValue || model.Usu_IdResponsable <= 0) return Json(Error("Elija al responsable del paso.", "paso-responsable"));
            if (!model.Apa_FechaLimite.HasValue) return Json(Error("Indique la fecha límite del paso.", "paso-fecha"));

            PlanApoyoResultadoViewModel r = await _service.AgregarPasoAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo agregar el paso.", "paso-responsable"));
            return Json(new { success = true, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "paso" }) + "#pa-pasos" });
        }

        /// <summary>Marcar hecho: quien gestiona, cualquier paso; los demás, solo los suyos (lo vuelve a revisar la API).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompletarPasoAsync([FromBody] PlanApoyoPasoAccionViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await Leer(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para ver este plan."));
            if (!plan.EstaAbierto) return Json(Error("Este plan ya terminó. Reábralo antes de cambiar sus pasos."));
            if (!PuedeGestionar())
            {
                var pasos = await _service.PasosAsync(model.Apo_Id, false, VerConfidenciales(), UsuId());
                if (!pasos.Any(p => p.Apa_Id == model.Apa_Id && p.EsMio))
                    return Json(Error("Solo puede marcar los pasos que tiene asignados."));
            }

            PlanApoyoResultadoViewModel r = await _service.CompletarPasoAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo marcar el paso como hecho."));
            return Json(new { success = true, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "hecho" }) + "#pa-pasos" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarPasoAsync([FromBody] PlanApoyoPasoAccionViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await PlanGestionable(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para quitar pasos de este plan."));
            if (!plan.EstaAbierto) return Json(Error("Este plan ya terminó. Reábralo antes de cambiar sus pasos."));

            PlanApoyoResultadoViewModel r = await _service.QuitarPasoAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo quitar el paso."));
            return Json(new { success = true, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "quitado" }) + "#pa-pasos" });
        }

        /// <summary>Notas: cualquiera que lee el plan mientras siga abierto.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarNotaAsync([FromBody] PlanApoyoAgregarNotaViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await Leer(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para ver este plan."));
            if (!plan.EstaAbierto) return Json(Error("Este plan ya terminó. Reábralo antes de escribir una nota.", "nota"));
            if (string.IsNullOrWhiteSpace(model.Apn_Texto)) return Json(Error("Escriba la nota.", "nota"));
            if (model.Apn_Texto.Trim().Length > 5000) return Json(Error("La nota admite hasta 5000 caracteres.", "nota"));

            PlanApoyoResultadoViewModel r = await _service.AgregarNotaAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo agregar la nota.", "nota"));
            return Json(new { success = true, redirect = Url.Action("Plan", new { id = model.Apo_Id, aviso = "nota" }) + "#pa-notas" });
        }

        /// <summary>Eliminar solo un borrador (un plan que arrancó se cancela con motivo).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarAsync([FromBody] PlanApoyoEliminarViewModel model)
        {
            if (model == null) return Json(Error("No llegaron datos."));
            PlanApoyoViewModel plan = await PlanGestionable(model.Apo_Id);
            if (plan == null) return Json(Error("No tiene permiso para eliminar este plan."));
            if (plan.Apo_Estado != "borrador") return Json(Error("Solo se elimina un borrador. Un plan que ya arrancó se cancela con un motivo."));

            PlanApoyoResultadoViewModel r = await _service.EliminarAsync(Escritor(model));
            if (r == null || !r.Ok) return Json(Error(r?.Mensaje ?? "No se pudo eliminar el plan."));
            return Json(new { success = true, redirect = Url.Action("Index") });
        }
    }
}
