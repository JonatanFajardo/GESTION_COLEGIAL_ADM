using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class Empleado : BaseViewModel
    {

        [Key]
        public int Emp_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Emp_Codigo { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Per_Id { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Tit_Id { get; set; }

        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Car_Id { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string Emp_EsActivo { get; set; }

    }
}