using System;

namespace GESTION_COLEGIAL.Business.Models
{
    // Centro de reportes (API CentroReportes, script 081).

    /// <summary>Un reporte que generó el usuario; se manda a la API para "Recientes".</summary>
    public class ReporteGeneradoViewModel
    {
        public int Usu_Id { get; set; }
        public string Rge_Clave { get; set; }
        public string Rge_Titulo { get; set; }
        /// <summary>Parámetros normalizados en JSON ({"anio":"2026",...}): con ellos se repite el reporte.</summary>
        public string Rge_Parametros { get; set; }
        public string Rge_Resumen { get; set; }
        /// <summary>listo | fallo</summary>
        public string Rge_Estado { get; set; }
        public int? Rge_Filas { get; set; }
        public string Rge_Error { get; set; }
        public DateTime? Rge_Fecha { get; set; }
    }

    /// <summary>Reporte reciente del usuario (el mismo reporte con los mismos parámetros sale una sola vez).</summary>
    public class ReporteRecienteViewModel
    {
        public int Rge_Id { get; set; }
        public string Rge_Clave { get; set; }
        public string Rge_Titulo { get; set; }
        public string Rge_Parametros { get; set; }
        public string Rge_Resumen { get; set; }
        public string Rge_Estado { get; set; }
        public int? Rge_Filas { get; set; }
        public string Rge_Error { get; set; }
        public DateTime Rge_Fecha { get; set; }
        public int Veces { get; set; }
    }
}
