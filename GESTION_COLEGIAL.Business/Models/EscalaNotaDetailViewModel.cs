using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class EscalaNotaDetailViewModel
    {
        public int Esc_Id { get; set; }
        public string Esc_Concepto { get; set; }
        public string Esc_Abreviatura { get; set; }
        public decimal Esc_NotaDesde { get; set; }
        public decimal Esc_NotaHasta { get; set; }
        public bool Esc_Aprueba { get; set; }
        public int Esc_Orden { get; set; }
        public string Esc_RegistradoPor { get; set; }
        public DateTime? Esc_FechaRegistra { get; set; }
        public string Esc_ModificadoPor { get; set; }
        public DateTime? Esc_FechaModifica { get; set; }
    }
}
