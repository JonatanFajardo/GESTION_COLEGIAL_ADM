using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ModalidadViewModel
    {
        [Key]
        public int Mda_Id { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Mda_Descripcion { get; set; }
    }
}