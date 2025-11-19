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
    }
}