using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class TarifaListViewModel
    {
        public int TarifaId { get; set; }
        public int ConceptoPagoId { get; set; }
        public int? NivelId { get; set; }
        public int? CursoNivelId { get; set; }
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
        public bool EsActivo { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}