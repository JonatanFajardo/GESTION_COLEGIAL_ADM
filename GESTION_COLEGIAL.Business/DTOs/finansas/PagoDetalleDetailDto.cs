using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class PagoDetalleDetailDto
    {
        public int PagoDetalleId { get; set; }
        public int PagoId { get; set; }
        public int CuentaCobrarId { get; set; }
        public decimal MontoAplicado { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}