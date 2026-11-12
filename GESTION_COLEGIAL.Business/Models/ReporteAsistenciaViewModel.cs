using System;

namespace GESTION_COLEGIAL.Business.Models
{
    // Reporte de asistencia de alumnos (API ReporteAsistencia). Los nombres de las propiedades
    // son las columnas que devuelven los procedimientos app.PR_ReporteAsistencia_*.
    // Estados de un día: P presente · T tardanza · A ausente · J justificado.

    /// <summary>Año lectivo que contiene una fecha.</summary>
    public class AsistenciaAnioViewModel
    {
        public int Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public DateTime Ani_FechaInicio { get; set; }
        public DateTime Ani_FechaFin { get; set; }
    }

    /// <summary>Período (parcial, trimestre…) del año lectivo.</summary>
    public class AsistenciaPeriodoViewModel
    {
        public int Pan_Id { get; set; }
        public string Pan_Nombre { get; set; }
        public DateTime Pan_FechaInicio { get; set; }
        public DateTime Pan_FechaFin { get; set; }
    }

    /// <summary>Una sección del año con su grado y nivel (arma los filtros en cascada).</summary>
    public class AsistenciaFiltroViewModel
    {
        public int Sea_Id { get; set; }
        public string Seccion { get; set; }
        public int Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int Cun_Id { get; set; }
        public string Grado { get; set; }
    }

    /// <summary>Resumen de asistencia de una sección en el rango.</summary>
    public class AsistenciaSeccionViewModel
    {
        public int Sea_Id { get; set; }
        public string Seccion { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public int Alumnos { get; set; }
        public int DiasClase { get; set; }
        public int DiasConRegistro { get; set; }
        public int Presentes { get; set; }
        public int Tardanzas { get; set; }
        public int Ausentes { get; set; }
        public int Justificados { get; set; }
        public decimal? PorcAsistencia { get; set; }
        public int AlumnosEnRiesgo { get; set; }
    }

    /// <summary>Encabezado de la planilla de una sección.</summary>
    public class AsistenciaSeccionInfoViewModel
    {
        public int Sea_Id { get; set; }
        public int Ani_Id { get; set; }
        public string Seccion { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public string Guia { get; set; }
    }

    /// <summary>Resumen de asistencia de un alumno en el rango.</summary>
    public class AsistenciaAlumnoViewModel
    {
        public int Alu_Id { get; set; }
        public string NombreAlumno { get; set; }
        public string Identidad { get; set; }
        public int Presentes { get; set; }
        public int Tardanzas { get; set; }
        public int Ausentes { get; set; }
        public int Justificados { get; set; }
        public int DiasRegistrados { get; set; }
        public decimal? PorcAsistencia { get; set; }
        public bool EnRiesgo { get; set; }
        public DateTime? UltimaAusencia { get; set; }
    }

    /// <summary>Un día hábil del rango (columna de la planilla).</summary>
    public class AsistenciaDiaViewModel
    {
        public DateTime Fecha { get; set; }
        public bool EsCierre { get; set; }
        public string Motivo { get; set; }
        public bool ConRegistro { get; set; }
    }

    /// <summary>Una celda de la planilla: el estado del alumno ese día.</summary>
    public class AsistenciaPlanillaViewModel
    {
        public int Alu_Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}
