using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CargoPendienteViewModel
    {
        public int cuentaCobrarId { get; set; }
        public string conceptoPago { get; set; } = string.Empty;
        public decimal montoOriginal { get; set; }
        public decimal montoDescuento { get; set; }
        public decimal montoMora { get; set; }
        public decimal montoTotal { get; set; }
        /// <summary>Saldo que falta pagar del cargo (si la API lo envía; si no, se usa montoTotal).</summary>
        public decimal? montoPendiente { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string estadoPago { get; set; } = string.Empty;
        public DateTime fechaCreacion { get; set; }
        public string observaciones { get; set; } = string.Empty;
    }
}
