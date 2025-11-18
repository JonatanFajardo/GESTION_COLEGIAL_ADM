using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class ConceptoPagoFindDto
    {
        public int ConceptoPagoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsRecurrente { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}