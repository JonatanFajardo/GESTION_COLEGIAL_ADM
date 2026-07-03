using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models.Instituto
{
    /// <summary>
    /// Datos del instituto que aparecen en documentos oficiales (constancias,
    /// boletines, recibos). Se persisten como JSON en App_Data/instituto-config.json.
    /// </summary>
    public class InstitutoConfigViewModel
    {
        [Required(ErrorMessage = "El nombre del instituto es obligatorio.")]
        [StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
        [Display(Name = "Nombre del instituto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
    }
}
