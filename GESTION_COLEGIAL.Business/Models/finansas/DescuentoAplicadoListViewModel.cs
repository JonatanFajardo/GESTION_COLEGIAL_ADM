using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DescuentoAplicadoListViewModel
    {
        public int DescuentoAplicadoId { get; set; }
        public int CuentaCobrarId { get; set; }
        public int DescuentoId { get; set; }
        public decimal MontoAplicado { get; set; }
        public string Justificacion { get; set; } = string.Empty;
        public string DescuentoDescripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal MontoTotal { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}