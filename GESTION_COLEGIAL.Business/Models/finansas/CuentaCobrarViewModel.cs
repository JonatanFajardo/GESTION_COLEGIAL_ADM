using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// ViewModel para la gestión de cuentas por cobrar.
    /// </summary>
    public class CuentaCobrarViewModel
    {
        [Key]
        public int CuentaCobrarId { get; set; }

        [Display(Name = "Alumno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int AlumnoId { get; set; }

        public string AlumnoNombre { get; set; }

        [Display(Name = "Concepto de Pago")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int ConceptoPagoId { get; set; }

        public string ConceptoPagoDescripcion { get; set; }

        [Display(Name = "Tarifa")]
        public int? TarifaId { get; set; }

        [Display(Name = "Monto Original")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal MontoOriginal { get; set; }

        [Display(Name = "Monto Descuento")]
        public decimal MontoDescuento { get; set; }

        [Display(Name = "Monto Mora")]
        public decimal MontoMora { get; set; }

        [Display(Name = "Monto Total")]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Monto Pendiente")]
        public decimal MontoPendiente { get; set; }

        [Display(Name = "Fecha de Emisión")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime FechaEmision { get; set; }

        [Display(Name = "Fecha de Vencimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime FechaVencimiento { get; set; }

        [Display(Name = "Estado de Pago")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int EstadoPagoId { get; set; }

        public string EstadoPagoDescripcion { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string Observaciones { get; set; }

        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
