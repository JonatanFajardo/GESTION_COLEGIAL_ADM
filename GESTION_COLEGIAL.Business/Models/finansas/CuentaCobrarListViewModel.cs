using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CuentaCobrarListViewModel
    {
        public int CuentaCobrarId { get; set; }
        public int AlumnoId { get; set; }
        public int ConceptoPagoId { get; set; }
        public int? TarifaId { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int EstadoPagoId { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}