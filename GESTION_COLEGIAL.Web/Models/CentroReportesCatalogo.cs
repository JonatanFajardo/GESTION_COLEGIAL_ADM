using GESTION_COLEGIAL.Business.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Models
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Centro de reportes: catálogo declarativo (como el ReportRegistry de Skuul).
    // Cada reporte dice: área, qué pantallas de origen permiten verlo (basta una),
    // sus parámetros, sus columnas y cómo pide las filas a la API.
    // Los permisos se validan en CentroReportesController (servidor), no solo en la vista.
    // ─────────────────────────────────────────────────────────────────────────────

    public class OpcionReporte
    {
        public string Valor { get; set; }
        public string Texto { get; set; }
    }

    /// <summary>
    /// Tipos: anio · mes · fecha · numero · opcion · anioLectivo · nivel · grado · seccion · concepto · formaPago.
    /// nivel → grado → sección van en cascada y dependen del año lectivo.
    /// Defecto: un valor fijo o una palabra que se resuelve al abrir la pantalla
    /// (hoy, inicioMes, hace30, anioActual, anioAnterior, mesActual, anioLectivoActual).
    /// </summary>
    public class ParametroReporte
    {
        public string Nombre { get; set; }
        public string Etiqueta { get; set; }
        public string Tipo { get; set; }
        public bool Requerido { get; set; }
        public string Defecto { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public List<OpcionReporte> Opciones { get; set; }
        public string Ayuda { get; set; }
    }

    /// <summary>Tipo: texto · entero · dinero · porcentaje · fecha · fechahora · sino. Total = suma al pie.</summary>
    public class ColumnaReporte
    {
        public string Campo { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; } = "texto";
        public bool Total { get; set; }
    }

    public class ReporteDefinicion
    {
        public string Clave { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        /// <summary>Pantalla del sistema de donde viene el reporte (se muestra como referencia).</summary>
        public string Origen { get; set; }
        /// <summary>Nombres de seguridad.tbPantallas que dan acceso (basta una). Admin pasa siempre.</summary>
        [JsonIgnore] public string[] Pantallas { get; set; } = Array.Empty<string>();
        public List<ParametroReporte> Parametros { get; set; } = new List<ParametroReporte>();
        public List<ColumnaReporte> Columnas { get; set; } = new List<ColumnaReporte>();
        /// <summary>Opciones de "Agrupar por" de la tabla (campos).</summary>
        public List<string> Agrupar { get; set; } = new List<string>();
        [JsonIgnore] public Func<ContextoReporte, Task<List<JObject>>> Ejecutar { get; set; }
    }

    /// <summary>Valores ya validados de una ejecución.</summary>
    public class ContextoReporte
    {
        public Dictionary<string, string> Valores { get; set; } = new Dictionary<string, string>();
        public DateTime Hoy { get; set; }
        /// <summary>Solo para asistencia: sin permiso de ver todas las secciones, el empleado de la sesión.</summary>
        public int? EmpIdAlcance { get; set; }
        public bool SinSecciones { get; set; }

        public string Texto(string nombre) => Valores.TryGetValue(nombre, out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;
        public int? Entero(string nombre) => int.TryParse(Texto(nombre), out int v) ? v : (int?)null;
        public string Iso(string nombre) => Texto(nombre);

        /// <summary>"&amp;nombre=valor" si hay valor.</summary>
        public string Qs(string nombreApi, string nombre) => Texto(nombre) is string v ? $"&{nombreApi}={Uri.EscapeDataString(v)}" : "";
    }

    public static class CatalogoReportes
    {
        public const string Academico = "Académico";
        public const string Finanzas = "Finanzas";
        public const string Personal = "Recursos humanos";
        public static readonly string[] Areas = { Academico, Finanzas, Personal };

        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        private static readonly CentroReportesService Api = new CentroReportesService();

        // ── Fábricas de parámetros ──
        private static ParametroReporte P(string nombre, string etiqueta, string tipo, bool requerido = false, string defecto = null) =>
            new ParametroReporte { Nombre = nombre, Etiqueta = etiqueta, Tipo = tipo, Requerido = requerido, Defecto = defecto };

        private static ParametroReporte Anio(string nombre = "anio", string etiqueta = "Año", string defecto = "anioActual") =>
            new ParametroReporte { Nombre = nombre, Etiqueta = etiqueta, Tipo = "anio", Requerido = true, Defecto = defecto, Min = 2000, Max = 2100 };

        private static ParametroReporte Desde(string defecto = "inicioMes") => P("desde", "Desde", "fecha", true, defecto);
        private static ParametroReporte Hasta() => P("hasta", "Hasta", "fecha", true, "hoy");

        private static ParametroReporte Opcion(string nombre, string etiqueta, string defecto, params (string Valor, string Texto)[] opciones) =>
            new ParametroReporte
            {
                Nombre = nombre, Etiqueta = etiqueta, Tipo = "opcion", Requerido = true, Defecto = defecto,
                Opciones = opciones.Select(o => new OpcionReporte { Valor = o.Valor, Texto = o.Texto }).ToList()
            };

        /// <summary>Año lectivo → Nivel → Grado → Sección (solo el año es obligatorio).</summary>
        private static IEnumerable<ParametroReporte> Ubicacion() => new[]
        {
            P("anioLectivo", "Año lectivo", "anioLectivo", true, "anioLectivoActual"),
            P("nivel", "Nivel", "nivel"),
            P("grado", "Grado", "grado"),
            P("seccion", "Sección", "seccion")
        };

        private static ColumnaReporte C(string campo, string titulo, string tipo = "texto", bool total = false) =>
            new ColumnaReporte { Campo = campo, Titulo = titulo, Tipo = tipo, Total = total };

        private static List<JObject> Filtrar(List<JObject> filas, Func<JObject, bool> condicion) =>
            filas?.Where(condicion).ToList();

        private static string Hoy(ContextoReporte c) => c.Hoy.ToString("yyyy-MM-dd");

        private static string Ubic(ContextoReporte c) =>
            $"aniId={c.Entero("anioLectivo")}" + c.Qs("nivId", "nivel") + c.Qs("cunId", "grado") + c.Qs("seaId", "seccion");

        private static DateTime? Fecha(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null) return null;
            if (t.Type == JTokenType.Date) return t.Value<DateTime>();
            return DateTime.TryParse(t.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : (DateTime?)null;
        }

        public static readonly List<ReporteDefinicion> Todos = new List<ReporteDefinicion>
        {
            // ───────────────────────────── Académico ─────────────────────────────
            new ReporteDefinicion
            {
                Clave = "lista-clase", Area = Academico, Icono = "bi bi-people",
                Titulo = "Lista de clase por sección",
                Descripcion = "Alumnos matriculados en cada sección, en orden alfabético, con identidad, edad y teléfono.",
                Origen = "Alumnos", Pantallas = new[] { "Listado de alumnos" },
                Parametros = Ubicacion().Append(Opcion("matricula", "Matrícula", "activas",
                    ("activas", "Solo activas"), ("todas", "Todas (incluye retirados y promovidos)"))).ToList(),
                Columnas = { C("Nivel", "Nivel"), C("Grado", "Grado"), C("Seccion", "Sección"), C("Numero", "N.º", "entero"),
                             C("Alumno", "Alumno"), C("Identidad", "Identidad"), C("Sexo", "Sexo"), C("FechaNacimiento", "Nacimiento", "fecha"),
                             C("Edad", "Edad", "entero"), C("Telefono", "Teléfono"), C("EstadoMatricula", "Matrícula") },
                Agrupar = { "Seccion" },
                Ejecutar = c => Api.FilasAsync($"CentroReportes/ListaClaseAsync?{Ubic(c)}&soloActivos={(c.Texto("matricula") == "activas" ? "true" : "false")}&hoy={Hoy(c)}")
            },
            new ReporteDefinicion
            {
                Clave = "matricula-programa", Area = Academico, Icono = "bi bi-diagram-3",
                Titulo = "Matrícula por programa y sexo",
                Descripcion = "Cuántos alumnos hay en cada programa (ciclo o BTP) del año, separados por sexo.",
                Origen = "Pirámide de matrícula", Pantallas = new[] { "Pirámide de Matrícula" },
                Parametros = { Anio() },
                Columnas = { C("Niv_Descripcion", "Nivel"), C("Cur_Nombre", "Programa"), C("TotalMasculino", "Hombres", "entero", true),
                             C("TotalFemenino", "Mujeres", "entero", true), C("TotalOtro", "Otro", "entero", true), C("TotalGeneral", "Total", "entero", true) },
                Ejecutar = c => Api.FilasAsync($"PiramideMatricula/ListAsync?Anio={c.Entero("anio")}")
            },
            new ReporteDefinicion
            {
                Clave = "comparativo-matricula", Area = Academico, Icono = "bi bi-arrow-left-right",
                Titulo = "Matrícula: comparativo entre dos años",
                Descripcion = "Alumnos por programa en dos años y cuánto subió o bajó cada uno.",
                Origen = "Comparativo por año", Pantallas = new[] { "Comparativo de años lectivos", "Comparativo de Años" },
                Parametros = { Anio("anioBase", "Año base", "anioAnterior"), Anio("anioComparacion", "Año a comparar", "anioActual") },
                Columnas = { C("Cur_Descripcion", "Programa"), C("TotalAnioBase", "Alumnos {anioBase}", "entero", true),
                             C("TotalAnioComparacion", "Alumnos {anioComparacion}", "entero", true), C("Diferencia", "Diferencia", "entero", true),
                             C("PorcentajeCambio", "% de cambio", "porcentaje") },
                Ejecutar = c => Api.FilasAsync($"ComparativoAnios/ListAsync?AnioBase={c.Entero("anioBase")}&AnioComparacion={c.Entero("anioComparacion")}")
            },
            new ReporteDefinicion
            {
                Clave = "asistencia-secciones", Area = Academico, Icono = "bi bi-calendar-check",
                Titulo = "Asistencia de alumnos por sección",
                Descripcion = "Presentes, tardanzas, ausencias y % de asistencia de cada sección en un rango de fechas (según el pase de lista).",
                Origen = "Reporte de asistencia", Pantallas = new[] { "Reporte de asistencia de alumnos" },
                Parametros = Ubicacion().Concat(new[] { Desde("hace30"), Hasta() }).ToList(),
                Columnas = { C("Nivel", "Nivel"), C("Grado", "Grado"), C("Seccion", "Sección"), C("Alumnos", "Alumnos", "entero", true),
                             C("DiasClase", "Días de clase", "entero"), C("Presentes", "Presentes", "entero", true), C("Tardanzas", "Tardanzas", "entero", true),
                             C("Ausentes", "Ausentes", "entero", true), C("Justificados", "Justificados", "entero", true),
                             C("PorcAsistencia", "% asistencia", "porcentaje"), C("AlumnosEnRiesgo", "Alumnos en riesgo", "entero", true) },
                Ejecutar = c => c.SinSecciones ? Task.FromResult(new List<JObject>())
                    : Api.FilasAsync($"ReporteAsistencia/SeccionesAsync?{Ubic(c)}&desde={c.Iso("desde")}&hasta={c.Iso("hasta")}"
                                     + (c.EmpIdAlcance.HasValue ? $"&empId={c.EmpIdAlcance.Value}" : ""))
            },

            // ───────────────────────────── Finanzas ─────────────────────────────
            new ReporteDefinicion
            {
                Clave = "saldos-alumno", Area = Finanzas, Icono = "bi bi-wallet2",
                Titulo = "Saldos por alumno",
                Descripcion = "Lo cargado, pagado, pendiente y vencido de cada alumno del año, por nivel, grado o sección.",
                Origen = "Cuentas por cobrar", Pantallas = new[] { "Listado de cuentas por cobrar" },
                Parametros = Ubicacion().Append(Opcion("mostrar", "Mostrar", "conSaldo",
                    ("conSaldo", "Solo alumnos con saldo"), ("todos", "Todos los alumnos"))).ToList(),
                Columnas = { C("Nivel", "Nivel"), C("Grado", "Grado"), C("Seccion", "Sección"), C("Alumno", "Alumno"), C("Identidad", "Identidad"),
                             C("Telefono", "Teléfono"), C("Cargado", "Cargado", "dinero", true), C("Pagado", "Pagado", "dinero", true),
                             C("Saldo", "Saldo", "dinero", true), C("Vencido", "Vencido", "dinero", true),
                             C("CargosVencidos", "Cargos vencidos", "entero", true), C("UltimoPago", "Último pago", "fecha") },
                Agrupar = { "Seccion" },
                Ejecutar = c => Api.FilasAsync($"CentroReportes/SaldosAlumnoAsync?{Ubic(c)}&soloConSaldo={(c.Texto("mostrar") == "conSaldo" ? "true" : "false")}&hoy={Hoy(c)}")
            },
            new ReporteDefinicion
            {
                Clave = "ingresos-concepto", Area = Finanzas, Icono = "bi bi-cash-coin",
                Titulo = "Ingresos por concepto",
                Descripcion = "Dinero cobrado en un rango de fechas por concepto y forma de pago (sin pagos anulados).",
                Origen = "Ingresos del mes", Pantallas = new[] { "Reportes financieros" },
                Parametros = { Desde(), Hasta(), P("concepto", "Concepto", "concepto"), P("formaPago", "Forma de pago", "formaPago") },
                // "Pagos" no se suma: un pago con varios conceptos cuenta en cada uno.
                Columnas = { C("Concepto", "Concepto"), C("FormaPago", "Forma de pago"), C("Pagos", "Pagos", "entero"),
                             C("Alumnos", "Alumnos", "entero"), C("Monto", "Monto", "dinero", true) },
                Agrupar = { "Concepto", "FormaPago" },
                Ejecutar = c => Api.FilasAsync($"CentroReportes/IngresosConceptoAsync?desde={c.Iso("desde")}&hasta={c.Iso("hasta")}"
                                               + c.Qs("cpaId", "concepto") + c.Qs("fpaId", "formaPago"))
            },
            new ReporteDefinicion
            {
                Clave = "historial-pagos", Area = Finanzas, Icono = "bi bi-receipt",
                Titulo = "Pagos recibidos",
                Descripcion = "Cada pago registrado en un rango de fechas: alumno, forma de pago, monto y quién lo registró.",
                Origen = "Historial de transacciones", Pantallas = new[] { "Historial de transacciones" },
                Parametros = { Desde(), Hasta(), P("formaPago", "Forma de pago", "formaPago") },
                Columnas = { C("Pag_FechaPago", "Fecha", "fechahora"), C("Pag_Id", "N.º de pago", "entero"), C("Pag_NumeroReferencia", "Referencia"),
                             C("Alu_NombreCompleto", "Alumno"), C("Alu_Identidad", "Identidad"), C("Cur_Nombre", "Programa"),
                             C("Fpa_Descripcion", "Forma de pago"), C("CantidadConceptos", "Conceptos", "entero"),
                             C("Pag_MontoTotal", "Monto", "dinero", true), C("CajeroNombre", "Registró") },
                Agrupar = { "Fpa_Descripcion" },
                Ejecutar = async c =>
                {
                    var filas = await Api.FilasAsync($"HistorialTransacciones/ListAsync?fechaDesde={c.Iso("desde")}&fechaHasta={c.Iso("hasta")}");
                    int? fpa = c.Entero("formaPago");
                    return fpa.HasValue ? Filtrar(filas, f => (int?)f["Fpa_Id"] == fpa) : filas;
                }
            },
            new ReporteDefinicion
            {
                Clave = "cargos-pendientes", Area = Finanzas, Icono = "bi bi-hourglass-split",
                Titulo = "Cargos pendientes de cobro",
                Descripcion = "Cada cargo con saldo: alumno, concepto, vencimiento, días de atraso y monto pendiente.",
                Origen = "Pendientes por cobrar", Pantallas = new[] { "Pendientes por cobrar hoy", "Pendientes por Cobrar" },
                Parametros = { P("concepto", "Concepto", "concepto"),
                               Opcion("cargos", "Cargos", "todos", ("todos", "Todos los pendientes"), ("vencidos", "Solo vencidos")) },
                Columnas = { C("Alu_NombreCompleto", "Alumno"), C("Alu_Identidad", "Identidad"), C("Alu_Telefono", "Teléfono"),
                             C("Cur_Nombre", "Programa"), C("Sec_Descripcion", "Sección"), C("Cpa_Descripcion", "Concepto"), C("Periodo", "Período"),
                             C("Cco_FechaVencimiento", "Vence", "fecha"), C("DiasVencida", "Días de atraso", "entero"),
                             C("Cco_MontoTotal", "Monto", "dinero", true), C("Cco_MontoPendiente", "Pendiente", "dinero", true), C("Epa_Descripcion", "Estado") },
                Agrupar = { "Cpa_Descripcion", "Cur_Nombre" },
                Ejecutar = async c =>
                {
                    var filas = await Api.FilasAsync("PendientesCobrar/ListAsync");
                    int? cpa = c.Entero("concepto");
                    bool soloVencidos = c.Texto("cargos") == "vencidos";
                    filas = Filtrar(filas, f => (!cpa.HasValue || (int?)f["Cpa_Id"] == cpa) && (!soloVencidos || (bool?)f["EsVencida"] == true));
                    filas?.ForEach(f => f["Periodo"] = $"{(int?)f["Cco_Mes"]:00}/{(int?)f["Cco_Anio"]}");
                    return filas;
                }
            },
            new ReporteDefinicion
            {
                Clave = "antiguedad-saldos", Area = Finanzas, Icono = "bi bi-clock-history",
                Titulo = "Antigüedad de saldos",
                Descripcion = "La deuda de cada alumno repartida según cuántos días lleva vencida (1-30, 31-60, 61-90, más de 90).",
                Origen = "Antigüedad de saldos", Pantallas = new[] { "Aging de Cuentas por Cobrar" },
                Columnas = { C("NombreAlumno", "Alumno"), C("Rango_1_30", "1 a 30 días", "dinero", true), C("Rango_31_60", "31 a 60 días", "dinero", true),
                             C("Rango_61_90", "61 a 90 días", "dinero", true), C("Rango_Mas90", "Más de 90 días", "dinero", true), C("TotalDeuda", "Total", "dinero", true) },
                Ejecutar = c => Api.FilasAsync("AgingCuentas/ListAsync")
            },
            new ReporteDefinicion
            {
                Clave = "mayores-deudores", Area = Finanzas, Icono = "bi bi-sort-down",
                Titulo = "Mayores deudores",
                Descripcion = "Los alumnos que más deben, con teléfono para llamar y su último vencimiento.",
                Origen = "Mayores deudores", Pantallas = new[] { "Top Alumnos con Deuda" },
                Parametros = { new ParametroReporte { Nombre = "cantidad", Etiqueta = "Cuántos alumnos", Tipo = "numero", Requerido = true, Defecto = "20", Min = 5, Max = 200 } },
                Columnas = { C("NombreAlumno", "Alumno"), C("Per_Telefono", "Teléfono"), C("Cur_Nombre", "Programa"),
                             C("TotalDeuda", "Deuda", "dinero", true), C("CuotasPendientes", "Cargos pendientes", "entero", true), C("UltimoVencimiento", "Último vencimiento", "fecha") },
                Ejecutar = c => Api.FilasAsync($"TopDeudores/ListAsync?Top={c.Entero("cantidad")}")
            },
            new ReporteDefinicion
            {
                Clave = "morosidad-programa", Area = Finanzas, Icono = "bi bi-exclamation-triangle",
                Titulo = "Morosidad por programa",
                Descripcion = "En cada programa: alumnos, cuántos tienen deuda vencida, monto vencido y % de morosidad.",
                Origen = "Morosidad por nivel", Pantallas = new[] { "Morosidad por Nivel" },
                Columnas = { C("Cur_Nombre", "Programa"), C("TotalAlumnos", "Alumnos", "entero", true), C("AlumnosMorosos", "Con deuda vencida", "entero", true),
                             C("MontoMoroso", "Monto vencido", "dinero", true), C("PorcentajeMorosidad", "% morosidad", "porcentaje") },
                Ejecutar = c => Api.FilasAsync("MorosidadPorNivel/ListAsync")
            },
            new ReporteDefinicion
            {
                Clave = "flujo-caja", Area = Finanzas, Icono = "bi bi-graph-up",
                Titulo = "Flujo de caja por mes",
                Descripcion = "Mes a mes: lo cobrado, lo que quedó pendiente de los cargos del mes y la diferencia.",
                Origen = "Flujo de caja", Pantallas = new[] { "Flujo de caja proyectado", "Flujo de Caja" },
                Parametros = { Anio() },
                Columnas = { C("NombreMes", "Mes"), C("TotalIngresos", "Cobrado", "dinero", true), C("TotalPendiente", "Pendiente", "dinero", true),
                             C("SaldoNeto", "Diferencia", "dinero", true) },
                Ejecutar = c => Api.FilasAsync($"FlujoCaja/ListAsync?Anio={c.Entero("anio")}")
            },
            new ReporteDefinicion
            {
                Clave = "estado-resultados", Area = Finanzas, Icono = "bi bi-clipboard-data",
                Titulo = "Estado de resultados del mes",
                Descripcion = "Total cobrado, descuentos, ingresos netos, cuentas pendientes y morosos de un mes.",
                Origen = "Estado de resultados", Pantallas = new[] { "Estado de resultados mensual", "Estado de Resultados" },
                Parametros = { Anio(), P("mes", "Mes", "mes", true, "mesActual") },
                Columnas = { C("Concepto", "Concepto"), C("Monto", "Monto", "dinero") },
                Ejecutar = c => Api.FilasAsync($"EstadoResultados/FindAsync?Anio={c.Entero("anio")}&Mes={c.Entero("mes")}")
            },
            new ReporteDefinicion
            {
                Clave = "cobrado-concepto", Area = Finanzas, Icono = "bi bi-pie-chart",
                Titulo = "Cobrado y pendiente por concepto",
                Descripcion = "En el año, cuánto se cobró y cuánto falta de cada concepto de pago, con su % cobrado.",
                Origen = "Análisis de ingresos", Pantallas = new[] { "Analisis ingresos vs egresos", "Análisis Ingresos vs Egresos" },
                Parametros = { Anio() },
                Columnas = { C("ConceptoPago", "Concepto"), C("TotalCobrado", "Cobrado", "dinero", true), C("TotalPendiente", "Pendiente", "dinero", true),
                             C("PorcentajeCobrado", "% cobrado", "porcentaje") },
                Ejecutar = c => Api.FilasAsync($"AnalisisIngresos/ListAsync?Anio={c.Entero("anio")}")
            },

            // ───────────────────────────── Recursos humanos ─────────────────────────────
            new ReporteDefinicion
            {
                Clave = "antiguedad-personal", Area = Personal, Icono = "bi bi-award",
                Titulo = "Antigüedad del personal",
                Descripcion = "Empleados activos con su fecha de registro y los años y meses que llevan.",
                Origen = "Antigüedad", Pantallas = new[] { "Reporte de antigüedad" },
                Columnas = { C("Emp_Codigo", "Código"), C("NombreCompleto", "Empleado"), C("FechaIngreso", "Fecha de registro", "fecha"),
                             C("Antiguedad_Anios", "Años", "entero"), C("Antiguedad_Meses", "Meses", "entero") },
                Ejecutar = async c =>
                {
                    var filas = await Api.FilasAsync("EmpleadosAntiguedad/ListAsync");
                    // Años y meses se calculan aquí: la API devuelve los años en 0 (no lee la columna "Años").
                    filas?.ForEach(f =>
                    {
                        var ingreso = Fecha(f["FechaIngreso"]);
                        if (!ingreso.HasValue) return;
                        int meses = (c.Hoy.Year - ingreso.Value.Year) * 12 + c.Hoy.Month - ingreso.Value.Month - (c.Hoy.Day < ingreso.Value.Day ? 1 : 0);
                        if (meses < 0) meses = 0;
                        f["Antiguedad_Anios"] = meses / 12;
                        f["Antiguedad_Meses"] = meses % 12;
                    });
                    return filas;
                }
            },
            new ReporteDefinicion
            {
                Clave = "asistencia-personal", Area = Personal, Icono = "bi bi-person-check",
                Titulo = "Asistencia del personal",
                Descripcion = "Por empleado en un rango: días presentes, tardanzas, ausencias, permisos y % de asistencia (los más bajos primero).",
                Origen = "Asistencia del personal", Pantallas = new[] { "Asistencia Empleados" },
                Parametros = { Desde(), Hasta() },
                Columnas = { C("Codigo", "Código"), C("Empleado", "Empleado"), C("Cargo", "Cargo"), C("Registros", "Días registrados", "entero"),
                             C("Presente", "Presente", "entero", true), C("Tardanza", "Tardanza", "entero", true), C("Ausente", "Ausente", "entero", true),
                             C("Permiso", "Permiso", "entero", true), C("PorcAsistencia", "% asistencia", "porcentaje"), C("UltimaAusencia", "Última ausencia", "fecha") },
                Agrupar = { "Cargo" },
                Ejecutar = c => Api.FilasAsync($"CentroReportes/AsistenciaPersonalAsync?desde={c.Iso("desde")}&hasta={c.Iso("hasta")}")
            },
            new ReporteDefinicion
            {
                Clave = "documentos-personal", Area = Personal, Icono = "bi bi-folder-check",
                Titulo = "Documentos pendientes del personal",
                Descripcion = "Qué documento le falta entregar a cada empleado (y cuáles están vencidos o por vencer).",
                Origen = "Cumplimiento documental", Pantallas = new[] { "Cumplimiento de Documentos" },
                Parametros = { Opcion("mostrar", "Mostrar", "faltantes", ("faltantes", "Documentos sin entregar"), ("obligatorios", "Obligatorios sin entregar"),
                                      ("vencidos", "Vencidos o por vencer"), ("todos", "Todos los documentos")) },
                Columnas = { C("Emp_Codigo", "Código"), C("NombreCompleto", "Empleado"), C("Per_Identidad", "Identidad"), C("Documento", "Documento"),
                             C("EsObligatorio", "Obligatorio", "sino"), C("EsEntregado", "Entregado", "sino"), C("FechaEntrega", "Entregado el", "fecha"),
                             C("Doc_FechaVencimiento", "Vence", "fecha"), C("Situacion", "Situación") },
                Agrupar = { "NombreCompleto", "Documento" },
                Ejecutar = async c =>
                {
                    var filas = await Api.FilasAsync("CumplimientoDocumentos/ListAsync");
                    string mostrar = c.Texto("mostrar");
                    filas = Filtrar(filas, f =>
                    {
                        bool entregado = (bool?)f["EsEntregado"] == true, obligatorio = (bool?)f["EsObligatorio"] == true;
                        string estado = (string)f["EstadoVencimiento"];
                        return mostrar switch
                        {
                            "faltantes" => !entregado,
                            "obligatorios" => !entregado && obligatorio,
                            "vencidos" => estado == "vencido" || estado == "por-vencer",
                            _ => true
                        };
                    });
                    filas?.ForEach(f => f["Situacion"] = (string)f["EstadoVencimiento"] switch
                    {
                        "pendiente" => "Sin entregar",
                        "vencido" => "Vencido",
                        "por-vencer" => "Por vencer",
                        "sin-vencimiento" => "Entregado",
                        "vigente" => "Vigente",
                        string otro => otro,
                        _ => ""
                    });
                    return filas;
                }
            }
        };

        public static ReporteDefinicion Buscar(string clave) =>
            Todos.FirstOrDefault(r => string.Equals(r.Clave, clave, StringComparison.OrdinalIgnoreCase));

        public static readonly string[] Meses =
            { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
    }
}
