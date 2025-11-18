using System;

namespace Gestion.Colegial.Entities.DTOs.finansas
{
    public class MoratoriaDetailDto
    {
        public int MoratoriaId { get; set; }
        public int CuentaCobrarId { get; set; }
        public int DiasAtraso { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal MontoMora { get; set; }
        public DateTime FechaCalculo { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}