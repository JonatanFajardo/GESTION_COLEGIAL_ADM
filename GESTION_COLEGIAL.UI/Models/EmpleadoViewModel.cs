using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class EmpleadoViewModel
    {

        [Key]
        public int Emp_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Emp_Codigo { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Per_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Tit_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Car_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Emp_EsActivo { get; set; }

    }
}