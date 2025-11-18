using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DescuentoAplicadoDetailViewModel
    {
        public int DescuentoAplicadoId { get; set; }
        public int CuentaCobrarId { get; set; }
        public int DescuentoId { get; set; }
        public decimal MontoAplicado { get; set; }
        public string Justificacion { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}