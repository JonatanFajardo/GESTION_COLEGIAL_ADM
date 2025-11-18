using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class EstadoPagoListViewModel
    {
        public int EstadoPagoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public int CantidadCuentas { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}