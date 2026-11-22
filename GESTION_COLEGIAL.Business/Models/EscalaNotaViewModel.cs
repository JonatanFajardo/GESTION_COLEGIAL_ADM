using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Fila de la escala de notas: un rango de promedio (0-100) y el concepto que le corresponde en el boletín.
    /// </summary>
    public class EscalaNotaViewModel
    {
        public int Esc_Id { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio.")]
        [StringLength(40, ErrorMessage = "El concepto no debe superar los 40 caracteres.")]
        [Display(Name = "Concepto")]
        public string Esc_Concepto { get; set; }

        [StringLength(5, ErrorMessage = "La abreviatura no debe superar los 5 caracteres.")]
        [Display(Name = "Abreviatura")]
        public string Esc_Abreviatura { get; set; }

        [Required(ErrorMessage = "La nota mínima del rango es obligatoria.")]
        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100.")]
        [Display(Name = "Nota desde")]
        public decimal Esc_NotaDesde { get; set; }

        [Required(ErrorMessage = "La nota máxima del rango es obligatoria.")]
        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100.")]
        [Display(Name = "Nota hasta")]
        public decimal Esc_NotaHasta { get; set; }

        [Display(Name = "Aprueba")]
        public bool Esc_Aprueba { get; set; }

        [Display(Name = "Orden")]
        public int Esc_Orden { get; set; }
    }
}
