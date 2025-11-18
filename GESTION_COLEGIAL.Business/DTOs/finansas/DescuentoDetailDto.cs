using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class DescuentoDetailDto
    {
        public int DescuentoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public bool EsActivo { get; set; }
        public int CantidadAplicaciones { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}