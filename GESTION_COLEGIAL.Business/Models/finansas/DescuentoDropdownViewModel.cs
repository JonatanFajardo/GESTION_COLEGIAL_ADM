using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DescuentoDropdownViewModel
    {
        public int DescuentoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}