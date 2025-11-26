using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class HistoricoPagoViewModel
    {
        public int pagoId { get; set; }
        public DateTime fechaPago { get; set; }
        public decimal montoTotal { get; set; }
        public string formaPago { get; set; } = string.Empty;
        public string numeroReferencia { get; set; } = string.Empty;
        public string numeroRecibo { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public string observaciones { get; set; } = string.Empty;
    }
}
