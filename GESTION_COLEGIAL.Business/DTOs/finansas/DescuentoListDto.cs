using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class DescuentoListDto
    {
        public int DescuentoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}