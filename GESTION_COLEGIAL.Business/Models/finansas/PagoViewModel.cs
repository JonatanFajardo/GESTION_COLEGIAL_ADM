using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista para la creación y edición de pagos.
    /// </summary>
    public class PagoViewModel
    {
        public int PagoId { get; set; }
        public int AlumnoId { get; set; }
        public int? EncargadoId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string NumeroReferencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        /// <summary>Cargos que se están pagando: quedan pagados y el excedente pasa a saldo a favor.</summary>
        public System.Collections.Generic.List<int> CuentasCobrarIds { get; set; } = new System.Collections.Generic.List<int>();
    }
}