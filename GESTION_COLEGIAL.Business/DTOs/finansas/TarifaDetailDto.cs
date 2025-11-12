using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class TarifaDetailDto
    {
        public int TarifaId { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
        public int? NivelId { get; set; }
        public int? CursoNivelId { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}