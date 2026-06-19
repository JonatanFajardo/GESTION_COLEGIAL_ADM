namespace GESTION_COLEGIAL.Business.Models
{
    public class CuentaCobrarPorAlumnoViewModel
    {
        public int CuentaCobrarId { get; set; }
        public string ConceptoDescripcion { get; set; } = string.Empty;
        public decimal MontoOriginal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoPendiente { get; set; }
        public string EstadoPagoDescripcion { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
