using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CuentaCobrarListViewModel
    {
        public long? Fila { get; set; }
        public int CuentaCobrarId { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public string Alumno { get; set; } = string.Empty;
        public decimal Pendiente { get; set; }
        public DateTime FechaVence { get; set; }
        public string EstadoPago { get; set; } = string.Empty;

        // Ubicación del alumno y desglose del cargo (para la pantalla de Cuentas por cobrar).
        public int? AlumnoId { get; set; }
        public string Identidad { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Grado { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public decimal MontoOriginal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoTotal { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
    }
}
