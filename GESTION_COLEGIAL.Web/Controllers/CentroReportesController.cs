using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Centro de reportes (espejo del ReportDesk de Skuul): un solo lugar para pedir cualquier reporte
    /// con sus filtros, verlo y bajarlo en Excel o PDF. Cada reporte del catálogo
    /// (<see cref="CatalogoReportes"/>) solo aparece y solo se ejecuta si el usuario tiene el permiso
    /// de su pantalla de origen; eso se comprueba aquí, en el servidor, en cada llamada.
    /// Cada ejecución queda en app.tbReportesGenerados ("Recientes").
    /// </summary>
    [SessionManager("Centro de reportes")]
    public class CentroReportesController : BaseController
    {
        private const string PantallaTodasSecciones = "Ver asistencia de todas las secciones";
        private const int MaxDiasRango = 366;

        private readonly CentroReportesService _service = new CentroReportesService();
        private readonly ReporteAsistenciaService _asistencia = new ReporteAsistenciaService();
        private readonly AniosLectivosService _anios = new AniosLectivosService();
        private readonly ConceptosPagoService _conceptos = new ConceptosPagoService();
        private readonly FormasPagoService _formas = new FormasPagoService();

        private static readonly CultureInfo Cultura = CatalogoReportes.Cultura;

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool Puede(ReporteDefinicion r) => r != null && (EsAdmin() || r.Pantallas.Any(TienePantalla));

        private IEnumerable<ReporteDefinicion> Visibles() => CatalogoReportes.Todos.Where(Puede);

        private int? EmpId() =>
            int.TryParse(Session[SesionUsuario.EmpId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        /// <summary>Asistencia: quien no ve todas las secciones solo ve las suyas (como en Reporte de asistencia).</summary>
        private bool Alcance(string clave, out int? empId)
        {
            empId = null;
            if (!string.Equals(clave, "asistencia-secciones", StringComparison.OrdinalIgnoreCase)) return false;
            if (EsAdmin() || TienePantalla(PantallaTodasSecciones)) return false;
            empId = EmpId();
            return true;
        }

        private ActionResult Respuesta(object o, int status = 200)
        {
            Response.StatusCode = status;
            return Content(JsonConvert.SerializeObject(o), "application/json");
        }

        private ActionResult Error(string mensaje, int status) => Respuesta(new { ok = false, mensaje }, status);

        // ───────────────────────────── Pantalla ─────────────────────────────

        /// <summary>/centroreportes?r=saldos-alumno abre ese reporte.</summary>
        public async Task<ActionResult> Index(string r)
        {
            DateTime hoy = HoraHonduras.Hoy;
            var anio = await _asistencia.AnioAsync(hoy);
            string aniActual = anio != null && anio.Ani_Id > 0 ? anio.Ani_Id.ToString() : "";

            var reportes = Visibles().Select(def => new
            {
                def.Clave, def.Titulo, def.Area, def.Descripcion, def.Icono, def.Origen, def.Columnas, def.Agrupar,
                Parametros = def.Parametros.Select(p => new
                {
                    p.Nombre, p.Etiqueta, p.Tipo, p.Requerido, p.Min, p.Max, p.Opciones, p.Ayuda,
                    Defecto = ResolverDefecto(p.Defecto, hoy, aniActual)
                })
            }).ToList();

            ViewBag.Catalogo = JsonConvert.SerializeObject(new { reportes, meses = CatalogoReportes.Meses },
                new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            ViewBag.Inicial = reportes.Any(x => x.Clave == r) ? r : "";
            return View(reportes.Select(x => (x.Clave, x.Titulo, x.Area, x.Descripcion, x.Icono)).ToList());
        }

        private static string ResolverDefecto(string defecto, DateTime hoy, string aniActual) => defecto switch
        {
            "hoy" => hoy.ToString("yyyy-MM-dd"),
            "inicioMes" => new DateTime(hoy.Year, hoy.Month, 1).ToString("yyyy-MM-dd"),
            "hace30" => hoy.AddDays(-29).ToString("yyyy-MM-dd"),
            "anioActual" => hoy.Year.ToString(),
            "anioAnterior" => (hoy.Year - 1).ToString(),
            "mesActual" => hoy.Month.ToString(),
            "anioLectivoActual" => aniActual,
            _ => defecto ?? ""
        };

        // ───────────────────────────── Opciones de los filtros ─────────────────────────────

        /// <summary>
        /// Opciones de una lista, ya filtradas por su padre: anioLectivo · nivel (aniId) · grado (aniId, nivId)
        /// · seccion (aniId, cunId) · concepto · formaPago. Con clave=asistencia-secciones respeta el alcance del docente.
        /// </summary>
        public async Task<ActionResult> OpcionesAsync(string tipo, string clave, int? aniId, int? nivId, int? cunId)
        {
            IEnumerable<OpcionReporte> lista;
            switch (tipo)
            {
                case "anioLectivo":
                    lista = (await _anios.ListAsync() ?? Enumerable.Empty<AnioLectivoViewModel>())
                        .OrderByDescending(a => a.Ani_Anio)
                        .Select(a => new OpcionReporte { Valor = a.Ani_Id.ToString(), Texto = string.IsNullOrWhiteSpace(a.Ani_Nombre) ? a.Ani_Anio.ToString() : a.Ani_Nombre });
                    break;
                case "nivel":
                case "grado":
                case "seccion":
                    if (!aniId.HasValue) return Respuesta(new { data = Array.Empty<object>() });
                    var filtros = await Filtros(aniId.Value, clave);
                    lista = tipo == "nivel"
                        ? filtros.GroupBy(f => f.Niv_Id).Select(g => new OpcionReporte { Valor = g.Key.ToString(), Texto = g.First().Nivel })
                        : tipo == "grado"
                            ? (nivId.HasValue ? filtros.Where(f => f.Niv_Id == nivId).GroupBy(f => f.Cun_Id)
                                                       .Select(g => new OpcionReporte { Valor = g.Key.ToString(), Texto = g.First().Grado })
                                              : Enumerable.Empty<OpcionReporte>())
                            : (cunId.HasValue ? filtros.Where(f => f.Cun_Id == cunId)
                                                       .Select(f => new OpcionReporte { Valor = f.Sea_Id.ToString(), Texto = f.Seccion })
                                              : Enumerable.Empty<OpcionReporte>());
                    break;
                case "concepto":
                    lista = (await _conceptos.ListAsync() ?? Enumerable.Empty<ConceptoPagoListViewModel>())
                        .Where(c => !c.EsEliminado)
                        .Select(c => new OpcionReporte { Valor = c.ConceptoPagoId.ToString(), Texto = c.Descripcion });
                    break;
                case "formaPago":
                    lista = (await _formas.ListAsync() ?? Enumerable.Empty<FormaPagoListViewModel>())
                        .Where(f => !f.EsEliminado)
                        .Select(f => new OpcionReporte { Valor = f.FormaPagoId.ToString(), Texto = f.Descripcion });
                    break;
                default:
                    return Error("Lista no válida.", 400);
            }
            return Respuesta(new { data = lista.ToList() });
        }

        private async Task<List<AsistenciaFiltroViewModel>> Filtros(int aniId, string clave)
        {
            if (Alcance(clave, out int? empId) && !empId.HasValue) return new List<AsistenciaFiltroViewModel>();
            return await _asistencia.FiltrosAsync(aniId, empId);
        }

        // ───────────────────────────── Ejecutar ─────────────────────────────

        /// <summary>
        /// /centroreportes/ejecutarasync?clave=flujo-caja&amp;anio=2026 — valida permiso y parámetros,
        /// trae las filas, registra la ejecución y devuelve columnas + filas.
        /// </summary>
        public async Task<ActionResult> EjecutarAsync(string clave)
        {
            var def = CatalogoReportes.Buscar(clave);
            if (def == null) return Error("El reporte no existe.", 404);
            if (!Puede(def)) return Error("No tiene permiso para ver este reporte.", 403);

            DateTime hoy = HoraHonduras.Hoy;
            var valores = new Dictionary<string, string>();
            foreach (var p in def.Parametros)
            {
                string v = Request.Query[p.Nombre].ToString()?.Trim();
                if (!string.IsNullOrEmpty(v)) valores[p.Nombre] = v;
            }

            var textos = new Dictionary<string, string>();
            string error = await Validar(def, valores, textos, hoy);
            if (error != null) return Error(error, 400);

            var ctx = new ContextoReporte { Valores = valores, Hoy = hoy };
            if (Alcance(def.Clave, out int? empId))
            {
                ctx.EmpIdAlcance = empId;
                ctx.SinSecciones = !empId.HasValue;
            }

            string resumen = Resumen(def, valores, textos);
            string parametros = JsonConvert.SerializeObject(valores);
            DateTime ahora = HoraHonduras.Ahora;

            List<JObject> filas;
            try { filas = await def.Ejecutar(ctx); }
            catch (Exception) { filas = null; }

            if (filas == null)
            {
                const string fallo = "No se pudo generar el reporte. Intente de nuevo en un momento.";
                await Registrar(def, parametros, resumen, "fallo", null, fallo, ahora);
                return Error(fallo, 502);
            }

            foreach (var fila in filas)
                foreach (var prop in fila.Properties().Where(x => x.Value.Type == JTokenType.String))
                    prop.Value = ((string)prop.Value).Trim();

            await Registrar(def, parametros, resumen, "listo", filas.Count, null, ahora);

            return Respuesta(new
            {
                ok = true,
                titulo = def.Titulo,
                resumen,
                generado = ahora.ToString("dd/MM/yyyy h:mm tt", Cultura),
                archivo = $"{def.Clave}-{ahora:yyyyMMdd-HHmm}",
                columnas = def.Columnas.Select(c => new ColumnaReporte
                {
                    Campo = c.Campo, Tipo = c.Tipo, Total = c.Total,
                    Titulo = valores.Aggregate(c.Titulo, (t, kv) => t.Replace("{" + kv.Key + "}", kv.Value))
                }),
                agrupar = def.Agrupar,
                filas
            });
        }

        private async Task Registrar(ReporteDefinicion def, string parametros, string resumen, string estado, int? filas, string error, DateTime fecha)
        {
            int usuId = GetUsuarioId();
            if (usuId <= 0) return;
            await _service.RegistrarAsync(new ReporteGeneradoViewModel
            {
                Usu_Id = usuId, Rge_Clave = def.Clave, Rge_Titulo = def.Titulo, Rge_Parametros = parametros,
                Rge_Resumen = resumen?.Length > 500 ? resumen.Substring(0, 500) : resumen,
                Rge_Estado = estado, Rge_Filas = filas, Rge_Error = error, Rge_Fecha = fecha
            });
        }

        /// <summary>Revisa cada parámetro según su tipo; null si todo está bien. Llena los textos para el resumen.</summary>
        private async Task<string> Validar(ReporteDefinicion def, Dictionary<string, string> v, Dictionary<string, string> textos, DateTime hoy)
        {
            foreach (var p in def.Parametros)
            {
                v.TryGetValue(p.Nombre, out string valor);
                if (valor == null)
                {
                    if (p.Requerido) return $"Falta «{p.Etiqueta}».";
                    continue;
                }
                switch (p.Tipo)
                {
                    case "anio":
                    case "numero":
                        if (!int.TryParse(valor, out int n) || (p.Min.HasValue && n < p.Min) || (p.Max.HasValue && n > p.Max))
                            return $"«{p.Etiqueta}» tiene que ser un número entre {p.Min} y {p.Max}.";
                        break;
                    case "mes":
                        if (!int.TryParse(valor, out int m) || m < 1 || m > 12) return $"«{p.Etiqueta}» no es un mes válido.";
                        textos[p.Nombre] = CatalogoReportes.Meses[m - 1];
                        break;
                    case "fecha":
                        if (!DateTime.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f))
                            return $"«{p.Etiqueta}» no es una fecha válida.";
                        textos[p.Nombre] = f.ToString("dd/MM/yyyy");
                        break;
                    case "opcion":
                        var op = p.Opciones?.FirstOrDefault(o => o.Valor == valor);
                        if (op == null) return $"«{p.Etiqueta}» no es una opción válida.";
                        textos[p.Nombre] = op.Texto;
                        break;
                    case "concepto":
                    case "formaPago":
                    case "anioLectivo":
                    case "nivel":
                    case "grado":
                    case "seccion":
                        if (!int.TryParse(valor, out int id) || id <= 0) return $"«{p.Etiqueta}» no es válido.";
                        break;
                    default:
                        return "El reporte tiene un parámetro desconocido.";
                }
            }

            // Rango de fechas: en orden y de un año como máximo.
            if (v.TryGetValue("desde", out string d) && v.TryGetValue("hasta", out string h))
            {
                DateTime desde = DateTime.ParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime hasta = DateTime.ParseExact(h, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (desde > hasta) return "La fecha «Desde» tiene que ser anterior o igual a la fecha «Hasta».";
                if ((hasta - desde).TotalDays + 1 > MaxDiasRango) return $"Elija un rango de {MaxDiasRango} días como máximo.";
            }

            // Listas: el valor tiene que existir y, en la cascada, pertenecer a su padre.
            if (v.TryGetValue("anioLectivo", out string ani))
            {
                int aniId = int.Parse(ani);
                var anio = (await _anios.ListAsync() ?? Enumerable.Empty<AnioLectivoViewModel>()).FirstOrDefault(a => a.Ani_Id == aniId);
                if (anio == null) return "El año lectivo no existe.";
                textos["anioLectivo"] = string.IsNullOrWhiteSpace(anio.Ani_Nombre) ? anio.Ani_Anio.ToString() : anio.Ani_Nombre;

                int? niv = v.TryGetValue("nivel", out string sn) ? int.Parse(sn) : (int?)null;
                int? cun = v.TryGetValue("grado", out string sc) ? int.Parse(sc) : (int?)null;
                int? sea = v.TryGetValue("seccion", out string ss) ? int.Parse(ss) : (int?)null;
                if (cun.HasValue && !niv.HasValue) return "Elija primero el nivel.";
                if (sea.HasValue && !cun.HasValue) return "Elija primero el grado.";
                if (niv.HasValue)
                {
                    var filtros = await Filtros(aniId, def.Clave);
                    var fn = filtros.FirstOrDefault(f => f.Niv_Id == niv);
                    if (fn == null) return "El nivel no tiene secciones en ese año lectivo.";
                    textos["nivel"] = fn.Nivel;
                    if (cun.HasValue)
                    {
                        var fc = filtros.FirstOrDefault(f => f.Niv_Id == niv && f.Cun_Id == cun);
                        if (fc == null) return "El grado no pertenece al nivel elegido.";
                        textos["grado"] = fc.Grado;
                    }
                    if (sea.HasValue)
                    {
                        var fs = filtros.FirstOrDefault(f => f.Cun_Id == cun && f.Sea_Id == sea);
                        if (fs == null) return "La sección no pertenece al grado elegido.";
                        textos["seccion"] = fs.Seccion;
                    }
                }
            }
            if (v.TryGetValue("concepto", out string cpa))
            {
                var c = (await _conceptos.ListAsync() ?? Enumerable.Empty<ConceptoPagoListViewModel>())
                    .FirstOrDefault(x => x.ConceptoPagoId.ToString() == cpa && !x.EsEliminado);
                if (c == null) return "El concepto de pago no existe.";
                textos["concepto"] = c.Descripcion;
            }
            if (v.TryGetValue("formaPago", out string fpa))
            {
                var f = (await _formas.ListAsync() ?? Enumerable.Empty<FormaPagoListViewModel>())
                    .FirstOrDefault(x => x.FormaPagoId.ToString() == fpa && !x.EsEliminado);
                if (f == null) return "La forma de pago no existe.";
                textos["formaPago"] = f.Descripcion;
            }
            return null;
        }

        /// <summary>"Año lectivo: 2026 · Nivel: Educación básica · Desde: 01/11/2026 …"</summary>
        private static string Resumen(ReporteDefinicion def, Dictionary<string, string> v, Dictionary<string, string> textos)
        {
            var partes = def.Parametros
                .Where(p => v.ContainsKey(p.Nombre))
                .Select(p => $"{p.Etiqueta}: {(textos.TryGetValue(p.Nombre, out string t) ? t : v[p.Nombre])}")
                .ToList();
            return partes.Count == 0 ? "Sin filtros" : string.Join(" · ", partes);
        }

        // ───────────────────────────── Recientes ─────────────────────────────

        /// <summary>Últimos reportes del usuario que todavía puede ver (sus permisos pudieron cambiar).</summary>
        public async Task<ActionResult> RecientesAsync()
        {
            int usuId = GetUsuarioId();
            if (usuId <= 0) return Respuesta(new { data = Array.Empty<object>() });
            var lista = (await _service.RecientesAsync(usuId))
                .Select(r => new { r, def = CatalogoReportes.Buscar(r.Rge_Clave) })
                .Where(x => Puede(x.def))
                .Take(8)
                .Select(x => new
                {
                    clave = x.def.Clave,
                    titulo = x.def.Titulo,
                    area = x.def.Area,
                    resumen = x.r.Rge_Resumen,
                    estado = x.r.Rge_Estado,
                    filas = x.r.Rge_Filas,
                    error = x.r.Rge_Error,
                    fecha = x.r.Rge_Fecha.ToString("dd/MM/yyyy h:mm tt", Cultura),
                    veces = x.r.Veces,
                    parametros = Parametros(x.r.Rge_Parametros)
                });
            return Respuesta(new { data = lista.ToList() });
        }

        private static Dictionary<string, string> Parametros(string json)
        {
            try { return JsonConvert.DeserializeObject<Dictionary<string, string>>(json ?? "{}") ?? new Dictionary<string, string>(); }
            catch (JsonException) { return new Dictionary<string, string>(); }
        }
    }
}
