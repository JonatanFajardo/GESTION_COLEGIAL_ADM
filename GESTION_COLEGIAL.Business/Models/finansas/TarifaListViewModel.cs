using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class TarifaListViewModel
    {
        public int TarifaId { get; set; }
        public int ConceptoPagoId { get; set; }
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}