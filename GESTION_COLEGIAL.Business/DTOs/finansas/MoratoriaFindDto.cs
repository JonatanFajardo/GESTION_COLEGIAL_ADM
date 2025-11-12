using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class MoratoriaFindDto
    {
        public int MoratoriaId { get; set; }
        public int CuentaCobrarId { get; set; }
        public int DiasAtraso { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal MontoMora { get; set; }
        public DateTime FechaCalculo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}