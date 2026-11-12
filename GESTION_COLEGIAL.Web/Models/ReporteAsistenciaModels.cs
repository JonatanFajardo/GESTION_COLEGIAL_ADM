using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Formatos y reglas comunes del reporte de asistencia.</summary>
    public static class FormatoAsistencia
    {
        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        /// <summary>Rango máximo que se puede consultar, en días.</summary>
        public const int MaxDias = 366;

        /// <summary>Por debajo de este % el alumno o la sección está en riesgo.</summary>
        public const decimal UmbralRiesgo = 90m;

        public static string Dia(DateTime d) => d.ToString("d MMM yyyy", Cultura).Replace(".", "");
        public static string DiaCorto(DateTime d) => d.ToString("d MMM", Cultura).Replace(".", "");
        public static string DiaSemana(DateTime d) => d.ToString("ddd", Cultura).Replace(".", "");
        public static string Iso(DateTime d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public static string Porcentaje(decimal? p) =>
            p.HasValue ? p.Value.ToString("0.0", Cultura) + " %" : "Sin registros";

        /// <summary>Ancho de la barrita (0-100) con punto decimal, para CSS.</summary>
        public static string Ancho(decimal? p) =>
            Math.Max(0, Math.Min(100, p ?? 0)).ToString("0.#", CultureInfo.InvariantCulture);

        /// <summary>Tono de la barrita: bien (≥95), atención (≥90) o riesgo.</summary>
        public static string Tono(decimal? p) =>
            !p.HasValue ? "vacio" : p.Value >= 95 ? "bien" : p.Value >= UmbralRiesgo ? "atencion" : "riesgo";

        public static string Estado(string e) => e switch
        {
            "P" => "Presente",
            "T" => "Tardanza",
            "A" => "Ausente",
            "J" => "Justificado",
            _ => ""
        };
    }

    /// <summary>Atajo de fechas del formulario (períodos del año, "Este mes"…).</summary>
    public class AtajoAsistencia
    {
        public string Nombre { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public bool Activo { get; set; }
    }

    /// <summary>Pantalla "Asistencia de alumnos": filtros, KPI y tabla por sección.</summary>
    public class ReporteAsistenciaIndexViewModel
    {
        public DateTime Hoy { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public int? NivId { get; set; }
        public int? CunId { get; set; }
        public int? SeaId { get; set; }

        /// <summary>Mensaje si las fechas no sirven (no se consulta nada).</summary>
        public string Error { get; set; }
        public bool VerTodas { get; set; }

        public AsistenciaAnioViewModel Anio { get; set; }
        public List<AtajoAsistencia> Atajos { get; set; } = new List<AtajoAsistencia>();
        public List<AsistenciaFiltroViewModel> Filtros { get; set; } = new List<AsistenciaFiltroViewModel>();
        public List<AsistenciaSeccionViewModel> Secciones { get; set; } = new List<AsistenciaSeccionViewModel>();

        public string DesdeIso => FormatoAsistencia.Iso(Desde);
        public string HastaIso => FormatoAsistencia.Iso(Hasta);
        public string HoyIso => FormatoAsistencia.Iso(Hoy);
        public bool HayFiltros => NivId.HasValue || CunId.HasValue || SeaId.HasValue;

        public IEnumerable<AsistenciaFiltroViewModel> Niveles =>
            Filtros.GroupBy(f => f.Niv_Id).Select(g => g.First()).OrderBy(f => f.Niv_Id);

        public IEnumerable<AsistenciaFiltroViewModel> Grados =>
            !NivId.HasValue ? Enumerable.Empty<AsistenciaFiltroViewModel>()
            : Filtros.Where(f => f.Niv_Id == NivId).GroupBy(f => f.Cun_Id).Select(g => g.First()).OrderBy(f => f.Cun_Id);

        public IEnumerable<AsistenciaFiltroViewModel> SeccionesFiltro =>
            !CunId.HasValue ? Enumerable.Empty<AsistenciaFiltroViewModel>()
            : Filtros.Where(f => f.Niv_Id == NivId && f.Cun_Id == CunId).OrderBy(f => f.Seccion);

        // KPI: suma de todas las secciones del reporte.
        public int Presentes => Secciones.Sum(s => s.Presentes);
        public int Tardanzas => Secciones.Sum(s => s.Tardanzas);
        public int Ausentes => Secciones.Sum(s => s.Ausentes);
        public int Justificados => Secciones.Sum(s => s.Justificados);
        public int EnRiesgo => Secciones.Sum(s => s.AlumnosEnRiesgo);
        public int Registros => Presentes + Tardanzas + Ausentes + Justificados;

        public decimal? PorcGeneral =>
            Registros == 0 ? (decimal?)null : Math.Round((Presentes + Tardanzas) * 100m / Registros, 1);
    }

    /// <summary>Pantalla de una sección: resumen por alumno y planilla día por día.</summary>
    public class ReporteAsistenciaSeccionViewModel
    {
        public AsistenciaSeccionInfoViewModel Info { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }

        // Filtros del reporte general, para volver a él tal como estaba.
        public int? NivId { get; set; }
        public int? CunId { get; set; }
        public int? SeaIdFiltro { get; set; }

        public List<AsistenciaAlumnoViewModel> Alumnos { get; set; } = new List<AsistenciaAlumnoViewModel>();
        public List<AsistenciaDiaViewModel> Dias { get; set; } = new List<AsistenciaDiaViewModel>();

        /// <summary>Estado por alumno y fecha ("P", "T", "A", "J").</summary>
        public Dictionary<(int, DateTime), string> Planilla { get; set; } = new Dictionary<(int, DateTime), string>();

        public string DesdeIso => FormatoAsistencia.Iso(Desde);
        public string HastaIso => FormatoAsistencia.Iso(Hasta);
        public int EnRiesgo => Alumnos.Count(a => a.EnRiesgo);
        public bool HayRegistros => Alumnos.Any(a => a.DiasRegistrados > 0);

        public string EstadoDe(int aluId, DateTime fecha) =>
            Planilla.TryGetValue((aluId, fecha.Date), out string e) ? e : null;
    }
}
