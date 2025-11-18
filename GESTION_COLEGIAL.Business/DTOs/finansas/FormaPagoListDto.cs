using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class FormaPagoListDto
    {
        public int FormaPagoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}