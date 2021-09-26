using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class AlumnoViewModel
    {
        public PersonaViewModel persona { get; set; }

        public AlumnoViewModel()
        {
            persona = new PersonaViewModel();
        }

        [Key]
        public int Alu_Id { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Per_Id { get; set; }

        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cur_Id { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Est_Id { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string Alu_EsActivo { get; set; }


    }
}