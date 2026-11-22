using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Umbrales de promedio para el cuadro de honor (una sola fila).
    /// </summary>
    public class CuadroHonorConfigViewModel
    {
        public int Chc_Id { get; set; }

        [Required(ErrorMessage = "El umbral del cuadro de honor es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El umbral debe estar entre 0 y 100.")]
        [Display(Name = "Umbral Cuadro de Honor")]
        public decimal Chc_UmbralHonor { get; set; }

        [Required(ErrorMessage = "El umbral del cuadro de honor superior es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El umbral debe estar entre 0 y 100.")]
        [Display(Name = "Umbral Cuadro de Honor Superior")]
        public decimal Chc_UmbralHonorSuperior { get; set; }
    }
}
