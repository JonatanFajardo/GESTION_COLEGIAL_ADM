using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class EncargadoViewModel
    {
        public PersonaViewModel persona { get; set; }

        public EncargadoViewModel()
        {
            persona = new PersonaViewModel();
        }

        [Key]
        public int Enc_Id { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Per_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Ocupación")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Enc_Ocupacion { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Enc_EsActivo { get; set; }

    }
}