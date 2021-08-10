using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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